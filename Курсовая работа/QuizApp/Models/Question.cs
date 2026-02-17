using System.Collections.Generic;

namespace QuizApp.Models
{
    public class Question : Entity
    {
        public string Text { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectOptionIndex { get; set; } // индекс правильного ответа (0-based)

        public Question() { }

        public Question(string name, string text)
        {
            Name = name;
            Text = text;
        }
    }
}