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

        internal Student(string lastName, string firstName, string patronymic, string groupNumber, int priority, Subject subject)
        {
            LastName = lastName;
            FirstName = firstName;
            Patronymic = patronymic;
            GroupNumber = groupNumber;
            Priority = priority;
            SubjectToPassing = subject;
        }

        public override string ToString() => $"{LastName} {FirstName} {Patronymic} :{Priority}";
    }
}
