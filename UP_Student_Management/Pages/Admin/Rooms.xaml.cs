using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP_Student_Management.Classes.Context;
using UP_Student_Management.Classes.Context.StatusContext;
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Pages.Admin
{
    public partial class Rooms : Page
    {
        public Rooms()
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
                dataGridRooms.ItemsSource = new RoomContext().AllRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
        private void DatagridRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridRooms.SelectedItem is RoomContext selectedRoom)
            {
                LoadStudentsInRoom(selectedRoom.Id);
            }
            else
            {
                datagridStudentsInRoom.ItemsSource = null;
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Login());
        }

        private void gotoDepartments(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Admin.Departments());
        }

        private void gotoStudents(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPage(new Pages.Admin.Main());
        }

        private void btnAddRooms(object sender, RoutedEventArgs e)
        {
            var dialog = new Rooms_Add();
            dialog.Closed += (s, args) => updateList();
            dialog.ShowDialog();
        }

        private void btnEditRooms(object sender, RoutedEventArgs e)
        {
            if (dataGridRooms.SelectedItem is RoomContext selectedRoom)
            {
                var dialog = new Rooms_Edit(selectedRoom);
                dialog.Closed += (s, args) => updateList();
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите комнату для редактирования");
            }
        }

        private void btnDeleteRooms(object sender, RoutedEventArgs e)
        {
            if (dataGridRooms.SelectedItem is Room selectedRoom)
            {
                var result = MessageBox.Show($"Удалить комнату '{selectedRoom.Name}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) 
                {
                    try 
                    {
                        if (selectedRoom is RoomContext roomContext)
                        {
                            roomContext.Delete();
                        }
                        else 
                        {
                            MessageBox.Show("Не удалось преобразовать объект комнаты.");
                            return;
                        }
                        updateList();
                    } 
                    catch (Exception ex) 
                    {
                        MessageBox.Show($"Ошибка при удалении комнаты: {ex.Message}");
                    }
                }

            }
            else 
            {
                MessageBox.Show("Выберите комнату для удаления");
            }
        }
        private void LoadStudentsInRoom(int roomId)
        {
            try
            {
                var hostelContext = new HostelContext();
                var studentsInRoom = hostelContext.AllHostel().Where(h => h.RoomId == roomId).ToList();

                var studentContext = new StudentContext();
                var allStudents = studentContext.AllStudents();
                var allDepartments = new DepartmentContext().AllDepartments();

                var displayData = studentsInRoom
                    .Join(allStudents,
                        hostel => hostel.StudentId,
                        student => student.Id,
                        (hostel, student) => new StudentInRoomDisplay
                        {
                            FullName = $"{student.Surname} {student.Firstname} {student.Patronomyc}".Trim(),
                            GroupName = student.GroupName ?? "",
                            StartDate = hostel.StartDate,
                            EndDate = hostel.EndDate,
                            Status = hostel.Note ?? "Без статуса"
                        })
                    .ToList();

                datagridStudentsInRoom.ItemsSource = displayData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки студентов комнаты: {ex.Message}");
            }
        }
        private void createRecord(object sender, RoutedEventArgs e)
        {
            try
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = true;
                var workbook = excelApp.Workbooks.Add();
                var worksheet = workbook.Sheets[1];
                worksheet.Columns.AutoFit();
                // Заголовки отчета
                worksheet.Cells[1, 1] = "Пермский авиационный техникум им. А.Д. Швецова";
                worksheet.Cells[2, 1] = "База данных по воспитательной работе";
                worksheet.Cells[3, 1] = "Проживающие в общежитии";
                worksheet.Cells[6, 1] = $"на дату: {DateTime.Now:dd.MM.yyyy}";

                // Заголовки таблицы
                worksheet.Cells[8, 1] = "Комната";
                worksheet.Cells[8, 2] = "ФИО";
                worksheet.Cells[8, 3] = "Дата рождения";
                worksheet.Cells[8, 4] = "Группа";
                worksheet.Cells[8, 5] = "Контактный номер";
                worksheet.Cells[8, 6] = "Контакты родителей";
                worksheet.Cells[8, 7] = "Дата первого заселения";
                worksheet.Cells[8, 8] = "Общежитие (примечание)";
                worksheet.Cells[8, 9] = "Отчисление";

                var hostelContext = new HostelContext();
                var allHostel = hostelContext.AllHostel();

                var studentContext = new StudentContext();
                var allStudents = studentContext.AllStudents();

                var roomContext = new RoomContext();
                var allRooms = roomContext.AllRooms();

                int row = 9;
                int count = 0;
                foreach (var hostel in allHostel)
                {
                    var student = allStudents.FirstOrDefault(s => s.Id == hostel.StudentId);
                    var room = allRooms.FirstOrDefault(r => r.Id == hostel.RoomId);

                    if (student != null)
                    {
                        worksheet.Cells[row, 1] = room?.Name ?? "Неизвестно";
                        worksheet.Cells[row, 2] = $"{student.Surname} {student.Firstname} {student.Patronomyc}";
                        worksheet.Cells[row, 3] = student.BirthDate.ToString("dd.MM.yyyy");
                        worksheet.Cells[row, 4] = student.GroupName ?? "Неизвестно";
                        worksheet.Cells[row, 5] = student.Phone ?? "Неизвестно";
                        worksheet.Cells[row, 6] = "Неизвестно"; // Контакты родителей
                        worksheet.Cells[row, 7] = hostel.StartDate.ToString("dd.MM.yyyy");
                        worksheet.Cells[row, 8] = hostel.Note ?? "Неизвестно";
                        worksheet.Cells[row, 9] = "Неизвестно"; // Отчисление

                        row++;
                        count++;
                    }
                }

                // Итоги
                worksheet.Cells[row + 1, 1] = $"Количество записей: {count}";
                worksheet.Cells[row + 2, 1] = $"Дата печати: {DateTime.Now:dd.MM.yyyy}";

                worksheet.Columns.AutoFit();

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = System.IO.Path.Combine(desktopPath, "Отчет о студентах проживающих в общежитии.xlsx");
                workbook.SaveAs(filePath);
                workbook.Close();
                excelApp.Quit();

                MessageBox.Show("Отчет по общежитию создан на рабочем столе.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания отчета: {ex.Message}");
            }
        }
    }
}
