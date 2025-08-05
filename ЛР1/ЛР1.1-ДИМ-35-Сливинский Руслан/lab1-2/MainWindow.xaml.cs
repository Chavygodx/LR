using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab1_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtA.Text, out double a))
            {
                MessageBox.Show("Неверный формат числа в поле А");
                return;
            }
            if (!double.TryParse(txtB.Text, out double b))
            {
                MessageBox.Show("Неверный формат числа в поле Б");
                return;
            }

            Button button = (Button)sender;
            double result = 0;

            try
            {
                switch(button.Content.ToString())
                {
                    case "+":
                        result = a + b;
                        break;
                        case "-":
                        result = a - b;
                        break;
                        case "*":
                        result = a * b;
                        break;
                        case "/":
                        if (b == 0) throw new DivideByZeroException();
                        result = a / b;
                        break;
                }
                txtResult.Text = result.ToString();
            }
            catch(DivideByZeroException)
            {
                MessageBox.Show("Ошибка: деление на ноль!");
                txtResult.Text = "Ошибка";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
                txtResult.Text = "Ошибка";
            }
        }
    }

}