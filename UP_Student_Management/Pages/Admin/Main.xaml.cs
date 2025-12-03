using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UP_Student_Management.Classes.Context;
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Pages.Admin
{
    public partial class Main : Page
    {
        private ObservableCollection<StudentDisplay> allStudents;
        private ICollectionView studentsView;

        public delegate void UpdateListDelegate();
        public static UpdateListDelegate UpdateListRequested;

        private void UpdateStudentsList()
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
            UpdateListRequested += UpdateStudentsList;
            updateList();
        }
        private void updateList()
        {
            try
            {
                StudentContext studentContext = new StudentContext();
                var allStudentsData = studentContext.AllStudents();
                var allDepartments = new DepartmentContext().AllDepartments();

                var displayData = allStudentsData
                    .Join(allDepartments,
                        student => student.DepartmentId,
                        department => department.Id,
                        (student, department) => new StudentDisplay
                        {
                            FullName = $"{student.Surname} {student.Firstname} {student.Patronomyc}".Trim(),
                            GroupName = student.GroupName ?? "",
                            DepartmentName = department.Name ?? $"Отделение {department.Id}",
                            Phone = student.Phone ?? "",
                            Financing = student.isBudget == 1 ? "Бюджет" : "Контракт",
                            YearReceipts = student.YearReceipts.Year,
                            YearFinish = student.YearFinish.Year,
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

                allStudents = new ObservableCollection<StudentDisplay>(displayData);
                studentsView = CollectionViewSource.GetDefaultView(allStudents);
                datagridStudents.ItemsSource = studentsView;

                txtSearch.TextChanged += (s, e) => FilterStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
        private void FilterStudents()
        {
            studentsView.Filter = item =>
            {
                if (item is StudentDisplay student)
                {
                    string searchQuery = txtSearch.Text.ToLower();
                    return string.IsNullOrEmpty(searchQuery) ||
                           student.Surname.ToLower().Contains(searchQuery) ||
                           student.Firstname.ToLower().Contains(searchQuery) ||
                           student.Patronomyc.ToLower().Contains(searchQuery);
                }
                return false;
            };
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Login());
        }     

        private void btnAddStudent(object sender, RoutedEventArgs e)
        {
            var dialog = new Students_Add();
            dialog.Closed += (s, args) => updateList();
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
            if (datagridStudents.SelectedItem is StudentDisplay selectedDisplay)
            {
                var studentContext = new StudentContext();
                var student = studentContext.AllStudents().FirstOrDefault(s => s.Id == selectedDisplay.Id);
                if (student != null)
                {
                    var dialog = new Student_Edit(student);
                    dialog.Closed += (s, args) => updateList();
                    dialog.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Студент не найден");
                }
            }
            else
            {
                MessageBox.Show("Выберите студента для редактирования");
            }
        }

        private void btnDeleteStudent(object sender, RoutedEventArgs e)
        {
            if (datagridStudents.SelectedItem is StudentDisplay selectedDisplay)
            {
                var result = MessageBox.Show($"Удалить студента '{selectedDisplay.Surname}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        StudentContext studentContext = new StudentContext();
                        var studentToDelete = studentContext.AllStudents().FirstOrDefault(s => s.Id == selectedDisplay.Id);
                        if (studentToDelete != null)
                        {
                            studentToDelete.Delete();
                            updateList(); 
                        }
                        else
                        {
                            MessageBox.Show("Студент не найден.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении студента: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите студента для удаления");
            }
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
