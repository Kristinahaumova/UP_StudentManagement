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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Pages.Admin
{
    public partial class Departments : Page
    {
        public Departments()
        {
            InitializeComponent();
            updateList();
        }
        private void updateList()
        {
            try
            {
                DepartmentContext departmentContext = new DepartmentContext();
                var allDepartments = departmentContext.AllDepartments();
                datagridDepartments.ItemsSource = allDepartments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Login());
        }

        private void gotoRooms(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Admin.Rooms());
        }

        private void gotoStudents(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Admin.Main());
        }

        private void btnAddDepartment(object sender, RoutedEventArgs e)
        {
            var dialog = new Departments_Add();
            dialog.ShowDialog();
        }

        private void btnEditDepartment(object sender, RoutedEventArgs e)
        {
            var dialog = new Departments_Edit();
            dialog.ShowDialog();
        }

        private void btnDeleteDepartment(object sender, RoutedEventArgs e)
        {

        }
    }
}
