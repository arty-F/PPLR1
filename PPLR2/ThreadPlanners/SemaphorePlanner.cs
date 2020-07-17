using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PPLR2
{
    /// <summary>
    /// Планировщик потоков, использующий конструкцию <seealso cref="Semaphore"/>.
    /// </summary>
    internal class SemaphorePlanner : AbstractPlanner
    {
        private Semaphore sem;
        private List<Thread> threads { get; set; } = new List<Thread>();

        /// <summary>
        /// Создать новый экземпляр класса планировщика, использующего <seealso cref="Semaphore"/>.
        /// </summary>
        /// <param name="mode">Способ вывода результатов.</param>
        /// <param name="cards">Коллекция карт для анализа.</param>
        /// <param name="threadCount">Количество потоков.</param>
        /// <param name="pause">Пауза при обработке карты.</param>
        internal SemaphorePlanner(OutputMode mode, IEnumerable<Card> cards, int threadCount, int pause) :
            base(mode, PlainType.ThreadArraySemaphore, cards, threadCount, pause)
        {
            sem = new Semaphore(0, threadCount);
            StartMaxThreads();
        }

        protected override void StartMaxThreads()
        {
            sem.Release(threadCount);                           //Освобождаем семафор

            lock (threads)
                lock (cardController)
                    while (cardController.HasCards())           //Запуск анализа каждой карты в отдельном потоке
                    {
                        Thread t = new Thread(Analysis);
                        threads.Add(t);
                        t.Start(cardController.GetNextCard());
                    }
        }

        protected override void Analysis(object card)
        {
            sem.WaitOne();
            Thread.Sleep(pause);
            lock (cardController)
                cardController.RemoveFromFullCollection(card as Card);
            
            lock (logger)
            {
                logger.LogCard(card as Card, Thread.CurrentThread.ManagedThreadId);

                lock (threads)
                    //Если все потоки, кроме текущего, уже остановлены, то выводим результаты
                    if (threads.Where(t => t.ManagedThreadId != Thread.CurrentThread.ManagedThreadId).All(t => t.ThreadState == ThreadState.Stopped))
                        GetResult();
            }
            sem.Release();
        }
    }
}
