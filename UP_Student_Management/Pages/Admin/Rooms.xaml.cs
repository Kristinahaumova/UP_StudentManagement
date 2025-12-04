using System;
using System.Windows;
using System.Windows.Controls;
using UP_Student_Management.Classes.Context;
using UP_Student_Management.Classes.Models;

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
                dataGridRooms.ItemsSource = new RoomContext().AllRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
        private void DatagridRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridRooms.SelectedItem == null)
                return;
            else
            {
                var selectedRoom = dataGridRooms.SelectedItem as dynamic;
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Login());
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
            dialog.Closed += (s, args) => updateList();
            dialog.ShowDialog();
        }

        private void btnEditRooms(object sender, RoutedEventArgs e)
        {
            if (dataGridRooms.SelectedItem is RoomContext selectedRoom)
            {
                var dialog = new Rooms_Edit(selectedRoom);
                dialog.Closed += (s, args) => updateList();
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите комнату для редактирования");
            }
        }

        private void btnDeleteRooms(object sender, RoutedEventArgs e)
        {
            if (dataGridRooms.SelectedItem is Room selectedRoom)
            {
                var result = MessageBox.Show($"Удалить комнату '{selectedRoom.Name}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) 
                {
                    try 
                    {
                        if (selectedRoom is RoomContext roomContext)
                        {
                            roomContext.Delete();
                        }
                        else 
                        {
                            MessageBox.Show("Не удалось преобразовать объект комнаты.");
                            return;
                        }
                        updateList();
                    } 
                    catch (Exception ex) 
                    {
                        MessageBox.Show($"Ошибка при удалении комнаты: {ex.Message}");
                    }
                }

            }
            else 
            {
                MessageBox.Show("Выберите комнату для удаления");
            }
        }

        private void createRecord(object sender, RoutedEventArgs e)
        {

        }
    }
}
