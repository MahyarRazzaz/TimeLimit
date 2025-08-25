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

        public RequestController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendOtpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1️⃣ بررسی وجود User
            var user = await _ctx.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == dto.TargetPhoneNumber);

            if (user == null)
            {
                user = new User { PhoneNumber = dto.TargetPhoneNumber };
                _ctx.Users.Add(user);
                await _ctx.SaveChangesAsync(); // ذخیره User جدید و گرفتن Id
            }

            // 2️⃣ بررسی محدودیت ۵ دقیقه
            var lastRequest = await _ctx.Requests
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.RequestedAtUtc)
                .FirstOrDefaultAsync();

            if (lastRequest != null && (DateTime.UtcNow - lastRequest.RequestedAtUtc).TotalMinutes < 5)
            {
                return BadRequest("درخواست تکراری. حدود ۵ دقیقه‌ی دیگر تلاش کنید.");
            }

            // 3️⃣ ثبت Request جدید
            var request = new Request
            {
                TargetPhoneNumber = dto.TargetPhoneNumber,
                UserId = user.Id,
                RequestedAtUtc = DateTime.UtcNow
            };

            _ctx.Requests.Add(request);
            await _ctx.SaveChangesAsync();

            return Ok("کد تأیید ارسال شد.");
        }
    }
}
