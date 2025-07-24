using Entities;

namespace Services
{
    public interface IUserService
    {
        Task CreateTableIfNotExistsAsync();
        Task<User> CreateUserAsync(string fullName, DateTime birthday, string sex);
        Task<IEnumerable<User>> GetUniqueUsersSortedByNameAsync();
        Task GenerateAndAddUsersAsync(int primaryCount, int fCount);
        Task<(IEnumerable<User>, TimeSpan)> FindMalesWithLastNameStartingWithFAsync();
        Task CreatePerformanceIndexAsync();
        Task DeleteUserAsync(int id);   
    }
}