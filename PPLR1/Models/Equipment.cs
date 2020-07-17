namespace PPLR1.Models
{
    /// <summary>
    /// Лабораторное оборудование
    /// </summary>
    internal class Equipment
    {
        internal string Name { get; private set; }
        internal int Count { get; set; }

        internal Equipment(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public override string ToString() => $"{Name} (осталось :{Count})";
    }
}
