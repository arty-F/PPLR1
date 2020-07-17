using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace PPLR2
{
    /// <summary>
    /// Считывает данные из xml файла.
    /// </summary>
    internal class XmlBuilder
    {
        internal PlainType PlainType { get; } = PlainType.ThreadArraySemaphore;
        internal int ArrayLength { get; } = 25;
        internal int ThreadsCount { get; }
        internal int Pause { get; }
        internal List<Card> Cards { get; } = new List<Card>();

        /// <summary>
        /// Создать новый экземпляр класса и запустить процесс считывания данных из input.xml
        /// </summary>
        internal XmlBuilder()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "input.xml");
            XDocument xdoc = XDocument.Load(path);

            //Метод планирования
            var pa = xdoc.Root.Element("pa")?.Value;
            if (pa != null)
                PlainType = pa switch
                {
                    "tasemp" => PlainType.ThreadArraySemaphorePetri,
                    "tp" => PlainType.ThreadPool,
                    "tpp" => PlainType.ThreadPoolPetri,
                    _ => PlainType.ThreadArraySemaphore
                };

            //Размер массива
            if (int.TryParse(xdoc.Root.Element("n")?.Value, out int n))
                if (n > 0)
                    ArrayLength = n;

            //Размер массива
            if (int.TryParse(xdoc.Root.Element("m")?.Value, out int m))
                if (m > 0 && m <= n)
                    ThreadsCount = m;
                else if (Environment.ProcessorCount < n)
                    ThreadsCount = Environment.ProcessorCount;
                else
                    ThreadsCount = n;

            //Пауза
            if (int.TryParse(xdoc.Root.Element("pt")?.Value, out int pt))
                if (pt > 0)
                    Pause = pt;

            //Список карт
            var cards = xdoc.Root.Element("cards").Elements("card");
            for (int i = 0; i < n; i++)
            {
                if (cards.ElementAtOrDefault(i) != null)
                {
                    var rank = cards.ElementAt(i).Attribute("rank")?.Value;
                    var suit = cards.ElementAt(i).Attribute("rank")?.Value;

                    Cards.Add(new Card(rank, suit));
                }
                else
                    Cards.Add(new Card());
            }
        }
    }
}
