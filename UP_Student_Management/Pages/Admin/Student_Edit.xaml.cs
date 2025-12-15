using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Pages.Admin
{
    public class StudentFileItem
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public bool IsExistingFile { get; set; }
    }

    public class StudentFileContext
    {
        private string connectionString = "server=localhost;port=3307;database=StudentManagement;user=root;";
        public List<StudentFileItem> GetFilesByStudentId(int studentId)
        {
            List<StudentFileItem> files = new List<StudentFileItem>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT FilePath FROM StudentFiles WHERE IdStudent = @StudentId";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StudentId", studentId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            int counter = 1;
                            while (reader.Read())
                            {
                                string filePath = reader["FilePath"].ToString();

                                files.Add(new StudentFileItem
                                {
                                    Id = counter,
                                    FilePath = filePath,
                                    FileName = Path.GetFileName(filePath),
                                    FileExtension = Path.GetExtension(filePath),
                                    IsExistingFile = true
                                });
                                counter++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файлов для студента {studentId}: {ex.Message}");
            }

            return files;
        }

        // Добавить файл для студента
        public bool AddFile(int studentId, string filePath)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Проверяем, не добавлен ли уже этот файл
                    string checkQuery = "SELECT COUNT(*) FROM StudentFiles WHERE IdStudent = @StudentId AND FilePath = @FilePath";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@StudentId", studentId);
                        checkCommand.Parameters.AddWithValue("@FilePath", filePath);

                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                        if (count > 0)
                        {
                            return false; // Файл уже существует
                        }
                    }

                    // Добавляем путь в БД
                    string insertQuery = "INSERT INTO StudentFiles (IdStudent, FilePath) VALUES (@StudentId, @FilePath)";
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@StudentId", studentId);
                        command.Parameters.AddWithValue("@FilePath", filePath);
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении файла: {ex.Message}");
                return false;
            }
        }

        // Удалить файл
        public bool DeleteFile(int studentId, string filePath)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM StudentFiles WHERE IdStudent = @StudentId AND FilePath = @FilePath";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StudentId", studentId);
                        command.Parameters.AddWithValue("@FilePath", filePath);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении файла из БД: {ex.Message}");
                return false;
            }
        }
    }

    public partial class Student_Edit : Window
    {
        private StudentContext student;
        private ObservableCollection<StudentFileItem> studentFiles = new ObservableCollection<StudentFileItem>();
        private List<int> filesToDelete = new List<int>();

        public Student_Edit(StudentContext student)
        {
            InitializeComponent();
            this.student = student;
            LoadComboBoxData();
            LoadStudentData();
            LoadStudentFiles();

            AttachEventHandlers();

            lstFiles.ItemsSource = studentFiles;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateForm())
                    return;

                UpdateStudentFromForm();

                // Сохраняем изменения студента
                student.Save(true);

                // Удаляем файлы, помеченные на удаление
                DeleteMarkedFiles();

                // Добавляем новые файлы
                SaveNewFiles();

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnChooseFiles_Click(object sender, RoutedEventArgs e)
        {
            ChooseFiles();
        }

        private void ChooseFiles()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter = "Все файлы (*.*)|*.*|" +
                            "Документы (*.pdf;*.doc;*.docx;*.txt;*.rtf)|*.pdf;*.doc;*.docx;*.txt;*.rtf|" +
                            "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|" +
                            "Архивы (*.zip;*.rar;*.7z)|*.zip;*.rar;*.7z",
                    FilterIndex = 2
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        if (!studentFiles.Any(f => f.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)))
                        {
                            studentFiles.Add(new StudentFileItem
                            {
                                Id = GetNextFileId(),
                                FilePath = filePath,
                                FileName = Path.GetFileName(filePath),
                                FileExtension = Path.GetExtension(filePath),
                                IsExistingFile = false
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выбора файлов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetNextFileId()
        {
            return studentFiles.Count > 0 ? studentFiles.Max(f => f.Id) + 1 : 1;
        }

        private void btnClearFiles_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFiles();
        }

        private void ClearAllFiles()
        {
            if (studentFiles.Count > 0)
            {
                var result = MessageBox.Show($"Удалить все выбранные файлы ({studentFiles.Count})?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Помечаем существующие файлы на удаление
                    foreach (var file in studentFiles.Where(f => f.IsExistingFile))
                    {
                        if (!filesToDelete.Contains(file.Id))
                        {
                            filesToDelete.Add(file.Id);
                        }
                    }

                    studentFiles.Clear();
                }
            }
            else
            {
                MessageBox.Show("Файлы не выбраны", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.Tag is StudentFileItem file)
            {
                var result = MessageBox.Show($"Удалить файл '{file.FileName}'?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (file.IsExistingFile && !filesToDelete.Contains(file.Id))
                    {
                        filesToDelete.Add(file.Id);
                    }

                    studentFiles.Remove(file);
                }
            }
        }

        private void LoadComboBoxData()
        {
            try
            {
                cmbEducation.Items.Clear();
                cmbEducation.Items.Add("9 классов");
                cmbEducation.Items.Add("11 классов");
                cmbEducation.SelectedIndex = student.Education == "9" ? 0 : 1;

                cmbGender.Items.Clear();
                cmbGender.Items.Add("М");
                cmbGender.Items.Add("Ж");
                cmbGender.SelectedIndex = student.Sex == "М" ? 0 : 1;

                cmbFinance.Items.Clear();
                cmbFinance.Items.Add("Бюджет");
                cmbFinance.Items.Add("Контракт");
                cmbFinance.SelectedIndex = student.isBudget == 1 ? 0 : 1;

                LoadDepartments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDepartments()
        {
            try
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

                var studentDepartment = departments.FirstOrDefault(d => d.Id == student.DepartmentId);
                if (studentDepartment != null)
                {
                    cmbDepartments.SelectedItem = studentDepartment;
                }
                else if (cmbDepartments.Items.Count > 0)
                {
                    cmbDepartments.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отделений: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            if (student.DataDeductions != DateTime.MinValue && student.DataDeductions.Year > 1900)
            {
                dateExpulsion.SelectedDate = student.DataDeductions;
            }

            txtDescription.Text = student.Note;
            txtParentsInfo.Text = student.ParentsInfo;
            txtPenalties.Text = student.Penalties;
        }

        private void LoadStudentFiles()
        {
            try
            {
                var studentFileContext = new StudentFileContext();
                var existingFiles = studentFileContext.GetFilesByStudentId(student.Id);

                studentFiles.Clear();

                foreach (var file in existingFiles)
                {
                    studentFiles.Add(new StudentFileItem
                    {
                        Id = file.Id,
                        FilePath = file.FilePath,
                        FileName = file.FileName,
                        FileExtension = file.FileExtension,
                        IsExistingFile = true
                    });
                }

                // Для отладки
                if (existingFiles.Count > 0)
                {
                    MessageBox.Show($"Загружено {existingFiles.Count} файлов для студента ID: {student.Id}");
                }
            }
            catch (Exception ex)
            {
                // Если не удалось загрузить файлы, просто продолжаем
                MessageBox.Show($"Не удалось загрузить файлы из БД: {ex.Message}");
            }
        }

        private void AttachEventHandlers()
        {
            txtYear.PreviewTextInput += txtYear_PreviewTextInput;
            txtYearEnd.PreviewTextInput += txtYearEnd_PreviewTextInput;
            txtPhoneNumber.PreviewTextInput += txtPhoneNumber_PreviewTextInput;

            txtSurname.PreviewTextInput += txtSurname_PreviewTextInput;
            txtName.PreviewTextInput += txtName_PreviewTextInput;
            txtPatronomyc.PreviewTextInput += txtPatronomyc_PreviewTextInput;

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

            if (!IsValidName(txtSurname.Text, "фамилии"))
            {
                MessageBox.Show("Фамилия должна содержать только русские буквы и начинаться с заглавной буквы", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (!IsValidName(txtName.Text, "имени"))
            {
                MessageBox.Show("Имя должно содержать только русские буквы и начинаться с заглавной буквы", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtName.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPatronomyc.Text) && !IsValidName(txtPatronomyc.Text, "отчества"))
            {
                MessageBox.Show("Отчество должно содержать только русские буквы и начинаться с заглавной буквы", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtPatronomyc.Focus();
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

            if (!int.TryParse(txtYear.Text, out int yearReceipts) || yearReceipts < 1900 || yearReceipts > DateTime.Now.Year)
            {
                MessageBox.Show("Год поступления должен быть числом от 1900 до текущего года", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (!IsValidPhoneNumber(phone))
                {
                    MessageBox.Show("Номер телефона должен быть в формате +7 (XXX) XXX-XX-XX или 8XXXXXXXXXX", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    txtPhoneNumber.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool IsValidName(string name, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            Regex regex = new Regex(@"^[А-ЯЁ][а-яё\-]+$");
            return regex.IsMatch(name);
        }

        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true;

            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length == 11)
            {
                return digitsOnly.StartsWith("7") || digitsOnly.StartsWith("8");
            }
            else if (digitsOnly.Length == 10)
            {
                return true;
            }

            return false;
        }

        private void UpdateStudentFromForm()
        {
            int yearReceipts = int.Parse(txtYear.Text);
            int yearFinish = int.Parse(txtYearEnd.Text);
            DepartmentContext selectedDepartment = (DepartmentContext)cmbDepartments.SelectedItem;

            student.Surname = txtSurname.Text.Trim();
            student.Firstname = txtName.Text.Trim();
            student.BirthDate = dateCapacity.SelectedDate.Value;
            student.Patronomyc = txtPatronomyc.Text.Trim();
            student.Phone = FormatPhoneNumber(txtPhoneNumber.Text.Trim());
            student.Sex = cmbGender.SelectedItem?.ToString() ?? "М";
            student.Education = cmbEducation.SelectedIndex == 0 ? "9" : "11";
            student.GroupName = txtGroup.Text.Trim();
            student.isBudget = cmbFinance.SelectedIndex == 0 ? 1 : 0;
            student.YearReceipts = new DateTime(yearReceipts, 1, 1);
            student.YearFinish = new DateTime(yearFinish, 1, 1);
            student.DeductionsInfo = txtExpulsion.Text.Trim();

            if (dateExpulsion.SelectedDate.HasValue)
            {
                student.DataDeductions = dateExpulsion.SelectedDate.Value;
            }
            else
            {
                student.DataDeductions = DateTime.MinValue;
            }

            student.Note = txtDescription.Text.Trim();
            student.ParentsInfo = txtParentsInfo.Text.Trim();
            student.Penalties = txtPenalties.Text.Trim();
            student.DepartmentId = selectedDepartment.Id;
        }

        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return phone;

            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length == 11)
            {
                if (digitsOnly.StartsWith("8"))
                {
                    digitsOnly = "7" + digitsOnly.Substring(1);
                }
                return $"+7 ({digitsOnly.Substring(1, 3)}) {digitsOnly.Substring(4, 3)}-{digitsOnly.Substring(7, 2)}-{digitsOnly.Substring(9, 2)}";
            }
            else if (digitsOnly.Length == 10)
            {
                return $"+7 ({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3, 3)}-{digitsOnly.Substring(6, 2)}-{digitsOnly.Substring(8, 2)}";
            }

            return phone;
        }

        private void SaveNewFiles()
        {
            var studentFileContext = new StudentFileContext();
            var newFiles = studentFiles.Where(f => !f.IsExistingFile).ToList();

            foreach (var file in newFiles)
            {
                try
                {
                    if (studentFileContext.AddFile(student.Id, file.FilePath))
                    {
                        Console.WriteLine($"Путь к файлу сохранен в БД: {file.FilePath}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении информации о файле {file.FileName}: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void DeleteMarkedFiles()
        {
            if (filesToDelete.Count > 0)
            {
                var studentFileContext = new StudentFileContext();
                var filesToRemove = studentFiles
                    .Where(f => filesToDelete.Contains(f.Id) && f.IsExistingFile)
                    .ToList();

                foreach (var file in filesToRemove)
                {
                    try
                    {
                        // Удаляем только запись из БД, файл на диске остается
                        if (studentFileContext.DeleteFile(student.Id, file.FilePath))
                        {
                            Console.WriteLine($"Запись о файле удалена из БД: {file.FilePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении информации о файле {file.FileName}: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                filesToDelete.Clear();
            }
        }

        // Обработчики событий
        private void txtSurname_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[а-яА-ЯёЁ\-]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void txtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[а-яА-ЯёЁ\-]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void txtPatronomyc_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[а-яА-ЯёЁ\-]$");
            e.Handled = !regex.IsMatch(e.Text);
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
            Regex regex = new Regex(@"^[\d\+\-\(\)\s]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void txtPhoneNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            string phone = txtPhoneNumber.Text.Trim();
            if (!string.IsNullOrEmpty(phone))
            {
                txtPhoneNumber.Text = FormatPhoneNumber(phone);
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
            if (e.Key == Key.Enter) txtName.Focus();
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) txtPatronomyc.Focus();
        }

        private void txtPatronomyc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) dateCapacity.Focus();
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