using System.Linq;

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
        internal int MaxNumberOfStudents { get; private set; }
        internal string Id { get; private set; }

        internal Teacher(string lastName, string firstName, string patronymic, int numberOfStudent)
        {
            LastName = lastName;
            FirstName = firstName;
            Patronymic = patronymic;
            NumberOfStudents = numberOfStudent;
            MaxNumberOfStudents = NumberOfStudents;
            Id = $"{LastName.First()}{FirstName.First()}{Patronymic.First()}";
        }

        internal string Status() => $"{Id} {NumberOfStudents}/{MaxNumberOfStudents}";

        public override string ToString() => $"{LastName} {FirstName} {Patronymic} {NumberOfStudents}/{MaxNumberOfStudents} (Id: {Id})";
    }
}
