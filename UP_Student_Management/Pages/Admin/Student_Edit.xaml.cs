using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Pages.Admin
{
    public partial class Student_Edit : Window
    {
        private StudentContext student;
        private string selectedFilePath = string.Empty;
        public Student_Edit(StudentContext student)
        {
            InitializeComponent();
            this.student = student;
            LoadComboBoxData();
            LoadStudentData();
            AttachEventHandlers();
        }

        private void btnEdit(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateForm())
                    return;

                UpdateStudentFromForm();
                student.Save(true); 

                if (!string.IsNullOrEmpty(selectedFilePath) && File.Exists(selectedFilePath))
                {
                    string destinationPath = CopyStudentFile(student.Id);
                    if (!string.IsNullOrEmpty(destinationPath))
                    {
                        student.FilePath = destinationPath;
                        student.Save(true);
                    }
                }

                MessageBox.Show($"Студент {student.Surname} {student.Firstname} успешно изменён!",
                    "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении студента: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void btnChooseFile(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Все файлы (*.*)|*.*|Документы (*.pdf;*.doc;*.docx)|*.pdf;*.doc;*.docx|Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.FilterIndex = 2;

                if (openFileDialog.ShowDialog() == true)
                {
                    selectedFilePath = openFileDialog.FileName;
                    MessageBox.Show($"Выбран файл: {Path.GetFileName(selectedFilePath)}", "Файл выбран",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выбора файла: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnDeleteFile(object sender, RoutedEventArgs e)
        {
            DeleteSelectedFile();
        }
        private void LoadComboBoxData()
        {
            cmbEducation.Items.Add("9 классов");
            cmbEducation.Items.Add("11 классов");
            cmbEducation.SelectedIndex = student.Education == "9" ? 0 : 1;

            cmbGender.Items.Add("М");
            cmbGender.Items.Add("Ж");
            cmbGender.SelectedIndex = student.Sex == "М" ? 0 : 1;

            cmbFinance.Items.Add("Бюджет");
            cmbFinance.Items.Add("Контракт");
            cmbFinance.SelectedIndex = student.isBudget == 1 ? 0 : 1;

            LoadDepartments();
        }
        private void LoadDepartments()
        {
            var departmentContext = new DepartmentContext();
            var departments = departmentContext.AllDepartments();

            cmbDepartments.Items.Clear();
            foreach (var department in departments)
            {
                cmbDepartments.Items.Add(department);
            }

            cmbDepartments.DisplayMemberPath = "Name";
            cmbDepartments.SelectedValuePath = "Id";
            cmbDepartments.SelectedItem = departments.FirstOrDefault(d => d.Id == student.DepartmentId);
        }
        private void LoadStudentData()
        {
            txtSurname.Text = student.Surname;
            txtName.Text = student.Firstname;
            txtPatronomyc.Text = student.Patronomyc;
            dateCapacity.SelectedDate = student.BirthDate;
            txtPhoneNumber.Text = student.Phone;
            txtGroup.Text = student.GroupName;
            txtYear.Text = student.YearReceipts.Year.ToString();
            txtYearEnd.Text = student.YearFinish.Year.ToString();
            txtExpulsion.Text = student.DeductionsInfo;
            dateExpulsion.SelectedDate = student.DataDeductions;
            txtDescription.Text = student.Note;
            selectedFilePath = student.FilePath ?? string.Empty;
        }
        private void AttachEventHandlers()
        {
            txtYear.PreviewTextInput += txtYear_PreviewTextInput;
            txtYearEnd.PreviewTextInput += txtYearEnd_PreviewTextInput;
            txtPhoneNumber.PreviewTextInput += txtPhoneNumber_PreviewTextInput;
            txtPhoneNumber.LostFocus += txtPhoneNumber_LostFocus;
            txtYear.LostFocus += txtYear_LostFocus;
            txtSurname.KeyDown += txtSurname_KeyDown;
            txtName.KeyDown += txtName_KeyDown;
            txtPatronomyc.KeyDown += txtPatronomyc_KeyDown;
            dateCapacity.SelectedDateChanged += dateCapacity_SelectedDateChanged;
            dateExpulsion.SelectedDateChanged += dateExpulsion_SelectedDateChanged;
        }
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtSurname.Text))
            {
                MessageBox.Show("Пожалуйста, заполните фамилию студента", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtSurname.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Пожалуйста, заполните имя студента", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return false;
            }

            if (dateCapacity.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите дату рождения", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                dateCapacity.Focus();
                return false;
            }

            if (cmbDepartments.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите отделение", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbDepartments.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtYear.Text))
            {
                MessageBox.Show("Пожалуйста, укажите год поступления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtYear.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtYearEnd.Text))
            {
                MessageBox.Show("Пожалуйста, укажите год окончания", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtYearEnd.Focus();
                return false;
            }

            if (!int.TryParse(txtYear.Text, out int yearReceipts) || yearReceipts < 1900 || yearReceipts > DateTime.Now.Year)
            {
                MessageBox.Show("Год поступления должен быть числом от 1900 до текущего года", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtYear.Focus();
                return false;
            }

            if (!int.TryParse(txtYearEnd.Text, out int yearFinish) || yearFinish < yearReceipts)
            {
                MessageBox.Show("Год окончания должен быть больше или равен году поступления", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtYearEnd.Focus();
                return false;
            }

            int age = DateTime.Now.Year - dateCapacity.SelectedDate.Value.Year;
            if (DateTime.Now.DayOfYear < dateCapacity.SelectedDate.Value.DayOfYear)
                age--;

            if (age < 14)
            {
                MessageBox.Show("Студент должен быть старше 14 лет", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                dateCapacity.Focus();
                return false;
            }

            string phone = txtPhoneNumber.Text.Trim();
            if (!string.IsNullOrEmpty(phone))
            {
                string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
                if (digitsOnly.Length < 10)
                {
                    MessageBox.Show("Номер телефона должен содержать минимум 10 цифр", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPhoneNumber.Focus();
                    return false;
                }
            }

            return true;
        }

        private void UpdateStudentFromForm()
        {
            student.Surname = txtSurname.Text.Trim();
            student.Firstname = txtName.Text.Trim();
            student.BirthDate = dateCapacity.SelectedDate.Value;
            student.Patronomyc = txtPatronomyc.Text.Trim();
            student.Phone = txtPhoneNumber.Text.Trim();
            student.Sex = cmbGender.SelectedItem?.ToString() ?? "М";
            student.Education = cmbEducation.SelectedIndex == 0 ? "9" : "11";
            student.GroupName = txtGroup.Text.Trim();
            student.isBudget = cmbFinance.SelectedIndex == 0 ? 1 : 0;
            student.YearReceipts = new DateTime(int.Parse(txtYear.Text), 1, 1);
            student.YearFinish = new DateTime(int.Parse(txtYearEnd.Text), 1, 1);
            student.DeductionsInfo = txtExpulsion.Text.Trim();
            student.DataDeductions = dateExpulsion.SelectedDate ?? DateTime.MinValue;
            student.Note = txtDescription.Text.Trim();
            student.DepartmentId = ((DepartmentContext)cmbDepartments.SelectedItem).Id;
        }

        private string CopyStudentFile(int studentId)
        {
            try
            {
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "UP_Student_Management",
                    "StudentFiles"
                );

                Directory.CreateDirectory(appDataPath);

                string destinationPath = Path.Combine(
                    appDataPath,
                    $"{studentId}_{Path.GetFileName(selectedFilePath)}"
                );

                File.Copy(selectedFilePath, destinationPath, true);
                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при копировании файла: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return string.Empty;
            }
        }
        private void DeleteSelectedFile()
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                selectedFilePath = string.Empty;
                MessageBox.Show("Файл удален из выбора", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Файл не выбран", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void txtYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void txtYearEnd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void txtPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.Text, 0) || e.Text == "+" || e.Text == "(" || e.Text == ")" || e.Text == "-");
        }

        private void txtPhoneNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            string phone = txtPhoneNumber.Text.Trim();
            if (!string.IsNullOrEmpty(phone))
            {
                string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

                if (digitsOnly.Length == 11 && (digitsOnly.StartsWith("7") || digitsOnly.StartsWith("8")))
                {
                    txtPhoneNumber.Text = $"+7 ({digitsOnly.Substring(1, 3)}) {digitsOnly.Substring(4, 3)}-{digitsOnly.Substring(7, 2)}-{digitsOnly.Substring(9, 2)}";
                }
                else if (digitsOnly.Length == 10)
                {
                    txtPhoneNumber.Text = $"+7 ({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3, 3)}-{digitsOnly.Substring(6, 2)}-{digitsOnly.Substring(8, 2)}";
                }
            }
        }

        private void txtYear_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtYearEnd.Text) && !string.IsNullOrWhiteSpace(txtYear.Text))
            {
                if (int.TryParse(txtYear.Text, out int yearReceipts))
                {
                    txtYearEnd.Text = (yearReceipts + 4).ToString();
                }
            }
        }

        private void txtSurname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtName.Focus();
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPatronomyc.Focus();
            }
        }

        private void txtPatronomyc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dateCapacity.Focus();
            }
        }

        private void dateCapacity_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dateCapacity.SelectedDate.HasValue)
            {
                DateTime selectedDate = dateCapacity.SelectedDate.Value;
                if (selectedDate > DateTime.Now)
                {
                    MessageBox.Show("Дата рождения не может быть в будущем", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    dateCapacity.SelectedDate = DateTime.Now.AddYears(-18);
                }
            }
        }

        private void dateExpulsion_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dateExpulsion.SelectedDate.HasValue && dateCapacity.SelectedDate.HasValue)
            {
                if (dateExpulsion.SelectedDate.Value < dateCapacity.SelectedDate.Value)
                {
                    MessageBox.Show("Дата отчисления не может быть раньше даты рождения", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    dateExpulsion.SelectedDate = DateTime.Now;
                }
            }
        }
    }
}
