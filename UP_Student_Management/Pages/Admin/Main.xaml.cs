using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UP_Student_Management.Classes.Context;
using UP_Student_Management.Classes.Context.StatusContext;
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
            LoadCombobox();
            LoadFilters();
            


            if (!MainWindow.IsAdmin) 
            {
                gbWhatDo.Visibility = Visibility.Hidden;
            }
        }
        private void updateList()
        {
            try
            {
                StudentContext studentContext = new StudentContext();
                var allStudentsData = studentContext.AllStudents();
                var allDepartments = new DepartmentContext().AllDepartments();

                var sirotaContext = new SirotaContext();
                var invalidContext = new InvalidContext();
                var hostelContext = new HostelContext();
                var ovzContext = new OvzContext();
                var riskGroupContext = new RiskGroupContext();
                var scholarshipContext = new ScholarshipContext();
                var spppContext = new SPPPContext();
                var svoContext = new SVOContext();

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
                            Education = student.Education,
                            Statuses = GetStudentStatuses(student.Id, sirotaContext, invalidContext, hostelContext, ovzContext, riskGroupContext, scholarshipContext, spppContext, svoContext)
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
        private List<string> GetStudentStatuses(int studentId, SirotaContext sirotaContext, InvalidContext invalidContext,
            HostelContext hostelContext, OvzContext ovzContext, RiskGroupContext riskGroupContext,
            ScholarshipContext scholarshipContext, SPPPContext spppContext, SVOContext svoContext)
        {
            var statuses = new List<string>();

            if (sirotaContext.AllSirota().Any(s => s.StudentId == studentId)) statuses.Add("Сирота");
            if (invalidContext.AllInvalid().Any(i => i.StudentId == studentId)) statuses.Add("Инвалид");
            if (hostelContext.AllHostel().Any(i => i.StudentId == studentId)) statuses.Add("Общежитие");
            if (ovzContext.AllOvz().Any(i => i.StudentId == studentId)) statuses.Add("ОВЗ");
            if (riskGroupContext.AllRiskGroup().Any(i => i.StudentId == studentId)) statuses.Add("Группа риска");
            if (scholarshipContext.AllScholarship().Any(i => i.StudentId == studentId)) statuses.Add("Стипендия");
            if (spppContext.AllSPPP().Any(i => i.StudentId == studentId)) statuses.Add("СППП");
            if (svoContext.AllSVO().Any(i => i.StudentId == studentId)) statuses.Add("СВО");

            return statuses;
        }

        private void FilterStudents()
        {
            studentsView.Filter = item =>
            {
                if (item is StudentDisplay student)
                {
                    string searchQuery = txtSearch.Text.ToLower();
                    bool matchesSearch = string.IsNullOrEmpty(searchQuery) ||
                                        student.Surname.ToLower().Contains(searchQuery) ||
                                        student.Firstname.ToLower().Contains(searchQuery) ||
                                        student.Patronomyc.ToLower().Contains(searchQuery);

                    bool matchesDepartment = true;
                    if (cmbDepartmentFilter.SelectedItem != null && cmbDepartmentFilter.SelectedIndex > 0)
                    {
                        var selectedDepartment = (DepartmentContext)cmbDepartmentFilter.SelectedItem;
                        matchesDepartment = student.DepartmentId == selectedDepartment.Id;
                    }

                    bool matchesStatus = true;
                    var selectedStatuses = statusFilterPanel.Children.OfType<CheckBox>().Where(c => c.IsChecked == true).Select(c => c.Content.ToString()).ToList();
                    if (selectedStatuses.Count > 0)
                    {
                        matchesStatus = selectedStatuses.All(status => student.Statuses.Contains(status));
                    }

                    bool matchesSex = true;
                    if (cmbSexFilter.SelectedItem != null && cmbSexFilter.SelectedIndex > 0)
                    {
                        string selectedSex = cmbSexFilter.SelectedItem.ToString();
                        matchesSex = student.Sex == selectedSex;
                    }

                    bool matchesBudget = true;
                    if (cmbBudgetFilter.SelectedItem != null && cmbBudgetFilter.SelectedIndex > 0)
                    {
                        string selectedBudget = cmbBudgetFilter.SelectedItem.ToString();
                        matchesBudget = (selectedBudget == "Бюджет" && student.isBudget == 1) ||
                                         (selectedBudget == "Контракт" && student.isBudget == 0);
                    }

                    bool matchesEducation = true;
                    if (cmbEducationFilter.SelectedItem != null && cmbEducationFilter.SelectedIndex > 0)
                    {
                        string selectedEducation = cmbEducationFilter.SelectedItem.ToString();
                        matchesEducation = student.Education == selectedEducation;
                    }

                    return matchesSearch && matchesDepartment && matchesStatus && matchesSex && matchesBudget && matchesEducation;
                }
                return false;
            };
        }


        private void LoadCombobox() 
        {
            LoadDepartments();
            LoadStatuses();
        }
        private void LoadDepartments()
        {
            try
            {
                var departmentContext = new DepartmentContext();
                var departments = departmentContext.AllDepartments();

                cmbDepartmentFilter.Items.Clear();
                cmbDepartmentFilter.Items.Add("Выберите отделение");
                foreach (var department in departments)
                {
                    cmbDepartmentFilter.Items.Add(department);
                }

                cmbDepartmentFilter.DisplayMemberPath = "Name";
                cmbDepartmentFilter.SelectedValuePath = "Id";

                if (cmbDepartmentFilter.Items.Count > 0)
                    cmbDepartmentFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отделений: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadStatuses()
        {
            statusFilterPanel.Children.Clear();
            string[] statuses = { "Сирота", "Инвалид", "Общежитие", "ОВЗ", "Группа риска", "Стипендия", "СППП", "СВО" };
            foreach (string status in statuses)
            {
                CheckBox checkBox = new CheckBox { Content = status, Margin = new Thickness(5, 0, 5, 0) };
                checkBox.Checked += (s, e) => FilterStudents();
                checkBox.Unchecked += (s, e) => FilterStudents();
                statusFilterPanel.Children.Add(checkBox);
            }
        }

        private void LoadFilters()
        {
            cmbSexFilter.Items.Add("Все");
            cmbSexFilter.Items.Add("М");
            cmbSexFilter.Items.Add("Ж");
            cmbSexFilter.SelectedIndex = 0;

            cmbBudgetFilter.Items.Add("Все");
            cmbBudgetFilter.Items.Add("Бюджет");
            cmbBudgetFilter.Items.Add("Контракт");
            cmbBudgetFilter.SelectedIndex = 0;

            cmbEducationFilter.Items.Add("Все");
            cmbEducationFilter.Items.Add("11");
            cmbEducationFilter.Items.Add("9");
            cmbEducationFilter.SelectedIndex = 0;
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
            if (datagridStudents.SelectedItem is StudentDisplay selectedDisplay)
            {
                var studentContext = new StudentContext();
                var student = studentContext.AllStudents().FirstOrDefault(s => s.Id == selectedDisplay.Id);
                if (student != null)
                {
                    var dialog = new AddStatus(student);
                    dialog.Closed += (s, args) => updateList();
                    dialog.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Выберите студента для добавления статуса");
            }
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
            if (datagridStudents.SelectedItem is StudentDisplay selectedStudent)
            {
                try
                {
                    var excelApp = new Microsoft.Office.Interop.Excel.Application();
                    excelApp.Visible = true;
                    var workbook = excelApp.Workbooks.Add();
                    var worksheet = workbook.Sheets[1];

                    worksheet.Cells[1, 1] = "ФИО";
                    worksheet.Cells[1, 2] = selectedStudent.FullName;
                    worksheet.Cells[2, 1] = "Группа";
                    worksheet.Cells[2, 2] = selectedStudent.GroupName;
                    worksheet.Cells[3, 1] = "Отделение";
                    worksheet.Cells[3, 2] = selectedStudent.DepartmentName;
                    worksheet.Cells[4, 1] = "Телефон";
                    worksheet.Cells[4, 2] = selectedStudent.Phone;
                    worksheet.Cells[5, 1] = "Финансирование";
                    worksheet.Cells[5, 2] = selectedStudent.Financing;
                    worksheet.Cells[6, 1] = "Год поступления";
                    worksheet.Cells[6, 2] = selectedStudent.YearReceipts;
                    worksheet.Cells[7, 1] = "Год окончания";
                    worksheet.Cells[7, 2] = selectedStudent.YearFinish;

                    var statuses = GetStudentStatuses(selectedStudent.Id);
                    int row = 9;
                    worksheet.Cells[row, 1] = "Статусы студента:";
                    row++;
                    foreach (var status in statuses)
                    {
                        worksheet.Cells[row, 1] = status.StatusType;
                        worksheet.Cells[row, 2] = status.Note;
                        worksheet.Cells[row, 3] = status.StartDate.ToString("dd.MM.yyyy");
                        worksheet.Cells[row, 4] = status.EndDate?.ToString("dd.MM.yyyy") ?? "";
                        row++;
                    }
                    worksheet.Columns.AutoFit();
                    string desctopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string filePath = System.IO.Path.Combine(desctopPath, "Отчет по студенту " + selectedStudent.Surname + ".xlsx");
                    workbook.SaveAs(filePath);
                    workbook.Close();
                    excelApp.Quit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка создания отчета: {ex.Message}");
                }
            }
            else 
            {
                MessageBox.Show("Выберите студента для создания отчета");
            }
        }
        private List<StudentStatus> GetStudentStatuses(int studentId)
        {
            var statuses = new List<StudentStatus>();

            var sirotaContext = new SirotaContext();
            var invalidContext = new InvalidContext();
            var hostelContext = new HostelContext();
            var ovzContext = new OvzContext();
            var riskGroupContext = new RiskGroupContext();
            var scholarshipContext = new ScholarshipContext();
            var spppContext = new SPPPContext();
            var svoContext = new SVOContext();

            statuses.AddRange(sirotaContext.AllSirota().Where(i => i.StudentId == studentId).Select(i => new StudentStatus
            {
                StatusType = "Сирота",
                Note = i.Note,
                StartDate = i.StartDate,
                EndDate = i.EndDate
            }));

            statuses.AddRange(invalidContext.AllInvalid().Where(i => i.StudentId == studentId).Select(i => new StudentStatus
            {
                StatusType = "Инвалид",
                Note = i.Note,
                StartDate = i.StartDate,
                EndDate = i.EndDate
            }));

            statuses.AddRange(hostelContext.AllHostel().Where(i => i.StudentId == studentId).Select(i => new StudentStatus
            {
                StatusType = "Общежитие",
                Note = i.Note,
                StartDate = i.StartDate,
                EndDate = i.EndDate
            }));

            statuses.AddRange(ovzContext.AllOvz().Where(i => i.StudentId == studentId).Select(i => new StudentStatus
            {
                StatusType = "ОВЗ",
                Note = i.Note,
                StartDate = i.StartDate,
                EndDate = i.EndDate
            }));

            statuses.AddRange(riskGroupContext.AllRiskGroup().Where(i => i.StudentId == studentId).Select(i => new StudentStatus
            {
                StatusType = "Группа риска",
                Note = i.Note,
                StartDate = i.StartDate,
                EndDate = i.EndDate
            }));

            statuses.AddRange(scholarshipContext.AllScholarship().Where(i => i.StudentId == studentId).Select(i => new StudentStatus
            {
                StatusType = "Стипендия",
                StartDate = i.StartDate,
                EndDate = i.EndDate
            }));

            statuses.AddRange(spppContext.AllSPPP().Where(i => i.StudentId == studentId).Select(i => new StudentStatus
            {
                StatusType = "СППП",
                Note = i.Note,
                StartDate = i.Date
            }));

            statuses.AddRange(svoContext.AllSVO().Where(i => i.StudentId == studentId).Select(i => new StudentStatus
            {
                StatusType = "СВО",
                StartDate = i.StartDate,
                EndDate = i.EndDate
            }));


            return statuses;
        }
        private void cmbDepartmentFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterStudents();
        }

        private void cmbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterStudents();
        }
        private void cmbSexFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterStudents();
        }

        private void cmbBudgetFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterStudents();
        }

        private void cmbEducationFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterStudents();
        }

    }
}
