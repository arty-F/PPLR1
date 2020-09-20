using System;
using System.Collections.Generic;

namespace PPLR2
{
    /// <summary>
    /// Абстрактный класс планирования потоков. Содержит общий функционал.
    /// </summary>
    internal abstract class AbstractPlanner
    {
        protected PlainType plainType { get; }
        protected int threadCount { get; }
        protected int pause { get; }
        protected Logger logger { get; }
        protected CardController cardController { get; }
        private DateTime startTime { get; }
        private double linearTime;

        internal AbstractPlanner(OutputMode mode, PlainType plainType, IEnumerable<Card> cards, int threadCount, int pause)
        {
            cardController = new CardController(cards);
            this.threadCount = threadCount;
            this.pause = pause;
            linearTime = (double)(pause * cardController.Cards.Count) / 1000;
            this.plainType = plainType;
            logger = new Logger(mode);
            logger.LogInfo(this.plainType, cardController.Cards.Count, threadCount, pause);
            startTime = DateTime.Now;
        }

        /// <summary>
        /// Вывод результатов работы планировщика.
        /// </summary>
        protected void GetResult()
        {
            var totalSec = (DateTime.Now - startTime).TotalSeconds;
            logger.LogResult(cardController.CardsFullCollection, linearTime, totalSec);
        }

        /// <summary>
        /// Запуск максимально возможного килочества потоков
        /// </summary>
        protected abstract void StartMaxThreads();
        /// <summary>
        /// Процесс анализа карты.
        /// </summary>
        /// <param name="card"></param>
        protected abstract void Analysis(object card);
    }
}
