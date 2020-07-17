namespace PPLR2
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlBuilder xml = new XmlBuilder();
            switch (xml.PlainType)
            {
                case PlainType.ThreadArraySemaphorePetri:
                    new SemaphorePetriPlanner(OutputMode.Console, xml.Cards, xml.ThreadsCount, xml.Pause);
                    break;
                case PlainType.ThreadPool:
                    new ThreadPoolPlanner(OutputMode.Console, xml.Cards, xml.ThreadsCount, xml.Pause);
                    break;
                case PlainType.ThreadPoolPetri:
                    new ThreadPoolPetriPlanner(OutputMode.Console, xml.Cards, xml.ThreadsCount, xml.Pause);
                    break;
                default:
                    new SemaphorePlanner(OutputMode.Console, xml.Cards, xml.ThreadsCount, xml.Pause);
                    break;
            }
        }
    }
}
