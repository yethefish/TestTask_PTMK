using Entities;

namespace Data
{
    public interface IUserRepository
    {
        Task CreateTableIfNotExistsAsync();
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetUniqueUsersSortedByNameAsync();
        Task<IEnumerable<User>> FindMalesWithLastNameStartingWithFAsync();
        Task<int> AddAsync(User user);
        Task AddBatchAsync(IEnumerable<User> users);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task CreatePerformanceIndexAsync();
    }
}