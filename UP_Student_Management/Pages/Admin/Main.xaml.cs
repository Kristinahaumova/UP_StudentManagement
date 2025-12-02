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

        public delegate void UpdateListDelegate();
        public static UpdateListDelegate UpdateListRequested; // Должно быть static
        // В главном окне
        private void UpdateStudentsList() // Изменили на множественное число
        {
            Dispatcher.Invoke(() =>
            {
                updateList();
            });
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            UpdateListRequested -= UpdateStudentsList;
        }

        public Main()
        {
            InitializeComponent();
            UpdateListRequested += UpdateStudentsList; // Теперь совпадает
            updateList();
        }
        private void updateList()
        {
            try
            {
                StudentContext studentContext = new StudentContext();
                var allStudents = studentContext.AllStudents();
                var allDepartments = new DepartmentContext().AllDepartments();

                // Если не хотите создавать отдельный класс, можно использовать анонимные типы
                var displayData = allStudents
            .Join(allDepartments,
                student => student.DepartmentId,
                department => department.Id,
                (student, department) => new
                {
                    // ФИО
                    FullName = $"{student.Surname} {student.Firstname} {student.Patronomyc}".Trim(),

                    // Остальные поля напрямую
                    GroupName = student.GroupName ?? "",
                    DepartmentName = department.Name ?? $"Отделение {department.Id}",
                    Phone = student.Phone ?? "",
                    Financing = student.isBudget == 1 ? "Бюджет" : "Контракт",
                    YearReceipts = student.YearReceipts.Year,
                    YearFinish = student.YearFinish.Year,

                    // Скрытые поля для обработки
                    Id = student.Id,
                    Firstname = student.Firstname,
                    Surname = student.Surname,
                    Patronomyc = student.Patronomyc,
                    DepartmentId = student.DepartmentId,
                    isBudget = student.isBudget,
                    BirthDate = student.BirthDate,
                    Sex = student.Sex,
                    Education = student.Education
                })
            .ToList();

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
