using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeLimit.Data;
using TimeLimit.Dtos;
using TimeLimit.Models;

namespace TimeLimit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<RequestController> _logger; // لاگر

        public RequestController(AppDbContext ctx, ILogger<RequestController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendOtpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1️⃣ بررسی وجود کاربر
            var user = await _ctx.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == dto.TargetPhoneNumber);

            if (user == null)
            {
                user = new User { PhoneNumber = dto.TargetPhoneNumber };
                _ctx.Users.Add(user);
                await _ctx.SaveChangesAsync();
            }

            // 2️⃣ پیدا کردن آخرین درخواست کاربر
            var lastRequest = await _ctx.Requests
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.RequestedAtUtc)
                .FirstOrDefaultAsync();

            // 3️⃣ ثبت درخواست جدید
            var currentRequest = new Request
            {
                TargetPhoneNumber = dto.TargetPhoneNumber,
                UserId = user.Id,
                RequestedAtUtc = DateTime.UtcNow
            };

            _ctx.Requests.Add(currentRequest);
            await _ctx.SaveChangesAsync();

            // 4️⃣ منطق پاسخ‌دهی
            if (lastRequest == null)
            {
                // یعنی اولین درخواست است → پاسخی نده
                return NoContent(); // 204 No Content
            }
            else
            {
                // درخواست قبلی وجود داره → زمان بین دو درخواست
                var timeDiff = (currentRequest.RequestedAtUtc - lastRequest.RequestedAtUtc);
                return Ok(new
                {
                    PreviousRequestId = lastRequest.Id,
                    TimeDifferenceSeconds = timeDiff.TotalSeconds,
                    Message = "این پاسخ مربوط به درخواست قبلی است."
                });
            }
        }
    }
}
