using Entities;
using System.Text;

namespace Helpers
{
    public static class DataGenerator
    {
        private static readonly Random _random = new();

        private static readonly string[] _maleFirstNames = { "Alexander", "Ivan", "Maxim", "Dmitry", "Artem" };
        private static readonly string[] _femaleFirstNames = { "Sophia", "Anna", "Maria", "Victoria", "Daria" };
        private static readonly string[] _lastNames = { "Ivanov", "Smirnov", "Kuznetsov", "Popov", "Vasiliev", "Petrov", "Sokolov", "Mikhailov", "Fedorov", "Morozov" };
        private static readonly string[] _malePatronymics = { "Sergeevich", "Alexandrovich", "Dmitrievich", "Andreevich", "Ivanovich" };
        private static readonly string[] _femalePatronymics = { "Sergeevna", "Alexandrovna", "Dmitrievna", "Andreevna", "Ivanovna" };

        public static IEnumerable<User> GenerateRandomUsers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var sex = _random.Next(0, 2) == 0 ? "Male" : "Female";
                var firstName = sex == "Male" ? _maleFirstNames[_random.Next(_maleFirstNames.Length)] : _femaleFirstNames[_random.Next(_femaleFirstNames.Length)];

                var lastNameBuilder = new StringBuilder(_lastNames[_random.Next(_lastNames.Length)]);
                if (sex == "Female")
                {
                    lastNameBuilder.Append("a");
                }
                var lastName = lastNameBuilder.ToString();

                var patronymic = sex == "Male" ? _malePatronymics[_random.Next(_malePatronymics.Length)] : _femalePatronymics[_random.Next(_femalePatronymics.Length)];
                var birthday = new DateTime(_random.Next(1950, 2005), _random.Next(1, 13), _random.Next(1, 29));
                
                yield return User.Create(firstName, patronymic, lastName, birthday, sex);
            }
        }

        public static IEnumerable<User> GenerateFAndMaleUsers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                 var firstName = _maleFirstNames[_random.Next(_maleFirstNames.Length)];
                 var patronymic = _malePatronymics[_random.Next(_malePatronymics.Length)];
                 var birthday = new DateTime(_random.Next(1950, 2005), _random.Next(1, 13), _random.Next(1, 29));
                 
                 yield return User.Create(firstName, patronymic, "Fedorov", birthday, "Male");
            }
        }
    }
}