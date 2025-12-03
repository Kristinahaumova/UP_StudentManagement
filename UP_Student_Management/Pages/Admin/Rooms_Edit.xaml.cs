using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Pages.Admin
{
    public partial class Rooms_Edit : Window
    {
        private RoomContext room;
        public Rooms_Edit(RoomContext room)
        {
            InitializeComponent();
            this.room = room;
            txtRoomNumber.Text = room.Name;
            txtCapacity.Text = room.Capacity.ToString();
        }

        private void btnEdit(object sender, RoutedEventArgs e)
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
                room.Name = roomNumber;
                room.Capacity = capacity;
                room.Save(true);
                MessageBox.Show("Комната успешно обновлена");
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании комнаты: {ex.Message}");
            }
        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtCapacity_PTI(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}
