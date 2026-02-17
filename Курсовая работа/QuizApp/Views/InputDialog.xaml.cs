using System.Windows;

namespace QuizApp.Views
{
    public partial class InputDialog : Window
    {
        public string Answer { get; private set; }

        public InputDialog(string prompt, string defaultValue = "")
        {
            InitializeComponent();
            PromptText.Text = prompt;
            InputTextBox.Text = defaultValue;
            InputTextBox.Focus();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Answer = InputTextBox.Text;
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