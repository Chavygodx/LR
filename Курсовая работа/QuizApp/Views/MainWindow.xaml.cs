using System.Windows;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly QuizManager _quizManager;

        public MainWindow()
        {
            InitializeComponent();
            _quizManager = new QuizManager();
            _quizManager.Load();
            QuizzesListBox.ItemsSource = _quizManager.Quizzes;
        }

        private void QuizzesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool isSelected = QuizzesListBox.SelectedItem != null;
            PassButton.IsEnabled = isSelected;
            EditButton.IsEnabled = isSelected;
            DeleteButton.IsEnabled = isSelected;

            if (isSelected)
            {
                var selectedQuiz = (Quiz)QuizzesListBox.SelectedItem;
                QuizNameText.Text = selectedQuiz.Name;
                QuestionsCountText.Text = selectedQuiz.Questions.Count.ToString();
            }
            else
            {
                QuizNameText.Text = "";
                QuestionsCountText.Text = "";
            }
        }

        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedQuiz = (Quiz)QuizzesListBox.SelectedItem;
            var passWindow = new QuizPassWindow(selectedQuiz.Questions);
            passWindow.ShowDialog();
        }

        private void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditQuizWindow(new Quiz(), _quizManager);
            if (editWindow.ShowDialog() == true)
            {
                QuizzesListBox.Items.Refresh(); // обновим отображение (биндинг сам обновится)
            }
        }

        private void EditQuiz_Click(object sender, RoutedEventArgs e)
        {
            var selectedQuiz = (Quiz)QuizzesListBox.SelectedItem;
            // Передаём копию? Нет, работаем с оригиналом, но сохраняем изменения через менеджер.
            var editWindow = new EditQuizWindow(selectedQuiz, _quizManager);
            if (editWindow.ShowDialog() == true)
            {
                QuizzesListBox.Items.Refresh();
                // обновим информацию справа
                QuizzesListBox_SelectionChanged(null, null);
            }
        }

        private void DeleteQuiz_Click(object sender, RoutedEventArgs e)
        {
            var selectedQuiz = (Quiz)QuizzesListBox.SelectedItem;
            var result = MessageBox.Show($"Удалить викторину \"{selectedQuiz.Name}\"?",
                                          "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _quizManager.RemoveQuiz(selectedQuiz);
                QuizzesListBox.Items.Refresh();
            }
        }
    }
}