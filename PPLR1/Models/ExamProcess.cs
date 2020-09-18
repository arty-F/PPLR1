using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace PPLR1.Models
{
    /// <summary>
    /// Процесс сдачи предмета.
    /// </summary>
    internal class ExamProcess
    {
        internal Student Student { get; private set; }
        internal Teacher Teacher { get; }
        internal List<Equipment> Equipments { get; }
        internal int QueueLevel { get; }
        private Logger logger;
        private PlainType plainType;

        internal ExamProcess(ref List<Student> students, Teacher teacher, IEnumerable<Equipment> equipments, Logger logger, PlainType plainType, int queueLevel = 0)
        {
            Teacher = teacher;
            Equipments = equipments.ToList();
            QueueLevel = queueLevel;
            this.logger = logger;
            this.plainType = plainType;

            TakeResources(ref students);
        }

        /// <summary>
        /// Возвращает значение оставшегося времени CPU Burst в квантах.
        /// </summary>
        internal int RemainingCpuBurst() => Student.SubjectToPassing.RemainingTime;

        /// <summary>
        /// Забрать все необходимые для сдачи ресурсы.
        /// </summary>
        internal void TakeResources(ref List<Student> students)
        {
            lock (students)
            {
                var maxPriority = students.Min(s => s.Priority);
                Student = students.Where(s => s.Priority == maxPriority).First();
                students.Remove(Student);
                Student.CurrentState = CurrentThreadState.R;

                foreach (var eqName in Student.SubjectToPassing.EquipmentNames)
                {
                    var eq = Equipments.Where(e => e.Name == eqName && e.Count > 0).FirstOrDefault();
                    if (eq == null)
                        throw new Exception($"Нет свободного оборудования типа <{eqName}>.");
                    --eq.Count;
                }

                if (Teacher.NumberOfStudents < 1)
                    throw new Exception($"Преподаватель <{Teacher}> уже занят.");
                --Teacher.NumberOfStudents;

                //logger.LogTakenResources(this, plainType, QueueLevel);
            }
        }

        /// <summary>
        /// Освободить все занятые ресурсы.
        /// </summary>
        internal void ReleaseResources(ref List<Student> students, int threadId)
        {
            lock(students)
            {
                foreach (var eqName in Student.SubjectToPassing.EquipmentNames)
                {
                    var eq = Equipments.Where(e => e.Name == eqName).FirstOrDefault();
                    if (eq == null)
                        throw new Exception($"Попытка освободить несуществующий ресурс {eqName}.");
                    ++eq.Count;
                }

                if (Student.SubjectToPassing.RemainingTime > 0)
                    Student.CurrentState = CurrentThreadState.W;
                else
                    Student.CurrentState = CurrentThreadState.F;
                
                ++Teacher.NumberOfStudents;

                //logger.LogReleasedResources(this, threadId, plainType, QueueLevel);
            }
        }
    }
}
