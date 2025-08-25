using Microsoft.EntityFrameworkCore;
using TimeLimit.Data;
using TimeLimit.Models;

namespace TimeLimit.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _ctx;
        public RequestRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<Request?> GetLastAsync(string targetPhoneNumber)
        {
            return await _ctx.Requests
                .Where(r => r.TargetPhoneNumber == targetPhoneNumber)
                .OrderByDescending(r => r.RequestedAtUtc)
                .FirstOrDefaultAsync();
        }

        public async Task EnsureUserAsync(string targetPhoneNumber)
        {
            var exists = await _ctx.Users.AnyAsync(u => u.PhoneNumber == targetPhoneNumber);
            if (!exists)
                await _ctx.Users.AddAsync(new User { PhoneNumber = targetPhoneNumber });
        }

        public async Task AddRequestAsync(Request request)
            => await _ctx.Requests.AddAsync(request);

        public Task SaveAsync() => _ctx.SaveChangesAsync();
    }
}
