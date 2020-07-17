﻿using PPLR1.Models;
using System.Collections.Generic;
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
            : base(mode, quantDuration, maxCpuBurst, maxThreadPriority, equipments, students, teachers)
        {
            plainType = PlainType.LCFS;
            StartMaxThreads();
        }

        /// <summary>
        /// Начать процесс сдачи предмета. Останавливаем поток на время (quantDuration * RemainingCpuBurst)
        /// </summary>
        protected override void StartPassExam(object examProcess)
        {
            var exam = examProcess as ExamProcess;
            for (int i = 0; i < exam.RemainingCpuBurst(); i++)
                Thread.Sleep(quantDuration);

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
