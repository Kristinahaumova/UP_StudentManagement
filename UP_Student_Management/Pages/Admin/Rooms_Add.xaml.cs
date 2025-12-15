using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Pages.Admin
{
    public partial class Rooms_Add : Window
    {
        public Rooms_Add()
        {
            InitializeComponent();
        }

        private void btnAdd(object sender, RoutedEventArgs e)
        {
            string roomNumber = txtRoomNumber.Text.Trim();
            
            if (string.IsNullOrEmpty(roomNumber))
            {
                MessageBox.Show("Заполните номер комнаты");
                return;
            }

            string inputCapacity = txtCapacity.Text.Trim();

            if (string.IsNullOrEmpty(inputCapacity))
            {
                MessageBox.Show("Заполните вместимость комнаты");
                return;
            }

            int capacity = int.Parse(inputCapacity);
            try 
            {
                RoomContext roomContext = new RoomContext();

                var allRooms = roomContext.AllRooms();
                bool numberExists = allRooms.Exists(u => u.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));
                if (numberExists)
                {
                    MessageBox.Show("Комната с таким номером уже существует.");
                    return;
                }
                RoomContext newRoom = new RoomContext
                {
                    Name = roomNumber,
                    Capacity = capacity
                };
                newRoom.Save(false);
                Close();
            } 
            catch (Exception ex) 
            {
                MessageBox.Show($"Ошибка при добавлении комнаты: {ex}");
            }
        }
        private void txtCapacity_PTI(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
        private void btnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
