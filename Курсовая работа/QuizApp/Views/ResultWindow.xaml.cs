using System.Windows;

namespace QuizApp.Views
{
    public partial class ResultWindow : Window
    {
        public ResultWindow(int correct, int wrong)
        {
            InitializeComponent();
            CorrectCountText.Text = correct.ToString();
            WrongCountText.Text = wrong.ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}