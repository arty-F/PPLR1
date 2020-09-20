using System;
using System.Collections.Generic;
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

        // Возвращает количество работающих потоков в пуле
        private int GetBusyThreadsCount()
        {
            ThreadPool.GetAvailableThreads(out int work, out int _);
            ThreadPool.GetMaxThreads(out int all, out int _);
            return all - work;
        }

        protected override void StartMaxThreads()
        {
            lock (cardController)
                while (cardController.HasCards())   //Запуск задач
                    ThreadPool.QueueUserWorkItem(Analysis, cardController.GetNextCard());

            while (true)                            //Ожидание завершения работы пулом потоков
            {
                Thread.Sleep(10);
                lock (cardController)
                    if (!cardController.HasCards() && GetBusyThreadsCount() == 0)
                        break;
            }

            lock (logger)                           //Вывод результатов
                GetResult();
        }

        protected override void Analysis(object card)
        {
            sem.WaitOne();
            Thread.Sleep(pause);
            sem.Release();

            lock (cardController)
                cardController.RemoveFromFullCollection(card as Card);
            
            lock (logger)
                logger.LogCard(card as Card, Thread.CurrentThread.ManagedThreadId);
        }
    }
}
