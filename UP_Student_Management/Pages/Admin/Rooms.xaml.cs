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
    public partial class Rooms : Page
    {
        public Rooms()
        {
            InitializeComponent();
            updateList();
        }

        private void updateList()
        {
            try
            {
                RoomContext roomContext = new RoomContext();
                var allRooms = roomContext.AllRooms();
                datagridRooms.ItemsSource = allRooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
        private void DatagridRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datagridRooms.SelectedItem == null)
                return;
            else
            {
                var selectedRoom = datagridRooms.SelectedItem as dynamic;
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Login());
        }

        private void createRecord(object sender, RoutedEventArgs e)
        {

        }

        private void gotoDepartments(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Admin.Departments());
        }

        private void gotoStudents(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Admin.Main());
        }

        private void btnAddRooms(object sender, RoutedEventArgs e)
        {
            var dialog = new Rooms_Add();
            dialog.ShowDialog();
        }

        private void btnEditRooms(object sender, RoutedEventArgs e)
        {

        }

        private void btnDeleteRooms(object sender, RoutedEventArgs e)
        {

        }
    }
}
