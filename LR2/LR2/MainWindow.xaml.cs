using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace LR2
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper _dbHelper;
        private List<Student> _students;

        public MainWindow()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            LoadStudents();
        }

        private void LoadStudents()
        {
            try
            {
                _students = _dbHelper.GetAllStudents();
                studentsGrid.ItemsSource = _students;
                statusText.Text = $"Загружено {_students.Count} студентов";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                statusText.Text = "Ошибка загрузки данных";
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new StudentEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    _dbHelper.AddStudent(
                        editWindow.StudentId,
                        editWindow.FullName,
                        editWindow.BirthDate,
                        editWindow.PhysicsGrade,
                        editWindow.MathGrade);

                    LoadStudents();
                    statusText.Text = "Студент успешно добавлен";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении студента: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    statusText.Text = "Ошибка добавления студента";
                }
            }
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (studentsGrid.SelectedItem is Student selectedStudent)
            {
                var editWindow = new StudentEditWindow(selectedStudent);
                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        _dbHelper.UpdateStudent(
                            editWindow.StudentId,
                            editWindow.FullName,
                            editWindow.BirthDate,
                            editWindow.PhysicsGrade,
                            editWindow.MathGrade);

                        LoadStudents();
                        statusText.Text = "Данные студента обновлены";

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        statusText.Text = "Ошибка обновления данных";
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите студента для редактирования", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (studentsGrid.SelectedItem is Student selectedStudent)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить студента {selectedStudent.FullName}?", "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbHelper.DeleteStudent(selectedStudent.StudentId);
                        LoadStudents();
                        statusText.Text = "Студент удален";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении студента: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        statusText.Text = "Ошибка удаления студента";
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите студента для удаления", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}