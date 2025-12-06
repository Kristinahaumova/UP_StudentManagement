using System;
using System.Windows;
using System.Windows.Controls;
using UP_Student_Management.Classes.Context;
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Pages.Admin
{
    public partial class Departments : Page
    {
        public Departments()
        {
            InitializeComponent();
            updateList();

            if (!MainWindow.IsAdmin)
            {
                gbWhatDo.Visibility = Visibility.Hidden;
            }
        }
        private void updateList()
        {
            try
            {
                datagridDepartments.ItemsSource = new DepartmentContext().AllDepartments();
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
            dialog.Closed += (s, args) => updateList();
            dialog.ShowDialog();
        }

        private void btnEditDepartment(object sender, RoutedEventArgs e)
        {
            if (datagridDepartments.SelectedItem is DepartmentContext selectedDepartment) 
            {
                var dialog = new Departments_Edit(selectedDepartment);
                dialog.Closed += (s, args) => updateList();
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите отделение для редактирования");
            }
        }

        private void btnDeleteDepartment(object sender, RoutedEventArgs e)
        {
            if (datagridDepartments.SelectedItem is Department selectedDepartment) 
            {
                var result = MessageBox.Show($"Удалить отделение '{selectedDepartment.Name}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) 
                {
                    try 
                    {
                        if (selectedDepartment is DepartmentContext departmentContext) 
                        {
                            departmentContext.Delete();
                        } 
                        else 
                        {
                            MessageBox.Show("Не удалось преобразовать объект отделения.");
                            return;
                        }
                        updateList();
                    } 
                    catch (Exception ex) 
                    {
                        MessageBox.Show($"Ошибка при удалении отделения: {ex.Message}");
                    }
                }
            } 
            else 
            {
                MessageBox.Show("Выберите отделение для удаления");
            }
        }
    }
}
