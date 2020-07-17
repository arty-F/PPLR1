using PPLR1.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace PPLR1
{
    /// <summary>
    /// Считывает данные из xml файла.
    /// </summary>
    internal class XmlBuilder
    {
        internal PlainType PlainType { get; } = PlainType.LCFS;
        internal int QuantTime { get; } = 100;
        internal int MaxCpuBurst { get; } = 10;
        internal int MaxThreadPriority { get; } = 100;

        internal int TeachersCount { get; } = 3;
        internal int MaxStudentsCountForOneTeacher { get; } = 3;
        internal List<Teacher> Teachers { get; } = new List<Teacher>();

        internal int EquipmentsCount { get; } = 5;
        internal int EquipmentInStock { get; } = 3;
        internal List<Equipment> Equipments { get; } = new List<Equipment>();

        internal int StudentsCount { get; } = 8;
        internal int StudentMaxEquip { get; }
        internal List<Student> Students { get; } = new List<Student>();

        /// <summary>
        /// Создать новый экземпляр класса и запустить процесс считывания данных из input.xml
        /// </summary>
        internal XmlBuilder()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "input.xml");
            XDocument xdoc = XDocument.Load(path);

            //Метод планирования
            if (xdoc.Root.Element("pa")?.Value != null && xdoc.Root.Element("pa")?.Value == "mlq")
                PlainType = PlainType.MLQ;

            //Время кванта
            if (int.TryParse(xdoc.Root.Element("qt")?.Value, out int qt))
                if (qt > 0)
                    QuantTime = qt;

            //Максимальное время CPU burst
            if (int.TryParse(xdoc.Root.Element("maxt")?.Value, out int maxt))
                if (maxt>0)
                    MaxCpuBurst = maxt;

            //Максимальный приоритет потока
            if (int.TryParse(xdoc.Root.Element("maxp")?.Value, out int maxp))
                if (maxp > 0)
                    MaxThreadPriority = maxp;

            //Количество преподавателей
            if (int.TryParse(xdoc.Root.Element("nt")?.Value, out int nt))
                if (nt > 0)
                    TeachersCount = nt;

            //Количество студентов у которых преподаватель может принимать зачет одновременно
            if (int.TryParse(xdoc.Root.Element("tmax")?.Value, out int tmax))
                if (nt > 0)
                    MaxStudentsCountForOneTeacher = tmax;

            //Список преподавателей
            var t = xdoc.Root.Element("teachers")?.Elements("teacher");
            if (t == null || t.Count() == 0)
            {
                Teachers = DataGenerator.GetTeachers(TeachersCount, MaxStudentsCountForOneTeacher).ToList();
            }
            else
            {
                for (int i = 0; i < TeachersCount; i++)
                {
                    if (t.ElementAtOrDefault(i) != null)
                    {
                        string lname = t.ElementAt(i).Attribute("lname")?.Value ?? DataGenerator.GetRngLastName();
                        string fname = t.ElementAt(i).Attribute("fname")?.Value ?? DataGenerator.GetRngFirstName();
                        string patr = t.ElementAt(i).Attribute("patr")?.Value ?? DataGenerator.GetRngPatronymic();
                        int studCount = MaxStudentsCountForOneTeacher;

                        if (int.TryParse(t.ElementAt(i).Attribute("stud")?.Value, out int stud))
                            if (stud > 0)
                                studCount = stud;

                        Teachers.Add(new Teacher(lname, fname, patr, studCount));
                    }
                    else
                        Teachers.Add(DataGenerator.GetTeacher(MaxStudentsCountForOneTeacher));
                }
            }
            
            //Количество оборудования
            if (int.TryParse(xdoc.Root.Element("ne")?.Value, out int ne))
                if (ne > 0)
                    EquipmentsCount = ne;

            //Максимальное количество оборудования каждого типа
            if (int.TryParse(xdoc.Root.Element("emax")?.Value, out int emax))
                if (emax > 0)
                    EquipmentInStock = emax;

            //Список оборудования
            var e = xdoc.Root.Element("equipments")?.Elements("equipment");
            if (e == null || e.Count() == 0)
            {
                Equipments = DataGenerator.GetEquipments(EquipmentsCount, EquipmentInStock, 1).ToList();
            }
            else
            {
                for (int i = 0; i < EquipmentsCount; i++)
                {
                    if (e.ElementAtOrDefault(i) != null)
                    {
                        string name = e.ElementAt(i).Attribute("name")?.Value ?? DataGenerator.GetRngEquip();
                        int stock = EquipmentInStock;

                        if (int.TryParse(e.ElementAt(i).Attribute("count")?.Value, out int count))
                            if (count > 0)
                                stock = count;

                        Equipments.Add(new Equipment(name, stock));
                    }
                    else
                        Equipments.Add(DataGenerator.GetEquipment(EquipmentInStock, 1));
                }
            }

            //Количество студентов
            if (int.TryParse(xdoc.Root.Element("ns")?.Value, out int ns))
                if (ns > 0)
                    StudentsCount = ns;

            //Максимальное количество оборудования, которое нужно студенту для сдачи
            StudentMaxEquip = EquipmentsCount;
            if (int.TryParse(xdoc.Root.Element("semax")?.Value, out int semax))
                if (semax > 0 && semax < EquipmentsCount)
                    StudentMaxEquip = semax;

            //Список студентов
            var s = xdoc.Root.Element("students")?.Elements("student");
            if (s == null || s.Count() == 0)
            {
                Students = DataGenerator.GetStudents(Equipments, MaxCpuBurst, MaxThreadPriority, StudentsCount).ToList();
            }
            else
            {
                for (int i = 0; i < StudentsCount; i++)
                {
                    if (s.ElementAtOrDefault(i) != null)
                    {
                        string lname = s.ElementAt(i).Attribute("lname")?.Value ?? DataGenerator.GetRngLastName();
                        string fname = s.ElementAt(i).Attribute("fname")?.Value ?? DataGenerator.GetRngFirstName();
                        string patr = s.ElementAt(i).Attribute("patr")?.Value ?? DataGenerator.GetRngPatronymic();
                        string group = s.ElementAt(i).Attribute("group")?.Value ?? DataGenerator.GetRngGroup();

                        int prior = DataGenerator.GetRngNumber(1, MaxThreadPriority);
                        if (int.TryParse(s.ElementAt(i).Attribute("prior")?.Value, out int p))
                            if (p > 0 && p <= MaxThreadPriority)
                                prior = p;

                        int eqcount = DataGenerator.GetRngNumber(1, EquipmentsCount);
                        if (int.TryParse(s.ElementAt(i).Attribute("eqcount")?.Value, out int ec))
                            if (ec > 0 && ec <= EquipmentsCount)
                                eqcount = ec;

                        int cpuburst = DataGenerator.GetRngNumber(1, MaxCpuBurst);
                        if (int.TryParse(s.ElementAt(i).Attribute("cpuburst")?.Value, out int cb))
                            if (cb > 0 && cb <= MaxCpuBurst)
                                cpuburst = cb;

                        //Список предметов студента для сдачи
                        List<string> eqNames = new List<string>();
                        var personalEquipment = s.ElementAt(i).Element("equipments")?.Elements("equipment");
                        if (personalEquipment == null || personalEquipment.Count() == 0)
                            eqNames = DataGenerator.GetRandomItems(Equipments.Select(e => e.Name).ToList()).ToList();
                        else
                            foreach (var equip in personalEquipment)
                                eqNames.Add(equip.Attribute("name")?.Value ?? Equipments.First().Name);

                        Subject subj = new Subject(eqNames, cpuburst);
                        Student st = new Student(lname, fname, patr, group, prior, subj);
                        Students.Add(st);
                    }
                    else
                    {
                        Students.Add(DataGenerator.GetStudent(Equipments, MaxCpuBurst, MaxThreadPriority));
                    }
                }
            }
        }
    }
}
