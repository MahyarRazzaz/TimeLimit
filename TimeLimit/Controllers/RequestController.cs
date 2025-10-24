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
        private readonly ILogger<RequestController> _logger; 

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

            //  بررسی وجود کاربر
            var user = await _ctx.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == dto.TargetPhoneNumber);

            if (user == null)
            {
                user = new User { PhoneNumber = dto.TargetPhoneNumber };
                _ctx.Users.Add(user);
                await _ctx.SaveChangesAsync();
            }

            //  پیدا کردن آخرین درخواست کاربر
            var lastRequest = await _ctx.Requests
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.RequestedAtUtc)
                .FirstOrDefaultAsync();

            //  ثبت درخواست جدید
            var currentRequest = new Request
            {
                TargetPhoneNumber = dto.TargetPhoneNumber,
                UserId = user.Id,
                RequestedAtUtc = DateTime.UtcNow
            };

            _ctx.Requests.Add(currentRequest);
            await _ctx.SaveChangesAsync();

            //   پاسخ‌دهی
            if (lastRequest == null)
            {
                //   پاسخی نده
                return NoContent(); // 204 No Content
            }
            else
            {
                //  زمان بین دو درخواست
                var timeDiff = (currentRequest.RequestedAtUtc - lastRequest.RequestedAtUtc);
                return Ok(new
                {
                    PreviousRequestId = lastRequest.Id,
                    TimeDifferenceSeconds = timeDiff.TotalSeconds,
                    Message = " پاسخ درخواست قبلی"
                });
            }
        }
    }
}
