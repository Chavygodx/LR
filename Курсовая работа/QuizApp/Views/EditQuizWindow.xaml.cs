using System.Linq;
using System.Windows;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Views
{
    public partial class EditQuizWindow : Window
    {
        private readonly Quiz _quiz;
        private readonly QuizManager _quizManager;

        public EditQuizWindow(Quiz quiz, QuizManager quizManager)
        {
            InitializeComponent();
            _quiz = quiz;
            _quizManager = quizManager;

            QuizNameTextBox.Text = _quiz.Name;
            QuestionsListBox.ItemsSource = _quiz.Questions;
        }

        private void QuestionsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool isSelected = QuestionsListBox.SelectedItem != null;
            EditQuestionButton.IsEnabled = isSelected;
            DeleteQuestionButton.IsEnabled = isSelected;
        }

        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            var newQuestion = new Question { Name = "Новый вопрос" };
            var dialog = new AddEditQuestionWindow(newQuestion);
            if (dialog.ShowDialog() == true)
            {
                _quiz.Questions.Add(newQuestion);
                QuestionsListBox.Items.Refresh();
            }
        }

        private void EditQuestion_Click(object sender, RoutedEventArgs e)
        {
            var selectedQuestion = (Question)QuestionsListBox.SelectedItem;
            var dialog = new AddEditQuestionWindow(selectedQuestion);
            if (dialog.ShowDialog() == true)
            {
                QuestionsListBox.Items.Refresh();
            }
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            var selectedQuestion = (Question)QuestionsListBox.SelectedItem;
            _quiz.Questions.Remove(selectedQuestion);
            QuestionsListBox.Items.Refresh();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuizNameTextBox.Text))
            {
                MessageBox.Show("Введите название викторины.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _quiz.Name = QuizNameTextBox.Text.Trim();

            if (_quiz.Id == default) // новая викторина
            {
                _quizManager.AddQuiz(_quiz);
            }
            else // существующая
            {
                _quizManager.UpdateQuiz(_quiz);
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}