using System.Collections.Generic;
using System.Linq;

namespace PPLR1.Models
{
    /// <summary>
    /// Предмет для сдачи
    /// </summary>
    internal class Subject
    {
        //Названия необходимых лабораторных установок для сдачи
        internal List<string> EquipmentNames { get; set; } = new List<string>();
        internal int RemainingTime { get; set; } //Оставшееся время CPU burst в квантах

        internal Subject(IEnumerable<string> equipmentNames, int remainingTime)
        {
            EquipmentNames = equipmentNames.ToList();
            RemainingTime = remainingTime;
        }

        internal string Status() => $"{RemainingTime}";

        public override string ToString() => $"{string.Join(", ", EquipmentNames)} ({RemainingTime} CPU)";
    }
}
