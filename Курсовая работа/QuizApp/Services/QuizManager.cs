using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using QuizApp.Models;

namespace QuizApp.Services
{
    public class QuizManager
    {
        public List<Quiz> Quizzes { get; private set; } = new List<Quiz>();
        private readonly string _filePath = "quizzes.json";

        public void Load()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                Quizzes = JsonSerializer.Deserialize<List<Quiz>>(json) ?? new List<Quiz>();
            }
            else
            {
                Quizzes = new List<Quiz>();
            }
        }

        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Quizzes, options);
            File.WriteAllText(_filePath, json);
        }

        public void AddQuiz(Quiz quiz)
        {
            Quizzes.Add(quiz);
            Save();
        }

        public void RemoveQuiz(Quiz quiz)
        {
            Quizzes.Remove(quiz);
            Save();
        }

        public void UpdateQuiz(Quiz updatedQuiz)
        {
            var existing = Quizzes.Find(q => q.Id == updatedQuiz.Id);
            if (existing != null)
            {
                existing.Name = updatedQuiz.Name;
                existing.Questions = updatedQuiz.Questions;
                Save();
            }
        }
    }
}