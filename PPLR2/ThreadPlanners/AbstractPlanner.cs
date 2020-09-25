using System;
using System.Collections.Generic;
using System.Linq;

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
        protected DateTime startTime { get; set; } = DateTime.MinValue;
        private double linearTime;
        protected int cardsLeft { get; set; }

        internal AbstractPlanner(OutputMode mode, PlainType plainType, IEnumerable<Card> cards, int threadCount, int pause)
        {
            cardController = new CardController(cards);
            cardsLeft = cards.Count();
            this.threadCount = threadCount;
            this.pause = pause;
            linearTime = (double)(pause * cardController.Cards.Count) / 1000;
            this.plainType = plainType;
            logger = new Logger(mode);
            logger.LogInfo(this.plainType, cardController.Cards.Count, threadCount, pause);
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
