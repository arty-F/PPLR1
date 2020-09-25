using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PPLR2
{
    internal class ThreadPoolPlanner : AbstractPlanner
    {
        private Semaphore sem;

        public ThreadPoolPlanner(OutputMode mode, IEnumerable<Card> cards, int threadCount, int pause) :
            base(mode, PlainType.ThreadPool, cards, threadCount < Environment.ProcessorCount ? Environment.ProcessorCount : threadCount, pause)
        {
            ThreadPool.SetMaxThreads(threadCount, 1);
            ThreadPool.SetMinThreads(threadCount, 1);
            sem = new Semaphore(threadCount, threadCount);

            StartMaxThreads();
        }

        protected override void StartMaxThreads()
        {
            while (cardController.HasCards())                           //Запуск задач
                ThreadPool.QueueUserWorkItem(Analysis, cardController.GetNextCard());
        }

        protected override void Analysis(object card)
        {
            if (startTime == DateTime.MinValue)                         //Установка времени начала работы алгоритма
                startTime = DateTime.Now;
            Stopwatch stopWatch = new Stopwatch();

            sem.WaitOne();
            stopWatch.Start();                                          //Запуск таймера
            cardController.RemoveFromFullCollection(card as Card);      //Обработка карты
            while (stopWatch.ElapsedMilliseconds < pause) { }           //Пауза
            sem.Release();

            logger.LogCard(card as Card, Thread.CurrentThread.ManagedThreadId);
            if (--cardsLeft == 0)
                GetResult();
        }
    }
}
