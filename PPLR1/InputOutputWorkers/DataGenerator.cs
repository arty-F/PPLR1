using PPLR1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PPLR1
{
    /// <summary>
    /// Класс отвечающий за генерацию случайных значений свойств моделей.
    /// </summary>
    internal static class DataGenerator
    {
        private static Random rng = new Random();

        private static List<string> equipments { get; set; } = new List<string> { "Испаритель", "Весы", "Плотнометр" ,
            "Рефрактометр", "Вольтметр" , "Термостат", "Дозатор", "Аккумулятор", "Спектрометр", "Тензиометр"};

        private static List<string> lastNames { get; set; } = new List<string> { "Антонов", "Борисов", "Васильев",
            "Григорьев", "Дмитриев", "Евгеньев", "Зинуров", "Иванов", "Кирилов", "Леонидов", "Михайлов",
            "Николаев", "Олегов", "Петров", "Романов", "Степанов", "Темиров"};

        private static List<string> firstNames { get; set; } = new List<string> { "Антон", "Борис", "Василий",
            "Григорий", "Дмитрий", "Евгений", "Зинур", "Иван", "Кирил", "Леонид", "Михаил",
            "Николай", "Олег", "Петр", "Роман", "Степан", "Темир"};

        private static List<string> patronymics { get; set; } = new List<string> { "Антонович", "Борисович", "Васильевич",
            "Григорьевич", "Дмитриевич", "Евгеньевич", "Зинурович", "Иванович", "Кирилович", "Леонидович", "Михайлович",
            "Николаевич", "Олегович", "Петрович", "Романович", "Степанович", "Темирович"};

        private static List<string> groups { get; set; } = new List<string> { "а.20", "б.20", "в.20", "а.21", "б.21" };

        internal static string GetRngFirstName() => firstNames[rng.Next(0, firstNames.Count)];
        internal static string GetRngLastName() => lastNames[rng.Next(0, lastNames.Count)];
        internal static string GetRngPatronymic() => patronymics[rng.Next(0, patronymics.Count)];
        internal static string GetRngGroup() => groups[rng.Next(0, groups.Count)];
        internal static string GetRngEquip() => equipments[rng.Next(0, equipments.Count)];
        internal static int GetRngNumber(int min, int max) => rng.Next(min, max + 1);

        /// <summary>
        /// Получить сгенерированную случайным образом коллекцию лабораторного оборудования.
        /// </summary>
        internal static IEnumerable<Equipment> GetEquipments(int count = 5, int maxInPack = 4, int minInPack = 2)
        {
            if (count > equipments.Count)
                throw new Exception("Запрошено типов оборудования большее количество, чем есть в наличии.");

            var tempEq = new List<string>(equipments);

            for (int i = 0; i < count; i++)
            {
                string name = tempEq[rng.Next(0, tempEq.Count)];
                tempEq.Remove(name);
                yield return new Equipment(name, GetRngNumber(minInPack, maxInPack));
            }
        }

        /// <summary>
        /// Получить сгенерированное случайным образом лабораторное оборудование.
        /// </summary>
        internal static Equipment GetEquipment(int maxInPack = 4, int minInPack = 2)
        {
            return new Equipment(GetRngEquip(), GetRngNumber(minInPack, maxInPack));
        }

        /// <summary>
        /// Получить сгенерированную случайным образом коллекцию студентов.
        /// </summary>
        internal static IEnumerable<Student> GetStudents(IEnumerable<Equipment> equipment, int maxCpuBurst, int maxThreadPriority, int count = 4)
        {
            for (int i = 0; i < count; i++)
                yield return GetStudent(equipment, maxCpuBurst, maxThreadPriority);
        }

        /// <summary>
        /// Возвращает сгенерированного случайным образом студента.
        /// </summary>
        internal static Student GetStudent(IEnumerable<Equipment> equipment, int maxCpuBurst, int maxThreadPriority)
        {
            if (equipment.Count() < 1)
                throw new Exception("Список оборудования должен содержать хотябы один элемент.");

            //Формируем случайный список оборудования
            List<string> subj = new List<string>(GetRandomItems(equipment.Select(e => e.Name).ToList()));

            if (subj.Count == 0)            //Если ничего не было добавлено, добавляем одну случайную установку
                subj.Add(equipment.ElementAt(rng.Next(0, equipment.Count())).Name);


            return new Student(GetRngLastName(), GetRngFirstName(), GetRngPatronymic(),
                GetRngGroup(), GetRngNumber(1, maxThreadPriority), new Subject(subj, GetRngNumber(1, maxCpuBurst)));
        }

        /// <summary>
        /// Получить сгенерированную случайным образом коллекцию преподавателей.
        /// </summary>
        internal static IEnumerable<Teacher> GetTeachers(int count = 2, int maxStudentsForOneTeacher = 3)
        {
            for (int i = 0; i < count; i++)
                yield return GetTeacher(maxStudentsForOneTeacher);
        }

        /// <summary>
        /// Получить сгенерированного случайным образом преподавателя.
        /// </summary>
        internal static Teacher GetTeacher(int maxStudentsForOneTeacher = 3)
        {
            return new Teacher(GetRngLastName(), GetRngFirstName(), GetRngPatronymic(), GetRngNumber(1, maxStudentsForOneTeacher));
        }

        /// <summary>
        /// Возвращает случайные элементы коллекции
        /// </summary>
        internal static IEnumerable<string> GetRandomItems(List<string> items)
        {
            foreach (var i in items)
                if (rng.Next(0, 2) == 1)
                    yield return i;
        }
    }
}
