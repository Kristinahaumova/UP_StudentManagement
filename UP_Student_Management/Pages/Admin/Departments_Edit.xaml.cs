using System;
using System.Windows;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Pages.Admin
{
    public partial class Departments_Edit : Window
    {
        private DepartmentContext department;
        public Departments_Edit(DepartmentContext department)
        {
            InitializeComponent();
            this.department = department;
            txtDepartmentName.Text = department.Name;
        }

        private void btnEdit(object sender, RoutedEventArgs e)
        {
            string departmentName = txtDepartmentName.Text.Trim();
            if (string.IsNullOrEmpty(departmentName))
            {
                MessageBox.Show("Заполните название отделения");
                return;
            }
            try
            {
                department.Name = departmentName;
                department.Save(true);
                MessageBox.Show("Отделение успешно обновлено");
                this.DialogResult = true;
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Ошибка при редактировании отделения: {ex.Message}");
            }
        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
