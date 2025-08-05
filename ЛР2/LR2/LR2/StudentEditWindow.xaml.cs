using System;
using System.Windows;

namespace LR2
{

    public partial class StudentEditWindow : Window
    {
    public int StudentId { get; private set; }
        public string FullName { get; private set; }
        public DateTime? BirthDate { get; private set; }
        public int PhysicsGrade { get; private set; }
        public int MathGrade { get; private set; }

        public StudentEditWindow()
        {
            InitializeComponent();
            Title = "Добавление нового студента";
        }

        public StudentEditWindow(Student student) : this()
        {
            Title = "Редактирование студента";
            StudentId = student.StudentId;
            FullName = student.FullName;
            BirthDate = student.BirthDate;
            PhysicsGrade = student.PhysicsGrade;
            MathGrade = student.MathGrade;

            txtId.Text = student.StudentId.ToString();
            txtFullName.Text = student.FullName;
            dpBirthDate.SelectedDate = student.BirthDate;
            cbPhysics.SelectedIndex = student.PhysicsGrade - 2;
            cbMath.SelectedIndex = student.MathGrade - 2;
        }
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Пожалуйста, введите ФИО студента", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return; 
            }
            if (string.IsNullOrEmpty(txtId.Text))
            {
                StudentId = new Random().Next(1000, 9999);
            }
            FullName = txtFullName.Text;
            BirthDate = dpBirthDate.SelectedDate;
            PhysicsGrade = cbPhysics.SelectedIndex + 2;
            MathGrade = cbMath.SelectedIndex + 2;

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click (object sender, EventArgs e) 
        {
            DialogResult = false;
            Close();
        }

    }
}
