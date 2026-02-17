using System.Collections.Generic;

namespace QuizApp.Models
{
    public class Quiz : Entity
    {
        public List<Question> Questions { get; set; } = new List<Question>();

        public Quiz() { }

        public Quiz(string name)
        {
            Name = name;
        }
    }
}