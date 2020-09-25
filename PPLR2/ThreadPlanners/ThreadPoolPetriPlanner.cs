using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PPLR2
{
    internal class ThreadPoolPetriPlanner : AbstractPlanner
    {
        private object pool = new object();
        private volatile int busyThreadsCount = 0;

        public ThreadPoolPetriPlanner(OutputMode mode, IEnumerable<Card> cards, int threadCount, int pause) :
            base(mode, PlainType.ThreadPoolPetri, cards, threadCount, pause)
        {
            ThreadPool.SetMaxThreads(threadCount, 1);
            ThreadPool.SetMinThreads(threadCount, 1);

            StartMaxThreads();
        }

        //Запуск максимально возможного количества потоков из пула
        private void RunPool()
        {
            lock (pool)
                while (cardController.HasCards() && busyThreadsCount < threadCount)
                {
                    ++busyThreadsCount;
                    ThreadPool.QueueUserWorkItem(Analysis, cardController.GetNextCard());
                }
        }

        private void EndWork()
        {
            lock (pool)
                --busyThreadsCount;
        }

        protected override void StartMaxThreads()
        {
            RunPool();
        }

        protected override void Analysis(object card)
        {
            if (startTime == DateTime.MinValue)                         //Установка времени начала работы алгоритма
                startTime = DateTime.Now;
            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();                                          //Запуск таймера
            cardController.RemoveFromFullCollection(card as Card);      //Обработка карты
            while (stopWatch.ElapsedMilliseconds < pause) { }           //Пауза

            EndWork();
            RunPool();

            logger.LogCard(card as Card, Thread.CurrentThread.ManagedThreadId);
            if (--cardsLeft == 0)
                GetResult();
        }
    }
}
