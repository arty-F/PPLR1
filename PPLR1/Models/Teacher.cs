namespace PPLR1.Models
{
    /// <summary>
    /// Преподаватель
    /// </summary>
    internal class Teacher
    {
        internal string LastName { get; private set; }
        internal string FirstName { get; private set; }
        internal string Patronymic { get; private set; }
        internal int NumberOfStudents { get; set; }

        internal Teacher(string lastName, string firstName, string patronymic, int numberOfStudent)
        {
            LastName = lastName;
            FirstName = firstName;
            Patronymic = patronymic;
            NumberOfStudents = numberOfStudent;
        }

        public override string ToString() => $"{LastName} {FirstName} {Patronymic} (осталось :{NumberOfStudents})";
    }
}
