using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PPLR2
{
    /// <summary>
    /// Планировщик потоков, использующий сеть Петри, иммитирующую семфор./>.
    /// </summary>
    internal class SemaphorePetriPlanner : AbstractPlanner
    {
        private readonly object sem = new object();
        private volatile int semRemaining;
        private List<Thread> threads { get; set; } = new List<Thread>();

        /// <summary>
        /// Создать новый экземпляр класса планировщика, использующего сеть Петри, которая имитирует семафор.
        /// </summary>
        /// <param name="mode">Способ вывода результатов.</param>
        /// <param name="cards">Коллекция карт для анализа.</param>
        /// <param name="threadCount">Количество потоков.</param>
        /// <param name="pause">Пауза при обработке карты.</param>
        public SemaphorePetriPlanner(OutputMode mode, IEnumerable<Card> cards, int threadCount, int pause) :
            base(mode, PlainType.ThreadArraySemaphorePetri, cards, threadCount, pause)
        {
            semRemaining = threadCount;
            StartMaxThreads();
        }

        protected override void StartMaxThreads()
        {
            lock (threads)
                lock (cardController)
                    while (cardController.HasCards())           //Запуск анализа каждой карты в отдельном потоке
                    {
                        Thread t = new Thread(Analysis);
                        threads.Add(t);
                        t.Start(cardController.GetNextCard());
                    }
        }

        //Имитация ожидания доступа к семафору
        private void SemaphoreWaitOne()
        {
            while (true)
            {
                lock (sem)
                    if (semRemaining > 0)
                    {
                        --semRemaining;
                        break;
                    }
                Thread.Sleep(1);
            }
        }

        //Имитация высвобождения семафора
        private void SemaphoreReleaseOne()
        {
            lock (sem)
                ++semRemaining;
        }

        protected override void Analysis(object card)
        {
            SemaphoreWaitOne();
            Thread.Sleep(pause);
            SemaphoreReleaseOne();

            lock (cardController)
            {
                cardController.RemoveFromFullCollection(card as Card);
                logger.LogCard(card as Card, Thread.CurrentThread.ManagedThreadId);
                //Если все потоки, кроме текущего, уже остановлены, то выводим результаты
                if (threads.Where(t => t.ManagedThreadId != Thread.CurrentThread.ManagedThreadId).All(t => t.ThreadState == ThreadState.Stopped))
                    GetResult();
            }
        }
    }
}

