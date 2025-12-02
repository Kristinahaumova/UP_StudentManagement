using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Pages.Admin
{
    public partial class Departments_Add : Window
    {
        public Departments_Add()
        {
            InitializeComponent();
        }

        private void btnAdd(object sender, RoutedEventArgs e)
        {
            string departmentName = txtDepartmentName.Text.Trim();

            if (string.IsNullOrEmpty(departmentName)) 
            {
                MessageBox.Show("Введите название отделения");
                return;
            }

            try 
            {
                DepartmentContext departmentContext = new DepartmentContext();

                var allDepartments = departmentContext.AllDepartments();
                bool nameExists = allDepartments.Exists(u => u.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));
                if (nameExists)
                {
                    MessageBox.Show("Отделение с таким названием уже существует.");
                    return;
                }
                DepartmentContext newDepartment = new DepartmentContext
                {
                    Name = departmentName
                };
                newDepartment.Save(false);
                //TODO: Добавить обновление списка отделений
                Close();
            } 
            catch (Exception ex) 
            {
                MessageBox.Show($"Ошибка при добавлении отделения {ex}");
            }
        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
