using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PPLR2
{
    /// <summary>
    /// Содержит методы для записи результатов работы программы.
    /// </summary>
    internal class Logger
    {
        private OutputMode mode;
        private StringBuilder sb = new StringBuilder();
        private string path = "";
        private PlainType lastUsingType;

        /// <summary>
        /// Создать экземпляр логгера, необходимо передать режим вывода <seealso cref="OutputMode"/>.
        /// </summary>
        /// <param name="mode">Режим вывода.</param>
        internal Logger(OutputMode mode)
        {
            this.mode = mode;
            if (mode == OutputMode.File)
            {
                path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output.txt");
                File.WriteAllText(path, string.Empty);
            }
        }

        /// <summary>
        /// Вывод входных параметров.
        /// </summary>
        /// <param name="type">Тип планирования.</param>
        /// <param name="cardsCount">Количество карт в коллекции.</param>
        /// <param name="threadCount">Количество потоков.</param>
        /// <param name="pause">Пауза.</param>
        internal void LogInfo(PlainType type, int cardsCount, int threadCount, int pause)
        {
            lock (sb)
            {
                sb.AppendLine($"Анализ коллекции из {cardsCount} карт, при ограниченнии количества одновременно работающих потоков {threadCount}, с задержкой {pause} мс.");
                sb.Append("Для планировки используется ");
                sb.AppendLine(type switch
                {
                    PlainType.ThreadArraySemaphore => "массив потоков, сфемафор.",
                    PlainType.ThreadArraySemaphorePetri => "массив потоков, сеть Петри моделирующая семафор.",
                    PlainType.ThreadPool => "системный пул потоков.",
                    PlainType.ThreadPoolPetri => "пул потоков, сеть Петри.",
                    _ => throw new Exception("Неверный тип планирования.")
                });
                Output();
            }

            lastUsingType = type;
        }

        /// <summary>
        /// Вывод информации о проанализированной карте.
        /// </summary>
        /// <param name="card">Карта.</param>
        /// <param name="threadId">Ид потока.</param>
        internal void LogCard(Card card, int threadId)
        {
            lock (sb)
            {
                sb.Append($"В потоке :{threadId} была считана карта <{card.Rank}, {card.Suit}>.");
                Output();
            }
        }

        /// <summary>
        /// Вывод результатов работы.
        /// </summary>
        /// <param name="cards">Коллекция нехватающих карт.</param>
        /// <param name="linearTime">Время линейной обработки.</param>
        /// <param name="parallelTime">Время параллельной обработки.</param>
        internal void LogResult(IEnumerable<Card> cards, double linearTime, double parallelTime)
        {
            lock (sb)
            {
                sb.Append(Environment.NewLine + $"Обработка всех карт завершена c помощью ");
                sb.AppendLine(lastUsingType switch
                {
                    PlainType.ThreadArraySemaphore => "массива потоков, сфемафора.",
                    PlainType.ThreadArraySemaphorePetri => "массива потоков и сети Петри моделирующей семафор.",
                    PlainType.ThreadPool => "системного пула потоков.",
                    PlainType.ThreadPoolPetri => "пула потоков моделируемого сетью Петри.",
                    _ => throw new Exception("Неверный тип планирования.")
                });
                sb.AppendLine($"Длительность обработки: {parallelTime} секунд.");
                sb.AppendLine($"Время линейной обработки заняло бы: {linearTime} секунд.");

                sb.AppendLine($"В данной колоде, для полной коллекции, отсутствуют следующие карты:");
                for (int i = 0; i < cards.Count(); i++)
                    sb.AppendLine($"{i + 1}. {cards.ElementAtOrDefault(i).Rank}, {cards.ElementAtOrDefault(i).Suit}.");

                Output();
            }
        }

        //Вывод результатов в соответствии с выбранным методом вывода
        private void Output()
        {
            switch (mode)
            {
                case OutputMode.Console:
                    Console.WriteLine(sb.ToString());
                    break;

                case OutputMode.File:
                    using (StreamWriter sw = File.AppendText(path))
                        sw.WriteLine(sb.ToString());
                    break;

                default:
                    throw new Exception("Неизвестный режим вывода.");
            }

            sb.Clear();
        }
    }
}
