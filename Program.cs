using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Data;
using Services;
using Npgsql;
using System.Data;
using System.Globalization;
using Entities;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string not found.");
        }
        
        services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
    })
    .Build();

await RunApplicationAsync(host.Services, args);

static async Task RunApplicationAsync(IServiceProvider services, string[] args)
{
    if (args.Length == 0)
    {
        PrintUsage();
        return;
    }

    using var scope = services.CreateScope();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

    var mode = args[0];

    try
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Executing mode: {mode}");
        Console.ResetColor();

        switch (mode)
        {
            case "1":
                await Mode1_CreateTable(userService);
                break;
            case "2":
                await Mode2_CreateUser(userService, args);
                break;
            case "3":
                await Mode3_ShowAllUniqueUsers(userService);
                break;
            case "4":
                await Mode4_GenerateData(userService);
                break;
            case "5":
                await Mode5_RunQueryWithTiming(userService);
                break;
            case "6":
                await Mode6_OptimizeAndCompare(userService);
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Unknown mode '{mode}'.");
                Console.ResetColor();
                PrintUsage();
                break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nAn error occurred: {ex.Message}");
        Console.ResetColor();
    }
}

static async Task Mode1_CreateTable(IUserService userService)
{
    Console.WriteLine("-> Creating table 'users' if it does not exist...");
    await userService.CreateTableIfNotExistsAsync();
    Console.WriteLine("Done. Table 'users' is ready.");
}

static async Task Mode2_CreateUser(IUserService userService, string[] args)
{
    if (args.Length != 4)
    {
        throw new ArgumentException("Invalid arguments for mode 2.\nUsage: 2 \"FullName\" YYYY-MM-DD Sex");
    }

    var fullName = args[1];
    if (!DateTime.TryParseExact(args[2], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthday))
    {
        throw new ArgumentException($"Invalid date format: {args[2]}. Please use YYYY-MM-DD.");
    }
    var sex = args[3];

    Console.WriteLine($"-> Creating user: {fullName}, Born: {birthday:d}, Sex: {sex}...");
    var newUser = await userService.CreateUserAsync(fullName, birthday, sex);
    Console.WriteLine($"User '{newUser.LastName} {newUser.FirstName}' created successfully with Id {newUser.UserId}.");
}

static async Task Mode3_ShowAllUniqueUsers(IUserService userService)
{
    Console.WriteLine("-> Fetching unique users sorted by full name...");
    var users = await userService.GetUniqueUsersSortedByNameAsync();
    
    if (!users.Any())
    {
        Console.WriteLine("No users found.");
        return;
    }
    
    Console.WriteLine("\nUnique users in database (sorted by Full Name):");
    Console.WriteLine("---------------------------------------------------------------------------------");
    Console.WriteLine($"{"Full Name",-35} | {"Birthday",-12} | {"Sex",-8} | {"Age"}");
    Console.WriteLine("---------------------------------------------------------------------------------");
    foreach (var user in users)
    {
        var fullName = $"{user.LastName} {user.FirstName} {user.Patronymic}".Trim();
        Console.WriteLine($"{fullName,-35} | {user.Birthday:yyyy-MM-dd} | {user.Sex,-8} | {user.Age}");
    }
    Console.WriteLine("---------------------------------------------------------------------------------");
}

static async Task Mode4_GenerateData(IUserService userService)
{
    Console.WriteLine("-> Populating database with 1,000,000 random users and 100 male users with last name starting with 'F'...");
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    await userService.GenerateAndAddUsersAsync(1_000_000, 100);
    stopwatch.Stop();
    Console.WriteLine($"\nDone. Database population took: {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
}

static async Task Mode5_RunQueryWithTiming(IUserService userService)
{
    Console.WriteLine("-> Querying for male users with last name starting with 'F'...");
    var (users, timeTaken) = await userService.FindMalesWithLastNameStartingWithFAsync();
    Console.WriteLine($"Query found {users.Count()} users.");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Execution Time: {timeTaken.TotalMilliseconds:F4} ms");
    Console.ResetColor();
}

static async Task Mode6_OptimizeAndCompare(IUserService userService)
{
    Console.WriteLine("--- Optimization Test ---");
    
    Console.WriteLine("\nStep 1: Running query on non-optimized table...");
    await Mode5_RunQueryWithTiming(userService);

    Console.WriteLine("\nStep 2: Creating performance index on (sex, lastname)...");
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    await userService.CreatePerformanceIndexAsync();
    stopwatch.Stop();
    Console.WriteLine($"Index created successfully. Time taken: {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
    
    Console.WriteLine("\nStep 3: Running the same query on OPTIMIZED table...");
    await Mode5_RunQueryWithTiming(userService);
    
    Console.WriteLine("\n--- Explanation ---");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("The initial query performs a full table scan.");
    Console.WriteLine("By creating a B-tree index for the 'sex' and 'lastname' fields, ");
    Console.WriteLine("the database was pre-sorted for queries on these fields.");
    Console.ResetColor();
}


static void PrintUsage()
{
    Console.WriteLine("""
    Modes:
      1                               - Creates the database table.
      2 "<LastName FirstName Patronymic>" "YYYY-MM-DD" "<Sex>" - Creates a new user.
      3                               - Displays all unique users, sorted by name.
      4                               - Populates DB with 1,000,000 random + 100 'F' male users.
      5                               - Times the query for male users with last name starting 'F'.
      6                               - Runs optimization test: times query before and after creating an index and explains.

    Example:
      dotnet run -- 2 "Ivanov Petr Sergeevich" "2009-07-12" "Male"
    """);
}