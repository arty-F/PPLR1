using PPLR1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PPLR1
{
    /// <summary>
    /// Класс представляющий MLQ, absolute priority способ планирования потоков.
    /// </summary>
    internal class MLQplanner : AbstractPlanner
    {
        private List<List<Student>> studentsQueue = new List<List<Student>>();

        /// <summary>
        /// Создать новый экземпляр класса и запустить процесс планирования потоков.
        /// </summary>
        /// <param name="quantDuration">Продолжительность кванта в мс.</param>
        /// <param name="maxCpuBurst">Максимальное время непрерывного использования процессора в квантах.</param>
        /// <param name="maxThreadPriority">Максимальное значение приоритета потоков.</param>
        /// <param name="equipments">Коллекция лабораторного оборудования.</param>
        /// <param name="students">Коллекция студентов.</param>
        /// <param name="teachers">Коллекция преподавателей.</param>
        internal MLQplanner(OutputMode mode = OutputMode.Console, int quantDuration = 100, int maxCpuBurst = 5, int maxThreadPriority = 100,
                         IEnumerable<Equipment> equipments = null, IEnumerable<Student> students = null, IEnumerable<Teacher> teachers = null)
           : base(mode, quantDuration, maxCpuBurst, maxThreadPriority, equipments, students, teachers)
        {
            //Инициализация необходимого количества списков
            for (int i = 0; i < this.students.Max(s => s.SubjectToPassing.RemainingTime); i++)
                studentsQueue.Add(new List<Student>());

            plainType = PlainType.MLQ;
            StartMaxThreads();
        }

        /// <summary>
        /// Начать процесс сдачи предмета. Останавливаем поток на время quantDuration.
        /// </summary>
        protected override void StartPassExam(object examProcess)
        {
            var exam = examProcess as ExamProcess;
            Thread.Sleep(quantDuration);
            --exam.Student.SubjectToPassing.RemainingTime;

            EndPassExam(exam);
        }

        /// <summary>
        /// Закончить процесс сдачи предмета. Освобождаем ресурсы.
        /// </summary>
        protected override void EndPassExam(object examProcess)
        {
            var exam = examProcess as ExamProcess;
            exam.ReleaseResources(ref students, Thread.CurrentThread.ManagedThreadId);

            NextQueueOrEnd(exam);
            
            StartMaxThreads();
        }

        //Определяет перемещать ли студента в следующую очередь, а также инициализирует списки очередей
        private void NextQueueOrEnd(ExamProcess exam)
        {
            lock (students)
            {
                lock (studentsQueue)
                {
                    if (exam.Student.SubjectToPassing.RemainingTime > 0)            //Если еще осталось CPU burst
                    {
                        if (exam.QueueLevel == queueLevel)                          //Если выполняется все та же очередь                                                                 
                            studentsQueue[queueLevel + 1].Add(exam.Student);        //Добавляем студента в след. очередь
                        else                                                        //Если выполняется уже следующая
                            studentsQueue[queueLevel].Add(exam.Student);            //Добавляем в нее студента
                    }
                    //Если текущая очередь пустая и эта очередь не последняя
                    if (students.Count == 0 && studentsQueue.ElementAtOrDefault(queueLevel + 1) != null)
                        students = studentsQueue[++queueLevel];                     //Переходим к следующей
                }
            }
        }
    }
}
