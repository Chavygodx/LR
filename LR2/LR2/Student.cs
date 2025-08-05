using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR2
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public int PhysicsGrade { get; set; }
        public int MathGrade { get; set; }
        public string BirthDateString => BirthDate?.ToString("dd.MM.yyyy") ?? "Не указана";

    }
}
