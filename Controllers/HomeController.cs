using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAdminPortal.Data;
using StudentAdminPortal.Models;
using StudentAdminPortal.Services;

namespace StudentAdminPortal.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IQrCodeService _qrCodeService;

    public HomeController(ApplicationDbContext context, IQrCodeService qrCodeService)
    {
        _context = context;
        _qrCodeService = qrCodeService;
    }

    public async Task<IActionResult> Index()
    {
        var procedures = await _context.Procedures
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Take(8)
            .ToListAsync();

        ViewBag.TotalCompleted = await _context.Requests.CountAsync(r => r.Status == RequestStatus.Approved);
        ViewBag.TotalStudents = await _context.Users.CountAsync(u => u.Role == "Student");
        ViewBag.TotalProcedures = await _context.Procedures.CountAsync(p => p.IsActive);

        return View(procedures);
    }

    [HttpGet]
    public async Task<IActionResult> Lookup(string? requestCode)
    {
        if (string.IsNullOrWhiteSpace(requestCode))
        {
            return View();
        }

        string code = requestCode.Trim().ToUpper();
        var request = await _context.Requests
            .Include(r => r.Procedure)
            .Include(r => r.Student)
            .Include(r => r.Logs.OrderBy(l => l.CreatedAt))
            .FirstOrDefaultAsync(r => r.RequestCode.ToUpper() == code);

        ViewBag.SearchCode = code;

        if (request != null && request.Status == RequestStatus.Approved)
        {
            string verifyUrl = $"{Request.Scheme}://{Request.Host}/Home/Lookup?requestCode={request.RequestCode}";
            ViewBag.QrCodeBase64 = _qrCodeService.GenerateQrCodeBase64(verifyUrl);
        }

        return View(request);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
