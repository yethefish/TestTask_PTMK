using Dapper;
using Entities;
using Npgsql;
using System.Data;

namespace Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly NpgsqlConnection _npgsqlConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            if (dbConnection is not NpgsqlConnection npgsqlConnection)
            {
                throw new InvalidOperationException("This repository requires an NpgsqlConnection.");
            }
            _npgsqlConnection = npgsqlConnection;
        }

        public async Task CreateTableIfNotExistsAsync()
        {
            var sql = """
                DROP TABLE IF EXISTS "users";

                CREATE TABLE users (
                    user_id SERIAL NOT NULL,
                    firstname varchar(32),
                    patronymic varchar(32),
                    lastname varchar(32),
                    birthday date NOT NULL,
                    age int,
                    sex varchar(8),
                    PRIMARY KEY (user_id)
                );
                """;

            if (_npgsqlConnection.State == ConnectionState.Closed)
            {
                await _npgsqlConnection.OpenAsync();
            }
            
            await _dbConnection.ExecuteAsync(sql);
        }

        public async Task<int> AddAsync(User user)
        {
            var sql = """
                INSERT INTO users (firstname, patronymic, lastname, birthday, age, sex)
                VALUES (@FirstName, @Patronymic, @LastName, @Birthday, @Age, @Sex)
                RETURNING user_id;
                """;
            return await _dbConnection.ExecuteScalarAsync<int>(sql, user);
        }

        public async Task AddBatchAsync(IEnumerable<User> users)
        {

            if (_npgsqlConnection.State == ConnectionState.Closed)
            {
                await _npgsqlConnection.OpenAsync();
            }

            await using var importer = await _npgsqlConnection.BeginBinaryImportAsync(
                "COPY users (firstname, patronymic, lastname, birthday, age, sex) FROM STDIN (FORMAT BINARY)");
            
            foreach (var user in users)
            {
                await importer.StartRowAsync();
                await importer.WriteAsync(user.FirstName);
                await importer.WriteAsync(user.Patronymic, NpgsqlTypes.NpgsqlDbType.Varchar);
                await importer.WriteAsync(user.LastName);
                await importer.WriteAsync(user.Birthday, NpgsqlTypes.NpgsqlDbType.Date);
                await importer.WriteAsync(user.Age);
                await importer.WriteAsync(user.Sex);
            }

            await importer.CompleteAsync();
        }


        public async Task<User?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM users WHERE user_id = @Id;";
            return await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var sql = "SELECT * FROM users ORDER BY user_id;";
            return await _dbConnection.QueryAsync<User>(sql);
        }
        
        public async Task<IEnumerable<User>> GetUniqueUsersSortedByNameAsync()
        {
            var sql = """
                SELECT DISTINCT ON (lastname, firstname, patronymic, birthday) *
                FROM users
                ORDER BY lastname, firstname, patronymic, birthday;
                """;
            return await _dbConnection.QueryAsync<User>(sql);
        }
        
        public async Task<IEnumerable<User>> FindMalesWithLastNameStartingWithFAsync()
        {
            var sql = "SELECT * FROM users WHERE sex = 'Male' AND lastname LIKE 'F%';";
            return await _dbConnection.QueryAsync<User>(sql);
        }

        public async Task UpdateAsync(User user)
        {
            var sql = """
                UPDATE users SET firstname = @FirstName, patronymic = @Patronymic, lastname = @LastName,
                                 birthday = @Birthday, age = @Age, sex = @Sex
                WHERE user_id = @UserId;
                """;
            await _dbConnection.ExecuteAsync(sql, user);
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM users WHERE user_id = @Id;";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task CreatePerformanceIndexAsync()
        {
            var sql = "CREATE INDEX IF NOT EXISTS idx_users_sex_lastname ON users (sex, lastname);";
            await _dbConnection.ExecuteAsync(sql);
        }
    }
}