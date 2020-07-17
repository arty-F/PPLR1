using System;

namespace PPLR2
{
    /// <summary>
    /// Экземпляр карты. Содержит масть и ранг.
    /// </summary>
    internal class Card
    {
        /// <summary>
        /// Ранг карты.
        /// </summary>
        internal CardRank Rank { get; }
        /// <summary>
        /// Масть карты.
        /// </summary>
        internal CardSuit Suit { get; }

        /// <summary>
        /// Создать новый экземпляр карты.
        /// </summary>
        /// <param name="rank">Ранк.</param>
        /// <param name="suit">Масть.</param>
        internal Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        /// <summary>
        /// Создать новый экземпляр карты. При создании будет предпринята попытка парсинга входящих
        /// параметров к соответствующим значениям перечислений <seealso cref="CardRank"/> и
        /// <seealso cref="CardSuit"/>. При неудаче, значение(я) будут сгенерированны случайным образом.
        /// </summary>
        /// <param name="rank">Ранг.</param>
        /// <param name="suit">Масть.</param>
        internal Card(string rank = null, string suit = null)
        {
            Random rng = new Random();

            if (rank != null)
            {
                if (Enum.TryParse(typeof(CardRank), rank, out object r))
                {
                    if (r != null && r is CardRank)
                        Rank = (CardRank)r;
                }
            }
            else
                Rank = (CardRank)rng.Next(0, Enum.GetValues(typeof(CardRank)).Length);

            if (suit != null)
            {
                if (Enum.TryParse(typeof(CardSuit), rank, out object s))
                {
                    if (s != null && s is CardSuit)
                        Suit = (CardSuit)s;
                }
            }
            else
                Suit = (CardSuit)rng.Next(0, Enum.GetValues(typeof(CardSuit)).Length);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
                return false;
            else
            {
                Card c = obj as Card;
                return (Rank == c.Rank) && (Suit == c.Suit);
            }
        }

        public override int GetHashCode()
        {
            var hash = 41;
            hash = hash * 199 + Rank.GetHashCode();
            hash = hash * 199 + Suit.GetHashCode();
            return hash;
        }
    }
}
