using TimeLimit.Models;

namespace TimeLimit.Repositories
{
    public interface IRequestRepository
    {
        Task<Request?> GetLastAsync(string targetPhoneNumber);
        Task EnsureUserAsync(string targetPhoneNumber);
        Task AddRequestAsync(Request request);
        Task SaveAsync();
    }
}
