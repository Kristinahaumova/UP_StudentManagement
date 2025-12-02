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

namespace UP_Student_Management.Pages.Admin
{
    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Login());
        }     

        private void btnAddStudent(object sender, RoutedEventArgs e)
        {
            var dialog = new Students_Add();
            dialog.ShowDialog();
        }
        private void btnAddStatus(object sender, RoutedEventArgs e)
        {
            //TODO:Передавать пользователя
            var dialog = new AddStatus();
            dialog.ShowDialog();
        }
        private void btnEditStudent(object sender, RoutedEventArgs e)
        {
            //TODO:Передавать пользователя
            var dialog = new Student_Edit();
            dialog.ShowDialog();
        }

        private void btnDeleteStudent(object sender, RoutedEventArgs e)
        {
            //TODO:Удалять пользователя
        }
        private void gotoRooms(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Admin.Rooms());
        }

        private void gotoDepartment(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Admin.Departments());
        }

        private void createRecord(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateReport();
            dialog.ShowDialog();
        }
    }
}
