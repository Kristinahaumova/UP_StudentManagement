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
    public partial class Departments : Page
    {
        public Departments()
        {
            InitializeComponent();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Login());
        }

        private void createRecord(object sender, RoutedEventArgs e)
        {

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

        }

        private void btnEditDepartment(object sender, RoutedEventArgs e)
        {

        }

        private void btnDeleteDepartment(object sender, RoutedEventArgs e)
        {

        }
    }
}
