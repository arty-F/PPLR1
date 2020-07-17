using System;
using System.Collections.Generic;
using System.Linq;

namespace PPLR2
{
    /// <summary>
    /// Инкапсулирует функционал по работе с коллекцией карт.
    /// </summary>
    internal class CardController
    {
        /// <summary>
        /// Коллекция имеющихся карт.
        /// </summary>
        internal List<Card> Cards { get; }
        /// <summary>
        /// Коллекция недостающих до полной колоды карт.
        /// </summary>
        internal List<Card> CardsFullCollection { get; set; } = new List<Card>();
        private int currentCardIndex = 0;                                           //текущий индекс считывания карт
        
        /// <summary>
        /// Создать новый экземпляр класса. Необходима коллекция карт.
        /// </summary>
        /// <param name="cards">Колекция карт.</param>
        public CardController(IEnumerable<Card> cards)
        {
            Cards = cards.ToList();

            foreach (var suit in Enum.GetValues(typeof(CardSuit)))                   //Генерация полной коллекции карт
                foreach (var rank in Enum.GetValues(typeof(CardRank)))
                    CardsFullCollection.Add(new Card((CardRank)rank, (CardSuit)suit));
        }

        /// <summary>
        /// Сообщает, имеются ли в коллекции не проанализированные карты.
        /// </summary>
        internal bool HasCards() => currentCardIndex < Cards.Count;

        /// <summary>
        /// Получить следующую карту для анализа.
        /// </summary>
        internal Card GetNextCard() 
        {
            if (HasCards())
                return Cards.ElementAt(currentCardIndex++);
            else
                return null;
        }

        /// <summary>
        /// Удалить проанализированную карту из полной коллекции карт.
        /// </summary>
        /// <param name="card">Карта, которую необходимо удалить из коллекции.</param>
        internal void RemoveFromFullCollection(Card card)
        {
            lock (CardsFullCollection)
            {
                if (CardsFullCollection.Contains(card))
                    CardsFullCollection.Remove(card);
            }
        }
    }
}
