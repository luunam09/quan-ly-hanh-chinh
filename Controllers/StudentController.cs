using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAdminPortal.Data;
using StudentAdminPortal.Models;
using StudentAdminPortal.Services;
using StudentAdminPortal.ViewModels;

namespace StudentAdminPortal.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly IQrCodeService _qrCodeService;

    public StudentController(
        ApplicationDbContext context,
        IFileService fileService,
        IQrCodeService qrCodeService)
    {
        _context = context;
        _fileService = fileService;
        _qrCodeService = qrCodeService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        int studentId = CurrentUserId;
        var student = await _context.Users.FindAsync(studentId);
        if (student == null) return NotFound();

        var requests = await _context.Requests
            .Include(r => r.Procedure)
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var procedures = await _context.Procedures
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        var model = new StudentDashboardViewModel
        {
            Student = student,
            TotalRequests = requests.Count,
            PendingRequests = requests.Count(r => r.Status == RequestStatus.Pending),
            ProcessingRequests = requests.Count(r => r.Status == RequestStatus.Processing),
            NeedUpdateRequests = requests.Count(r => r.Status == RequestStatus.NeedUpdate),
            ApprovedRequests = requests.Count(r => r.Status == RequestStatus.Approved),
            RejectedRequests = requests.Count(r => r.Status == RequestStatus.Rejected),
            RecentRequests = requests.Take(5).ToList(),
            AvailableProcedures = procedures
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Procedures()
    {
        var procedures = await _context.Procedures
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(procedures);
    }

    [HttpGet]
    public async Task<IActionResult> CreateRequest(int? procedureId)
    {
        if (!procedureId.HasValue)
        {
            return RedirectToAction(nameof(Procedures));
        }

        var procedure = await _context.Procedures.FindAsync(procedureId.Value);
        if (procedure == null || !procedure.IsActive)
        {
            TempData["ErrorMessage"] = "Thủ tục không tồn tại hoặc đã tạm dừng tiếp nhận.";
            return RedirectToAction(nameof(Procedures));
        }

        var model = new CreateRequestViewModel
        {
            ProcedureId = procedure.Id,
            Procedure = procedure,
            Quantity = 1,
            ReceiveMethod = "Digital"
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRequest(CreateRequestViewModel model)
    {
        var procedure = await _context.Procedures.FindAsync(model.ProcedureId);
        if (procedure == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Procedure = procedure;
            return View(model);
        }

        int studentId = CurrentUserId;
        var student = await _context.Users.FindAsync(studentId);

        // Generate unique request code: YC-2026-XXXX
        int countToday = await _context.Requests.CountAsync() + 1;
        string requestCode = $"YC-{DateTime.UtcNow.Year}-{countToday:D4}";

        var request = new AdministrativeRequest
        {
            RequestCode = requestCode,
            StudentId = studentId,
            ProcedureId = model.ProcedureId,
            Status = RequestStatus.Pending,
            Purpose = model.Purpose.Trim(),
            Quantity = model.Quantity,
            ReceiveMethod = model.ReceiveMethod,
            StudentNote = model.StudentNote?.Trim(),
            ExpectedDate = DateTime.UtcNow.AddDays(procedure.ProcessingDays),
            CreatedAt = DateTime.UtcNow
        };

        _context.Requests.Add(request);
        await _context.SaveChangesAsync();

        // Handle uploaded attachments
        if (model.Attachments != null && model.Attachments.Count > 0)
        {
            foreach (var file in model.Attachments)
            {
                if (file.Length > 0)
                {
                    var (relativePath, originalName, size, contentType) = await _fileService.SaveFileAsync(file, "requests");
                    var attachment = new RequestAttachment
                    {
                        RequestId = request.Id,
                        FileName = originalName,
                        FilePath = relativePath,
                        FileType = contentType,
                        FileSize = size,
                        IsSupplementary = false,
                        UploadedAt = DateTime.UtcNow
                    };
                    _context.Attachments.Add(attachment);
                }
            }
        }

        // Add history log
        var log = new RequestLog
        {
            RequestId = request.Id,
            ActorName = student?.FullName ?? User.Identity?.Name ?? "Sinh viên",
            ActorRole = "Student",
            Action = "Tạo mới yêu cầu",
            Note = $"Sinh viên gửi yêu cầu trực tuyến: {procedure.Name}",
            CreatedAt = DateTime.UtcNow
        };
        _context.RequestLogs.Add(log);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Gửi yêu cầu thành công! Mã hồ sơ của bạn là: {requestCode}";
        return RedirectToAction(nameof(Details), new { id = request.Id });
    }

    [HttpGet]
    public async Task<IActionResult> MyRequests(string? status, string? search)
    {
        int studentId = CurrentUserId;
        var query = _context.Requests
            .Include(r => r.Procedure)
            .Where(r => r.StudentId == studentId);

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status == status);
        }

        if (!string.IsNullOrEmpty(search))
        {
            string term = search.Trim().ToLower();
            query = query.Where(r => r.RequestCode.ToLower().Contains(term) ||
                                     r.Procedure!.Name.ToLower().Contains(term));
        }

        var list = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        ViewBag.SelectedStatus = status;
        ViewBag.SearchTerm = search;

        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        int studentId = CurrentUserId;
        var request = await _context.Requests
            .Include(r => r.Procedure)
            .Include(r => r.Attachments)
            .Include(r => r.Logs.OrderBy(l => l.CreatedAt))
            .FirstOrDefaultAsync(r => r.Id == id && r.StudentId == studentId);

        if (request == null)
        {
            return NotFound();
        }

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitSupplement(SupplementRequestViewModel model)
    {
        int studentId = CurrentUserId;
        var request = await _context.Requests
            .Include(r => r.Procedure)
            .FirstOrDefaultAsync(r => r.Id == model.RequestId && r.StudentId == studentId);

        if (request == null) return NotFound();

        if (request.Status != RequestStatus.NeedUpdate)
        {
            TempData["ErrorMessage"] = "Hồ sơ hiện tại không ở trạng thái yêu cầu bổ sung.";
            return RedirectToAction(nameof(Details), new { id = model.RequestId });
        }

        if (model.Files == null || model.Files.Count == 0)
        {
            TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một tệp để bổ sung.";
            return RedirectToAction(nameof(Details), new { id = model.RequestId });
        }

        foreach (var file in model.Files)
        {
            if (file.Length > 0)
            {
                var (relativePath, originalName, size, contentType) = await _fileService.SaveFileAsync(file, "requests");
                var attachment = new RequestAttachment
                {
                    RequestId = request.Id,
                    FileName = originalName,
                    FilePath = relativePath,
                    FileType = contentType,
                    FileSize = size,
                    IsSupplementary = true,
                    UploadedAt = DateTime.UtcNow
                };
                _context.Attachments.Add(attachment);
            }
        }

        // Change status back to Processing
        request.Status = RequestStatus.Processing;
        request.UpdatedAt = DateTime.UtcNow;

        var student = await _context.Users.FindAsync(studentId);
        var log = new RequestLog
        {
            RequestId = request.Id,
            ActorName = student?.FullName ?? "Sinh viên",
            ActorRole = "Student",
            Action = "Bổ sung hồ sơ",
            Note = string.IsNullOrWhiteSpace(model.Note) ? "Sinh viên đã nộp bổ sung giấy tờ theo yêu cầu." : $"Bổ sung: {model.Note}",
            CreatedAt = DateTime.UtcNow
        };
        _context.RequestLogs.Add(log);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Nộp bổ sung giấy tờ thành công! Hồ sơ đã chuyển sang trạng thái Đang xử lý.";
        return RedirectToAction(nameof(Details), new { id = request.Id });
    }

    [HttpGet]
    public async Task<IActionResult> ViewCertificate(int id)
    {
        int studentId = CurrentUserId;
        var request = await _context.Requests
            .Include(r => r.Procedure)
            .Include(r => r.Student)
            .FirstOrDefaultAsync(r => r.Id == id && r.StudentId == studentId);

        if (request == null || request.Status != RequestStatus.Approved)
        {
            TempData["ErrorMessage"] = "Chỉ xem được giấy xác nhận điện tử khi hồ sơ đã được phê duyệt.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // If verification code is missing, generate one
        if (string.IsNullOrEmpty(request.VerificationCode))
        {
            request.VerificationCode = $"VERIF-{request.RequestCode.Replace("-", "")}";
            await _context.SaveChangesAsync();
        }

        // Generate QR code for verification URL
        string verifyUrl = $"{Request.Scheme}://{Request.Host}/Home/Lookup?requestCode={request.RequestCode}";
        string qrCodeBase64 = _qrCodeService.GenerateQrCodeBase64(verifyUrl);

        ViewBag.QrCodeBase64 = qrCodeBase64;
        ViewBag.VerifyUrl = verifyUrl;

        return View(request);
    }
}
