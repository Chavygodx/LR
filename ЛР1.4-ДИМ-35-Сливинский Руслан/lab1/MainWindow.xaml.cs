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
using System.Globalization;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Eventing.Reader;


namespace lab1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            int currentYear = DateTime.Today.Year;
            for (int year = currentYear - 100; year <= currentYear; year++)
            {
                cmbYear.Items.Add(year);
            }
            for (int month = 1; month <= 12; month++)
            {
                cmbMonth.Items.Add(new
                {
                    Number = month,
                    Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)
                });
            }

            cmbMonth.DisplayMemberPath = "Name";
            cmbMonth.SelectedValuePath = "Number";
        }

        private void YearMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (cmbYear.SelectedItem == null || cmbMonth.SelectedItem == null)
            {
                cmbDay.IsEnabled = false;
                cmbDay.Items.Clear();
                return;
            }

            
            int year = (int)cmbYear.SelectedItem;
            int month = (int)cmbMonth.SelectedValue;

                
            int daysInMonth = DateTime.DaysInMonth(year, month);

            cmbDay.Items.Clear();
            for (int day = 1; day <= daysInMonth; day++)
            {
                cmbDay.Items.Add(day);
            }
            cmbDay.IsEnabled = true;
            txtResult.Text = "";
        }

        private void Day_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDay.SelectedItem == null) return;

            try
            {
                DateTime selectedDate = new DateTime(
                    (int)cmbYear.SelectedItem,
                    (int)cmbMonth.SelectedValue,
                    (int)cmbDay.SelectedItem
                );
                CalculateAndDisplayDifference(selectedDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void CalculateAndDisplayDifference(DateTime selectedDate)
        {
            DateTime today = DateTime.Today;
            if (selectedDate > today)
            {
                txtResult.Text = "Выбранная дата ещё не наступила!";
                return;
            }
            int years = today.Year - selectedDate.Year;
            int months = today.Month - selectedDate.Month;
            int days = today.Day - selectedDate.Day;
            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(today.Year, today.Month);
            }
            if (months < 0)
            {
                years--;
                months += 12;
            }
            txtResult.Text = $"Прошло: {years} {GetRussianWord(years, "год", "года", "лет")}, " +
                             $"{months} {GetRussianWord(months, "месяц", "месяца", "месяцев")}, " +
                             $"{days} {GetRussianWord(days, "день", "дня", "дней")}";
        }
        private string GetRussianWord(int number, string form1, string form2, string form5)
        {
            number = Math.Abs(number) % 100;
            int remainder = number % 10;

            if (number > 10 && number < 20) return form5;
            if (remainder > 1 && remainder < 5) return form2;
            if (remainder == 1) return form1;

            return form5;
        }
    }
}