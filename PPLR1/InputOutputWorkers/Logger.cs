using PPLR1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PPLR1
{
    /// <summary>
    /// Содержит методы для записи результатов работы программы.
    /// </summary>
    internal class Logger
    {
        private OutputMode mode;
        private StringBuilder sb = new StringBuilder();
        private string path = "";

        /// <summary>
        /// Создать экземпляр логгера, необходимо передать режим вывода <seealso cref="OutputMode"/>.
        /// </summary>
        /// <param name="mode">Режим вывода.</param>
        internal Logger(OutputMode mode)
        {
            this.mode = mode;
            if (mode == OutputMode.File)
            {
                path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output.txt");
                File.WriteAllText(path, string.Empty);
            }
        }

        /// <summary>
        /// Вывод списка студентов.
        /// </summary>
        internal void LogStudents(IEnumerable<Student> students)
        {
            sb.AppendLine("Сформирован список студентов по приоритету:");
            students.ToList().ForEach(s => sb.AppendLine(s + "\t\tТребуется: " + s.SubjectToPassing));
            Output();
        }

        /// <summary>
        /// Вывод занятых ресурсов.
        /// </summary>
        internal void LogTakenResources(ExamProcess exam, PlainType plainType, int queueLevel)
        {
            if (plainType == PlainType.MLQ)
                sb.Append($"(очередь :{queueLevel}) ");
            
            sb.Append($"{exam.Student} занимает: ");
            foreach (var eqName in exam.Student.SubjectToPassing.EquipmentNames)
                sb.Append($"<{exam.Equipments.Where(e => e.Name == eqName).FirstOrDefault()}> ");

            sb.AppendLine($"и начинает сдачу у <{exam.Teacher}>.");
            Output();
        }

        /// <summary>
        /// Вывод при освобождении ресурсов.
        /// </summary>
        internal void LogReleasedResources(ExamProcess exam, int threadId, PlainType plainType, int queueLevel = 0)
        {
            switch (plainType)
            {
                case PlainType.LCFS:
                    sb.Append($"{exam.Student} закончил сдачу в потоке №{threadId} и освобождает: ");
                    break;

                case PlainType.MLQ:
                    if (exam.Student.SubjectToPassing.RemainingTime > 0)
                        sb.Append($"{exam.Student} перемещен в очередь :{queueLevel + 1} (CPU Burst :" +
                            $"{exam.Student.SubjectToPassing.RemainingTime}), и освобождает: ");
                    else
                        sb.Append($"{exam.Student} окончательно закончил сдачу и освобождает: ");
                    break;

                default:
                    throw new Exception("Неизвестный тип планирования.");
            }

            foreach (var eqName in exam.Student.SubjectToPassing.EquipmentNames)
                sb.Append($"<{exam.Equipments.Where(e => e.Name == eqName).FirstOrDefault()}> ");

            sb.AppendLine($"и преподавателя <{exam.Teacher}>.");
            Output();
        }

        /// <summary>
        /// Вывод состояния ресурсов.
        /// </summary>
        internal void LogResources(IEnumerable<Teacher> teachers, IEnumerable<Equipment> equipments)
        {
            sb.AppendLine("Состояние ресурсов:");
            foreach (var teacher in teachers)
                sb.Append($"{teacher} ");
            sb.AppendLine();

            foreach (var equipment in equipments)
                sb.Append($"{equipment} ");
            
            sb.AppendLine();
            Output();
        }

        /// <summary>
        /// Вывод результатов.
        /// </summary>
        internal void LogResult(int threadsCount, DateTime time)
        {
            sb.AppendLine("Итоги: все студенты сдали работы.");
            sb.AppendLine($"Всего было запущено потоков: {threadsCount}.");
            sb.AppendLine($"Общее время работы: {(DateTime.Now - time).TotalSeconds} секунд.");
            Output();
        }

        //Вывод результатов в соответствии с выбранным методом вывода
        private void Output()
        {
            switch (mode)
            {
                case OutputMode.Console:
                    Console.WriteLine(sb.ToString());
                    break;

                case OutputMode.File:
                    using (StreamWriter sw = File.AppendText(path))
                        sw.WriteLine(sb.ToString());
                    break;

                default:
                    throw new Exception("Неизвестный режим вывода.");
            }

            sb.Clear();
        }
    }
}
