using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using QuizApp.Models;

namespace QuizApp.Views
{
    public partial class QuizPassWindow : Window
    {
        private readonly List<Question> _questions;
        private int _currentIndex = 0;
        private int _correctCount = 0;
        private int _wrongCount = 0;
        private bool _answered = false;
        private RadioButton[] _radioButtons;

        public QuizPassWindow(List<Question> questions)
        {
            InitializeComponent();
            _questions = questions;
            ShowQuestion(0);
        }

        private void ShowQuestion(int index)
        {
            if (index >= _questions.Count)
            {
                FinishQuiz();
                return;
            }

            var q = _questions[index];
            ProgressText.Text = $"Вопрос {index + 1} из {_questions.Count}";
            QuestionTextBlock.Text = q.Text;

            // Очищаем предыдущие варианты
            OptionsPanel.Children.Clear();

            // Создаем RadioButton для каждого варианта
            _radioButtons = new RadioButton[q.Options.Count];
            for (int i = 0; i < q.Options.Count; i++)
            {
                var rb = new RadioButton
                {
                    Content = q.Options[i],
                    Margin = new Thickness(5),
                    GroupName = "OptionsGroup",
                    Tag = i
                };
                OptionsPanel.Children.Add(rb);
                _radioButtons[i] = rb;
            }

            AnswerButton.IsEnabled = true;
            NextButton.IsEnabled = false;
            _answered = false;
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (_answered) return;

            // Находим выбранный вариант
            int selectedIndex = -1;
            for (int i = 0; i < _radioButtons.Length; i++)
            {
                if (_radioButtons[i].IsChecked == true)
                {
                    selectedIndex = i;
                    break;
                }
            }

            if (selectedIndex == -1)
            {
                MessageBox.Show("Выберите вариант ответа.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var currentQuestion = _questions[_currentIndex];
            bool isCorrect = (selectedIndex == currentQuestion.CorrectOptionIndex);

            if (isCorrect)
            {
                _correctCount++;
                MessageBox.Show("Правильно!", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _wrongCount++;
                string correctAnswer = currentQuestion.Options[currentQuestion.CorrectOptionIndex];
                MessageBox.Show($"Неправильно. Правильный ответ: {correctAnswer}", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Блокируем повторный ответ
            AnswerButton.IsEnabled = false;
            NextButton.IsEnabled = true;
            _answered = true;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            _currentIndex++;
            if (_currentIndex < _questions.Count)
            {
                ShowQuestion(_currentIndex);
            }
            else
            {
                FinishQuiz();
            }
        }

        private void FinishQuiz()
        {
            // Показываем окно с результатами
            var resultWindow = new ResultWindow(_correctCount, _wrongCount);
            resultWindow.ShowDialog();
            Close();
        }
    }
}