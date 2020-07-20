using System.Collections.Generic;
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
                lock (cardController)
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

            while (true)
            {
                Thread.Sleep(100);              //Ожидание завершения работы пулом потоков
                lock (pool)
                    lock (cardController)
                        if (!cardController.HasCards() && busyThreadsCount == 0)
                            break;
            }

            lock (logger)                       //Вывод результатов
                GetResult();
        }

        protected override void Analysis(object card)
        {
            Thread.Sleep(pause);
            lock (cardController)
                cardController.RemoveFromFullCollection(card as Card);
            
            lock (logger)
                logger.LogCard(card as Card, Thread.CurrentThread.ManagedThreadId);

            EndWork();
            
            RunPool();
        }
    }
}
