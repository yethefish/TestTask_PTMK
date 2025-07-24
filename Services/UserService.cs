using Data;
using Entities;
using Helpers;
using System.Diagnostics;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateTableIfNotExistsAsync()
        {
            await _userRepository.CreateTableIfNotExistsAsync();
        }


        public async Task<User> CreateUserAsync(string fullName, DateTime birthday, string sex)
        {
            var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length < 2)
            {
                throw new ArgumentException("Full name must contain at least a last name and a first name.");
            }

            var lastName = nameParts[0];
            var firstName = nameParts[1];
            var patronymic = nameParts.Length > 2 ? string.Join(" ", nameParts.Skip(2)) : null;

            var user = User.Create(firstName, patronymic, lastName, birthday, sex);
            
            var newId = await _userRepository.AddAsync(user);
            user.UserId = newId; 
            
            return user;
        }
        
        public async Task<IEnumerable<User>> GetUniqueUsersSortedByNameAsync()
        {
            return await _userRepository.GetUniqueUsersSortedByNameAsync();
        }

        public async Task GenerateAndAddUsersAsync(int primaryCount, int fCount)
        {
            Console.WriteLine("Generating users...");
            var users = DataGenerator.GenerateRandomUsers(primaryCount).ToList();
            users.AddRange(DataGenerator.GenerateFAndMaleUsers(fCount));
            
            Console.WriteLine($"Total users generated: {users.Count}. Starting database insertion...");
            
            await _userRepository.AddBatchAsync(users);
        }
        
        public async Task<(IEnumerable<User>, TimeSpan)> FindMalesWithLastNameStartingWithFAsync()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var users = await _userRepository.FindMalesWithLastNameStartingWithFAsync();
            
            stopwatch.Stop();
            return (users, stopwatch.Elapsed);
        }

        public async Task CreatePerformanceIndexAsync()
        {
            await _userRepository.CreatePerformanceIndexAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                Console.WriteLine($"Warning: User with Id {id} not found. Nothing to delete.");
                return;
            }
            await _userRepository.DeleteAsync(id);
        }
    }
}