using System.Linq;

namespace PPLR1.Models
{
    /// <summary>
    /// Лабораторное оборудование
    /// </summary>
    internal class Equipment
    {
        internal string Name { get; private set; }
        internal int Count { get; set; }
        internal int MaxCount { get; private set; }
        internal string Id { get; private set; }

        internal Equipment(string name, int count)
        {
            Name = name;
            Count = count;
            MaxCount = Count;
            Id = string.Join('-', Name.Split(' ').Select(s => s.Substring(0, 3)));
        }

        internal string Status() => $"{Id} {Count}/{MaxCount}";

        public override string ToString() => $"{Name} {Count}/{MaxCount} (Id:{Id})";
    }
}
