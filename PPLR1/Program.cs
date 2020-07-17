namespace PPLR1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XmlBuilder xml = new XmlBuilder();          //Считываем данные из input.xml
            if (xml.PlainType == PlainType.LCFS)
            {
                LCFSplanner planner = new LCFSplanner(OutputMode.Console, xml.QuantTime, xml.MaxCpuBurst,
                    xml.MaxThreadPriority, xml.Equipments, xml.Students, xml.Teachers);
            }
            else
            {
                MLQplanner planner = new MLQplanner(OutputMode.Console, xml.QuantTime, xml.MaxCpuBurst,
                    xml.MaxThreadPriority, xml.Equipments, xml.Students, xml.Teachers);
            }
        }
    }
}
