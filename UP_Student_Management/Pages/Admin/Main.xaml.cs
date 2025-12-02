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
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Pages.Admin
{
    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();
            updateList();
        }
        private void updateList()
        {
            try
            {
                StudentContext studentContext = new StudentContext();
                var allStudents = studentContext.AllStudents();

                // Если не хотите создавать отдельный класс, можно использовать анонимные типы
                var displayData = allStudents.Select(s => new
                {
                    // ФИО
                    FullName = $"{s.Surname} {s.Firstname} {s.Patronomyc}".Trim(),

                    // Остальные поля напрямую
                    GroupName = s.GroupName ?? "",
                    DepartmentName = $"Отделение {s.DepartmentId}",
                    Phone = s.Phone ?? "",
                    Financing = s.isBudget == 1 ? "Бюджет" : "Контракт",
                    YearReceipts = s.YearReceipts.Year,
                    YearFinish = s.YearFinish.Year,

                    // Скрытые поля для обработки
                    Id = s.Id,
                    Firstname = s.Firstname,
                    Surname = s.Surname,
                    Patronomyc = s.Patronomyc,
                    DepartmentId = s.DepartmentId,
                    isBudget = s.isBudget,
                    BirthDate = s.BirthDate,
                    Sex = s.Sex,
                    Education = s.Education
                }).ToList();

                datagridStudents.ItemsSource = displayData;
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
