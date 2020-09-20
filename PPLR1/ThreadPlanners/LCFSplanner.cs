using PPLR1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PPLR1
{
    /// <summary>
    /// Класс представляющий LCFS, nonpremetive способ планирования потоков.
    /// </summary>
    internal class LCFSplanner : AbstractPlanner
    {
        /// <summary>
        /// Создать новый экземпляр класса и запустить процесс планирования потоков.
        /// </summary>
        /// <param name="quantDuration">Продолжительность кванта в мс.</param>
        /// <param name="maxCpuBurst">Максимальное время непрерывного использования процессора в квантах.</param>
        /// <param name="maxThreadPriority">Максимальное значение приоритета потоков.</param>
        /// <param name="equipments">Коллекция лабораторного оборудования.</param>
        /// <param name="students">Коллекция студентов.</param>
        /// <param name="teachers">Коллекция преподавателей.</param>
        internal LCFSplanner(OutputMode mode = OutputMode.Console, int quantDuration = 100, int maxCpuBurst = 50, int maxThreadPriority = 100,
                          IEnumerable<Equipment> equipments = null, IEnumerable<Student> students = null, IEnumerable<Teacher> teachers = null)
            : base(mode, quantDuration, maxCpuBurst, maxThreadPriority, equipments, LCFSsort(students), teachers)
        {
            plainType = PlainType.LCFS;
            new Thread(ShowStatus).Start();
            StartMaxThreads();
        }

        //Перед отправкой работы выяснилось, что не реализована часть nonpremetive для LCFS планировщика, когда коллекция студентов задается вручную.
        //Студенты, с одинаковым приоритетом, находящиеся в начале очереди, должны быть обработаны в последнюю очередь.
        //Данный статический метод переворачивает "Reverse()" студентов с одинаковым приоритетом в пределах одной очереди.
        private static IEnumerable<Student> LCFSsort(IEnumerable<Student> s)
        {
            if (s == null)
                return null;

            var result = s.ToList();
            var uniqPriority = result.Select(s => s.Priority).Distinct().ToList();          //Получаем студентов с уникальными приоритетами

            foreach (var priority in uniqPriority)
            {
                var studWithSamePriority = result.Where(stud => stud.Priority == priority).ToList();
                if (studWithSamePriority.Count() > 1)                                       //Если студентов с одинаковым приоритетом больше 1
                {
                    result.RemoveAll(s => studWithSamePriority.Contains(s));                //Удаляем из итоговой коллекции
                    studWithSamePriority.Reverse();                                         //Переворачиваем повторяющихся студентов
                    result.AddRange(studWithSamePriority);                                  //Добавляем обратно
                }                                                                           //Неважно, что добавляем в конец очереди, т.к. потом коллекция подвергнется сортировке
            }
            return result;
        }

        /// <summary>
        /// Начать процесс сдачи предмета. Останавливаем поток на время (quantDuration * RemainingCpuBurst)
        /// </summary>
        protected override void StartPassExam(object examProcess)
        {
            var exam = examProcess as ExamProcess;
            while (exam.Student.SubjectToPassing.RemainingTime > 0)
            {
                Thread.Sleep(quantDuration);
                --exam.Student.SubjectToPassing.RemainingTime;
            }     
                
            EndPassExam(exam);
        }

        /// <summary>
        /// Закончить процесс сдачи предмета. Освобождаем ресурсы.
        /// </summary>
        protected override void EndPassExam(object examProcess)
        {
            var exam = examProcess as ExamProcess;
            exam.ReleaseResources(ref students, Thread.CurrentThread.ManagedThreadId);

            StartMaxThreads();
        }
    }
}

