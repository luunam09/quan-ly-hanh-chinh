using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAdminPortal.Data;
using StudentAdminPortal.Models;
using StudentAdminPortal.Services;
using StudentAdminPortal.ViewModels;

namespace StudentAdminPortal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public AdminController(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var requests = await _context.Requests
            .Include(r => r.Procedure)
            .Include(r => r.Student)
            .ToListAsync();

        var urgentRequests = requests
            .Where(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.Processing)
            .OrderBy(r => r.CreatedAt)
            .Take(6)
            .ToList();

        var byProcedure = requests
            .Where(r => r.Procedure != null)
            .GroupBy(r => r.Procedure!.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        var byFaculty = requests
            .Where(r => r.Student != null && !string.IsNullOrEmpty(r.Student.Faculty))
            .GroupBy(r => r.Student!.Faculty!)
            .ToDictionary(g => g.Key, g => g.Count());

        var model = new AdminDashboardViewModel
        {
            TotalRequests = requests.Count,
            PendingRequests = requests.Count(r => r.Status == RequestStatus.Pending),
            ProcessingRequests = requests.Count(r => r.Status == RequestStatus.Processing),
            NeedUpdateRequests = requests.Count(r => r.Status == RequestStatus.NeedUpdate),
            ApprovedRequests = requests.Count(r => r.Status == RequestStatus.Approved),
            RejectedRequests = requests.Count(r => r.Status == RequestStatus.Rejected),
            TotalStudents = await _context.Users.CountAsync(u => u.Role == "Student"),
            TotalProcedures = await _context.Procedures.CountAsync(),
            UrgentRequests = urgentRequests,
            RequestsByProcedure = byProcedure,
            RequestsByFaculty = byFaculty
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Requests(string? status, int? procedureId, string? search, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Requests
            .Include(r => r.Procedure)
            .Include(r => r.Student)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status == status);
        }

        if (procedureId.HasValue && procedureId.Value > 0)
        {
            query = query.Where(r => r.ProcedureId == procedureId.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            string term = search.Trim().ToLower();
            query = query.Where(r => r.RequestCode.ToLower().Contains(term) ||
                                     r.Student!.FullName.ToLower().Contains(term) ||
                                     r.Student.Username.ToLower().Contains(term));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(r => r.CreatedAt >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            query = query.Where(r => r.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
        }

        var list = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var procedures = await _context.Procedures.OrderBy(p => p.Name).ToListAsync();

        var model = new RequestListFilterViewModel
        {
            Status = status,
            ProcedureId = procedureId,
            Search = search,
            FromDate = fromDate,
            ToDate = toDate,
            Requests = list,
            Procedures = procedures
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Review(int id)
    {
        var request = await _context.Requests
            .Include(r => r.Procedure)
            .Include(r => r.Student)
            .Include(r => r.Attachments)
            .Include(r => r.Logs.OrderByDescending(l => l.CreatedAt))
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            return NotFound();
        }

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(ReviewRequestViewModel model)
    {
        var request = await _context.Requests
            .Include(r => r.Procedure)
            .Include(r => r.Student)
            .FirstOrDefaultAsync(r => r.Id == model.RequestId);

        if (request == null) return NotFound();

        string oldStatus = request.Status;
        request.Status = model.NewStatus;
        request.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(model.AdminFeedback))
        {
            request.AdminFeedback = model.AdminFeedback.Trim();
        }

        if (!string.IsNullOrWhiteSpace(model.ResultNote))
        {
            request.ResultNote = model.ResultNote.Trim();
        }

        if (model.ExpectedDate.HasValue)
        {
            request.ExpectedDate = model.ExpectedDate.Value;
        }

        // Result File upload
        if (model.ResultFile != null && model.ResultFile.Length > 0)
        {
            var (relativePath, _, _, _) = await _fileService.SaveFileAsync(model.ResultFile, "results");
            request.ResultFileUrl = relativePath;
        }

        if (model.NewStatus == RequestStatus.Approved)
        {
            request.CompletedAt = DateTime.UtcNow;
            if (string.IsNullOrEmpty(request.VerificationCode))
            {
                request.VerificationCode = $"VERIF-{request.RequestCode.Replace("-", "")}";
            }
        }
        else if (model.NewStatus == RequestStatus.Rejected)
        {
            request.CompletedAt = DateTime.UtcNow;
        }

        // Action Name
        string actionName = model.NewStatus switch
        {
            RequestStatus.Processing => "Tiếp nhận & Xử lý",
            RequestStatus.NeedUpdate => "Yêu cầu bổ sung",
            RequestStatus.Approved => "Phê duyệt hồ sơ",
            RequestStatus.Rejected => "Từ chối tiếp nhận",
            _ => $"Chuyển trạng thái: {model.NewStatus}"
        };

        var log = new RequestLog
        {
            RequestId = request.Id,
            ActorName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name ?? "Quản trị viên",
            ActorRole = "Admin",
            Action = actionName,
            Note = model.AdminFeedback ?? model.ResultNote ?? $"Chuyển từ {RequestStatus.ToVietnamese(oldStatus)} sang {RequestStatus.ToVietnamese(model.NewStatus)}",
            CreatedAt = DateTime.UtcNow
        };
        _context.RequestLogs.Add(log);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Đã cập nhật trạng thái hồ sơ {request.RequestCode} thành công!";
        return RedirectToAction(nameof(Review), new { id = request.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Procedures()
    {
        var procedures = await _context.Procedures
            .Include(p => p.Requests)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(procedures);
    }

    [HttpGet]
    public IActionResult CreateProcedure()
    {
        return View(new Procedure { ProcessingDays = 3, IsActive = true, IconClass = "bi-file-earmark-text" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProcedure(Procedure model)
    {
        if (await _context.Procedures.AnyAsync(p => p.Code.ToUpper() == model.Code.Trim().ToUpper()))
        {
            ModelState.AddModelError("Code", "Mã thủ tục này đã tồn tại, vui lòng chọn mã khác.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Code = model.Code.Trim().ToUpper();
        model.Name = model.Name.Trim();
        model.CreatedAt = DateTime.UtcNow;

        _context.Procedures.Add(model);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Đã thêm mới thủ tục: {model.Name}";
        return RedirectToAction(nameof(Procedures));
    }

    [HttpGet]
    public async Task<IActionResult> EditProcedure(int id)
    {
        var procedure = await _context.Procedures.FindAsync(id);
        if (procedure == null) return NotFound();
        return View(procedure);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProcedure(int id, Procedure model)
    {
        var procedure = await _context.Procedures.FindAsync(id);
        if (procedure == null) return NotFound();

        if (await _context.Procedures.AnyAsync(p => p.Code.ToUpper() == model.Code.Trim().ToUpper() && p.Id != id))
        {
            ModelState.AddModelError("Code", "Mã thủ tục này đã bị trùng.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        procedure.Code = model.Code.Trim().ToUpper();
        procedure.Name = model.Name.Trim();
        procedure.Description = model.Description;
        procedure.ProcessingDays = model.ProcessingDays;
        procedure.RequiredDocuments = model.RequiredDocuments;
        procedure.IconClass = model.IconClass;
        procedure.IsActive = model.IsActive;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Đã cập nhật thủ tục: {procedure.Name}";
        return RedirectToAction(nameof(Procedures));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleProcedureStatus(int id)
    {
        var procedure = await _context.Procedures.FindAsync(id);
        if (procedure == null) return NotFound();

        procedure.IsActive = !procedure.IsActive;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Đã {(procedure.IsActive ? "mở tiếp nhận" : "tạm dừng")} thủ tục: {procedure.Name}";
        return RedirectToAction(nameof(Procedures));
    }

    [HttpGet]
    public async Task<IActionResult> Students(string? search, string? faculty)
    {
        var query = _context.Users
            .Where(u => u.Role == "Student")
            .Include(u => u.Requests)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            string term = search.Trim().ToLower();
            query = query.Where(u => u.Username.ToLower().Contains(term) ||
                                     u.FullName.ToLower().Contains(term) ||
                                     (u.ClassName != null && u.ClassName.ToLower().Contains(term)));
        }

        if (!string.IsNullOrEmpty(faculty))
        {
            query = query.Where(u => u.Faculty == faculty);
        }

        var list = await query.OrderBy(u => u.Username).ToListAsync();
        ViewBag.Faculties = await _context.Users
            .Where(u => u.Role == "Student" && !string.IsNullOrEmpty(u.Faculty))
            .Select(u => u.Faculty!)
            .Distinct()
            .ToListAsync();

        ViewBag.SearchTerm = search;
        ViewBag.SelectedFaculty = faculty;

        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> StudentDetails(int id)
    {
        var student = await _context.Users
            .Include(u => u.Requests)
                .ThenInclude(r => r.Procedure)
            .FirstOrDefaultAsync(u => u.Id == id && u.Role == "Student");

        if (student == null) return NotFound();

        return View(student);
    }
}
