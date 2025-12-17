using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Pages.Admin
{
    // Класс для хранения информации о выбранных файлах
    public class SelectedFile
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }

    public partial class Students_Add : Window
    {
        private ObservableCollection<SelectedFile> selectedFiles = new ObservableCollection<SelectedFile>();

        public Students_Add()
        {
            InitializeComponent();
            LoadComboBoxData();
            AttachEventHandlers();

            // Привязываем коллекцию файлов к ListBox
            lstFiles.ItemsSource = selectedFiles;
        }

        private void AttachEventHandlers()
        {
            txtYear.PreviewTextInput += txtYear_PreviewTextInput;
            txtYearEnd.PreviewTextInput += txtYearEnd_PreviewTextInput;
            txtPhoneNumber.PreviewTextInput += txtPhoneNumber_PreviewTextInput;

            // Валидация для фамилии, имени, отчества
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

        private void LoadComboBoxData()
        {
            try
            {
                cmbEducation.Items.Add("9 классов");
                cmbEducation.Items.Add("11 классов");
                cmbEducation.SelectedIndex = 1;

                cmbGender.Items.Add("М");
                cmbGender.Items.Add("Ж");
                cmbGender.SelectedIndex = 0;

                cmbFinance.Items.Add("Бюджет");
                cmbFinance.Items.Add("Контракт");
                cmbFinance.SelectedIndex = 0;

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
                var departments = departmentContext.AllDepartments().ToList(); // Преобразуем в List

                cmbDepartments.Items.Clear();

                if (!departments.Any())
                {
                    MessageBox.Show("В базе данных нет отделений. Добавьте отделения перед созданием студентов.",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                foreach (var department in departments)
                {
                    cmbDepartments.Items.Add(department);
                }

                cmbDepartments.DisplayMemberPath = "Name";
                cmbDepartments.SelectedValuePath = "Id";

                if (cmbDepartments.Items.Count > 0)
                    cmbDepartments.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отделений: {ex.Message}\n" +
                               "Проверьте подключение к базе данных и наличие таблицы Departments.",
                               "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                        // Проверяем, не добавлен ли уже этот файл
                        if (!selectedFiles.Any(f => f.FilePath == filePath))
                        {
                            selectedFiles.Add(new SelectedFile
                            {
                                FilePath = filePath,
                                FileName = Path.GetFileName(filePath),
                                FileExtension = Path.GetExtension(filePath)
                            });
                        }
                    }

                    UpdateFileListDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выбора файлов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClearFiles_Click(object sender, RoutedEventArgs e)
        {
            ClearAllFiles();
        }

        private void ClearAllFiles()
        {
            if (selectedFiles.Count > 0)
            {
                var result = MessageBox.Show($"Удалить все выбранные файлы ({selectedFiles.Count})?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    selectedFiles.Clear();
                    UpdateFileListDisplay();
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
            if (sender is System.Windows.Controls.Button button && button.Tag is SelectedFile file)
            {
                selectedFiles.Remove(file);
                UpdateFileListDisplay();
            }
        }

        private void UpdateFileListDisplay()
        {
            if (selectedFiles.Count == 0)
            {
                // Можно добавить текст "Файлы не выбраны" или оставить пустую область
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddStudent();
        }

        private void AddStudent()
        {
            try
            {
                if (!ValidateForm())
                    return;
                StudentContext student = CreateStudentFromForm();

                // ПРОВЕРКА 1: Получить все отделения из БД
                var departmentContext = new DepartmentContext();
                var allDepartments = departmentContext.AllDepartments().ToList();

                // Показать все отделения для отладки
                string departmentsInfo = "Отделения в базе данных:\n";
                foreach (var dept in allDepartments)
                {
                    departmentsInfo += $"ID: {dept.Id}, Название: {dept.Name}\n";
                }

                // ПРОВЕРКА 2: Конкретно проверить отделение с Id = 2
                bool departmentExists = allDepartments.Any(d => d.Id == student.DepartmentId);

                if (!departmentExists)
                {
                    return;
                }

                // Сохраняем студента в базу
                student.Save(false);

                // Сохраняем все выбранные файлы
                if (selectedFiles.Count > 0)
                {
                    foreach (var file in selectedFiles)
                    {
                        try
                        {
                            // Используем упрощенный метод
                            string savedPath = student.SaveStudentFile(file.FilePath);
                            if (!string.IsNullOrEmpty(savedPath))
                            {
                                // Файл успешно сохранен
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при сохранении файла {file.FileName}: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }

                MessageBox.Show($"Студент {student.Surname} {student.Firstname} успешно добавлен!\n" +
                               $"Прикреплено файлов: {selectedFiles.Count}",
                               "Успешно",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);

                ClearForm();

                this.DialogResult = true;
                this.Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка валидации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении студента: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                // Для отладки можно вывести внутреннее исключение
                if (ex.InnerException != null)
                {
                    MessageBox.Show($"Внутренняя ошибка: {ex.InnerException.Message}", "Детали ошибки",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private bool ValidateForm()
        {
            // Валидация фамилии
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

            // Валидация имени
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

            // Валидация отчества (необязательное поле)
            if (!string.IsNullOrWhiteSpace(txtPatronomyc.Text) && !IsValidName(txtPatronomyc.Text, "отчества"))
            {
                MessageBox.Show("Отчество должно содержать только русские буквы и начинаться с заглавной буквы", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtPatronomyc.Focus();
                return false;
            }

            // Валидация даты рождения
            if (dateCapacity.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите дату рождения", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                dateCapacity.Focus();
                return false;
            }

            // Валидация отделения
            if (cmbDepartments.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите отделение", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbDepartments.Focus();
                return false;
            }

            // ВАЛИДАЦИЯ ГРУППЫ
            if (string.IsNullOrWhiteSpace(txtGroup.Text))
            {
                MessageBox.Show("Пожалуйста, укажите группу студента", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtGroup.Focus();
                return false;
            }

            // Валидация года поступления
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

            // Валидация года окончания
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


        // Метод для валидации имени, фамилии, отчества
        private bool IsValidName(string name, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Проверяем, что строка состоит только из русских букв и дефиса
            // и начинается с заглавной буквы
            Regex regex = new Regex(@"^[А-ЯЁ][а-яё\-]+$");
            return regex.IsMatch(name);
        }

        // Метод для валидации номера телефона
        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true; // Пустой номер - допустимо

            // Убираем все нецифровые символы
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            // Проверяем российские форматы номеров
            if (digitsOnly.Length == 11)
            {
                // Форматы: 7XXXXXXXXXX или 8XXXXXXXXXX
                return digitsOnly.StartsWith("7") || digitsOnly.StartsWith("8");
            }
            else if (digitsOnly.Length == 10)
            {
                // Формат: XXXXXXXXXX (без кода страны)
                return true;
            }

            return false;
        }

        private StudentContext CreateStudentFromForm()
        {
            int yearReceipts = int.Parse(txtYear.Text);
            int yearFinish = int.Parse(txtYearEnd.Text);

            // ПРОВЕРКА: Убедимся, что отделение выбрано и имеет корректный Id
            if (cmbDepartments.SelectedItem == null)
            {
                throw new ArgumentException("Не выбрано отделение");
            }

            DepartmentContext selectedDepartment = (DepartmentContext)cmbDepartments.SelectedItem;

            // Дополнительная проверка Id
            if (selectedDepartment.Id <= 0)
            {
                throw new ArgumentException("Некорректный ID отделения");
            }

            return new StudentContext
            {
                Surname = txtSurname.Text.Trim(),
                Firstname = txtName.Text.Trim(),
                BirthDate = dateCapacity.SelectedDate.Value,
                Patronomyc = txtPatronomyc.Text.Trim(),
                Phone = FormatPhoneNumber(txtPhoneNumber.Text.Trim()),
                Sex = cmbGender.SelectedItem?.ToString() ?? "М",
                Education = cmbEducation.SelectedIndex == 0 ? "9" : "11",
                GroupName = txtGroup.Text.Trim(),
                isBudget = cmbFinance.SelectedIndex == 0 ? 1 : 0,
                YearReceipts = new DateTime(yearReceipts, 1, 1),
                YearFinish = new DateTime(yearFinish, 1, 1),
                DeductionsInfo = txtExpulsion.Text.Trim(),
                DataDeductions = dateExpulsion.SelectedDate ?? DateTime.MinValue,
                Note = txtDescription.Text.Trim(),
                ParentsInfo = txtParentsInfo.Text.Trim(),
                Penalties = txtPenalties.Text.Trim(),
                DepartmentId = selectedDepartment.Id // Теперь здесь гарантированно корректное значение
            };
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

        private string CopyStudentFile(int studentId, string sourceFilePath)
        {
            try
            {
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "UP_Student_Management",
                    "StudentFiles",
                    studentId.ToString()
                );

                Directory.CreateDirectory(appDataPath);

                string fileName = Path.GetFileName(sourceFilePath);
                string destinationPath = Path.Combine(appDataPath, fileName);

                // Если файл уже существует, добавляем временную метку
                if (File.Exists(destinationPath))
                {
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string extension = Path.GetExtension(fileName);
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    fileName = $"{fileNameWithoutExt}_{timestamp}{extension}";
                    destinationPath = Path.Combine(appDataPath, fileName);
                }

                File.Copy(sourceFilePath, destinationPath, false);
                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при копировании файла {Path.GetFileName(sourceFilePath)}: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return string.Empty;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelForm();
        }

        private void CancelForm()
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ClearForm()
        {
            txtSurname.Clear();
            txtName.Clear();
            txtPatronomyc.Clear();
            dateCapacity.SelectedDate = null;
            cmbGender.SelectedIndex = 0;
            txtPhoneNumber.Clear();
            cmbEducation.SelectedIndex = 1;
            txtGroup.Clear();
            cmbFinance.SelectedIndex = 0;
            txtYear.Clear();
            txtYearEnd.Clear();
            if (cmbDepartments.Items.Count > 0)
                cmbDepartments.SelectedIndex = 0;
            txtExpulsion.Clear();
            dateExpulsion.SelectedDate = null;
            txtDescription.Clear();
            txtParentsInfo.Clear();
            txtPenalties.Clear();
            selectedFiles.Clear();
        }

        // Обработчики событий для валидации ввода
        private void txtSurname_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только русские буквы и дефис
            Regex regex = new Regex(@"^[а-яА-ЯёЁ\-]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void txtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только русские буквы и дефис
            Regex regex = new Regex(@"^[а-яА-ЯёЁ\-]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void txtPatronomyc_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только русские буквы и дефис
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
            // Разрешаем цифры, плюс, скобки, дефис и пробел
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