using PPLR1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PPLR1
{
    /// <summary>
    /// Абстрактный класс планирования потоков. Содержит общий функционал.
    /// </summary>
    internal abstract class AbstractPlanner
    {
        protected List<Student> students = new List<Student>();         //Очередь из студентов по приоритету
        protected List<Equipment> equipments = new List<Equipment>();
        protected List<Teacher> teachers = new List<Teacher>();
        protected List<Thread> threads = new List<Thread>();
        protected volatile int quantDuration;
        protected volatile int queueLevel;
        protected DateTime time = DateTime.Now;
        protected Logger logger;
        protected PlainType plainType;

        internal AbstractPlanner(OutputMode mode = OutputMode.Console, int quantDuration = 100, int maxCpuBurst = 50, int maxThreadPriority = 100,
                          IEnumerable<Equipment> equipments = null, IEnumerable<Student> students = null, IEnumerable<Teacher> teachers = null)
        {
            logger = new Logger(mode);
            this.quantDuration = quantDuration;
            this.equipments = equipments?.ToList() ?? DataGenerator.GetEquipments().ToList();
            this.teachers = teachers?.ToList() ?? DataGenerator.GetTeachers().ToList();
            if (students == null)
                students = DataGenerator.GetStudents(this.equipments, maxCpuBurst, maxThreadPriority).OrderBy(s => s.Priority);

            this.students = students.OrderBy(s => s.Priority).ToList();              
            logger.LogStudents(this.students);                          //Вывод отсортированного списка студентов                         
            logger.LogResources(this.teachers, this.equipments);        //Вывод списка ресурсов
        }

        /// <summary>
        /// Проверка необходимых ресурсов для запуска указанного потока
        /// </summary>
        /// <param name="student">Студент для запуска</param>
        protected bool CheckResources(Student student)
        {
            foreach (var eqName in student.SubjectToPassing.EquipmentNames)
                if (!equipments.Where(e => e.Name == eqName && e.Count > 0).Any())
                    return false;

            if (!teachers.Where(t => t.NumberOfStudents > 0).Any())
                return false;

            return true;
        }

        //Возвращает студента с максимальным приоритетом
        protected Student HigherPriorityStudent()
        {
            return students.Where(s => s.Priority == students.Min(s => s.Priority)).First();
        }

        /// <summary>
        /// Запуск максимально возможного количества потоков
        /// </summary>
        /// <param name="students">Очередь студентов</param>
        protected void StartMaxThreads()
        {
            lock (students)
            {
                if (students.Count > 0)
                {
                    while (students.Count > 0 && CheckResources(HigherPriorityStudent()))
                    {
                        ExamProcess exam = new ExamProcess(ref students, teachers.Where(t => t.NumberOfStudents > 0).FirstOrDefault(), equipments, logger, plainType, queueLevel);
                        Thread t = new Thread(StartPassExam);
                        threads.Add(t);
                        t.Start(exam);
                    }

                    if (threads.Where(t => t.ManagedThreadId != Thread.CurrentThread.ManagedThreadId).All(t => t.ThreadState == ThreadState.Stopped))
                        throw new Exception("Deadlock.");
                }
                else if (threads.Where(t => t.ManagedThreadId != Thread.CurrentThread.ManagedThreadId).All(t => t.ThreadState == ThreadState.Stopped))
                {
                    logger.LogResult(threads.Count, time);
                }
            }
        }

        protected abstract void StartPassExam(object examProcess);
        protected abstract void EndPassExam(object examProcess);
    }
}
