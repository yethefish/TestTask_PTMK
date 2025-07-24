namespace Entities
{
    public class User : Person
    {
        public User() { }

        internal User(string firstname, string? patronymic, string lastname, DateTime birthday, string sex)
        {
            FirstName = firstname;
            Patronymic = patronymic;
            LastName = lastname;
            Birthday = birthday;
            Sex = sex;
            Age = DateTime.Today.Year - birthday.Year;
            if (birthday.Date > DateTime.Today.AddYears(-Age)) Age--;
        }

        public static User Create(string firstname, string? patronymic, string lastname, DateTime birthday, string sex)
        {
            if (string.IsNullOrWhiteSpace(firstname))
                throw new ArgumentException("First name cannot be empty.", nameof(firstname));
            if (string.IsNullOrWhiteSpace(lastname))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastname));
            if (birthday > DateTime.Now)
                throw new ArgumentException("Birthday cannot be in the future.", nameof(birthday));

            return new User(firstname, patronymic, lastname, birthday, sex);
        }
    }
}