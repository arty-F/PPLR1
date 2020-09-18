using System.Linq;

namespace PPLR1.Models
{
    internal class Student
    {
        internal string LastName { get; private set; }
        internal string FirstName { get; private set; }
        internal string Patronymic { get; private set; }
        internal string GroupNumber { get; private set; }
        internal int Priority { get; private set; }
        internal Subject SubjectToPassing { get; set; }  //Предмет, который необходимо сдать
        internal string Id { get; private set; }
        internal CurrentThreadState CurrentState { get; set; } = CurrentThreadState.W;

        internal Student(string lastName, string firstName, string patronymic, string groupNumber, int priority, Subject subject)
        {
            LastName = lastName;
            FirstName = firstName;
            Patronymic = patronymic;
            GroupNumber = groupNumber;
            Priority = priority;
            SubjectToPassing = subject;
            Id = $"#{LastName.First()}{FirstName.First()}{Patronymic.First()}{Priority.ToString("00")}";
        }

        internal string Status() => $"{Id}:{CurrentState} (CPU:{SubjectToPassing.RemainingTime})";

        public override string ToString() => $"{LastName} {FirstName} {Patronymic} приоритет - {Priority} (Id: {Id})";
    }
}
