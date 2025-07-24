namespace Entities
{
    public abstract class Person
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? Patronymic { get; set; }
        public string LastName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; } = string.Empty;
    }
}