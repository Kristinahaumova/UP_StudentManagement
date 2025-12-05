using Microsoft.Win32;
using System;
using System.Windows;
using UP_Student_Management.Classes.Context;
using UP_Student_Management.Classes.Context.StatusContext;

namespace UP_Student_Management.Pages.Admin
{
    public partial class AddStatus : Window
    {
        private string selectedFilePath = string.Empty;
        private StudentContext student;
        public AddStatus(StudentContext student)
        {
            InitializeComponent();
            this.student = student;
            LoadStatuses();
            AttachEventHandlers();
        }
        private void LoadStatuses()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Сирота");
            cmbStatus.Items.Add("Инвалид");
            cmbStatus.Items.Add("ОВЗ");
            cmbStatus.Items.Add("Группа риска");
            cmbStatus.Items.Add("СВО");
            cmbStatus.Items.Add("СППП");
            cmbStatus.Items.Add("Общежитие");
            cmbStatus.Items.Add("Стипендия");
            cmbStatus.SelectedIndex = 0;
        }
        private void AttachEventHandlers()
        {
            cmbStatus.SelectionChanged += (s, e) => ShowStatusGrid();
        }
        private void ShowStatusGrid()
        {
            sirota.Visibility = Visibility.Hidden;
            invalid.Visibility = Visibility.Hidden;
            ovz.Visibility = Visibility.Hidden;
            riskGroup.Visibility = Visibility.Hidden;
            svo.Visibility = Visibility.Hidden;
            sppp.Visibility = Visibility.Hidden;
            hostel.Visibility = Visibility.Hidden;
            scholarship.Visibility = Visibility.Hidden;

            switch (cmbStatus.SelectedItem?.ToString())
            {
                case "Сирота":
                    sirota.Visibility = Visibility.Visible;
                    break;
                case "Инвалид":
                    invalid.Visibility = Visibility.Visible;
                    break;
                case "ОВЗ":
                    ovz.Visibility = Visibility.Visible;
                    break;
                case "Группа риска":
                    riskGroup.Visibility = Visibility.Visible;
                    break;
                case "СВО":
                    svo.Visibility = Visibility.Visible;
                    break;
                case "СППП":
                    sppp.Visibility = Visibility.Visible;
                    break;
                case "Общежитие":
                    hostel.Visibility = Visibility.Visible;
                    LoadRooms();
                    break;
                case "Стипендия":
                    scholarship.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void addFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
            }
        }

        private void deleteFile(object sender, RoutedEventArgs e)
        {
            selectedFilePath = string.Empty;
            MessageBox.Show("Файл удален из выбора");
        }

        private void btnAdd(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (cmbStatus.SelectedItem?.ToString())
                {
                    case "Сирота":
                        AddSirota();
                        break;
                    case "Инвалид":
                        AddInvalid();
                        break;
                    case "ОВЗ":
                        AddOvz();
                        break;
                    case "Группа риска":
                        AddRiskGroup();
                        break;
                    case "СВО":
                        AddSVO();
                        break;
                    case "СППП":
                        AddSPPP();
                        break;
                    case "Общежитие":
                        AddHostel();
                        break;
                    case "Стипендия":
                        AddScholarship();
                        break;
                    default:
                        MessageBox.Show("Выберите статус");
                        return;
                }

                MessageBox.Show("Статус успешно добавлен");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении статуса: {ex.Message}");
            }
        }
        private void AddSirota()
        {
            SirotaContext sirota = new SirotaContext
            {
                StudentId = student.Id, 
                Prikaz = txtStatus.Text,
                Note = txtNote.Text,
                DocumentPath = selectedFilePath,
                StartDate = dateStartStatus.SelectedDate ?? DateTime.Now,
                EndDate = dateEndStatus.SelectedDate
            };
            sirota.Save();
        }
        private void AddInvalid()
        {
            InvalidContext invalid = new InvalidContext
            {
                StudentId = student.Id,
                Prikaz = txtStatusInvalid.Text,
                Note = txtNoteInvalid.Text,
                InvalidType = txtTypeInvalid.Text,
                DocumentPath = selectedFilePath,
                StartDate = dateStartStatusInvalid.SelectedDate ?? DateTime.Now,
                EndDate = dateEndStatusInvalid.SelectedDate
            };
            invalid.Save();
        }
        private void AddOvz()
        {
            OvzContext ovz = new OvzContext
            {
                StudentId = student.Id,
                Prikaz = txtStatusOvz.Text,
                Note = txtNoteOvz.Text,
                DocumentPath = selectedFilePath,
                StartDate = dateStartStatusOvz.SelectedDate ?? DateTime.Now,
                EndDate = dateEndStatusOvz.SelectedDate
            };
            ovz.Save();
        }
        private void AddRiskGroup()
        {
            RiskGroupContext riskGroup = new RiskGroupContext
            {
                StudentId = student.Id,
                Type = cmbTypeRisk.SelectedItem?.ToString(),
                Note = txtNoteRisk.Text,
                RegistrationOsnovanie = txtOsnovanieStart.Text,
                RemovalOsnovanie = txtOsnovanieEnd.Text,
                RegistrationReason = txtReasonStart.Text,
                RemovalReason = txtReasonEnd.Text,
                DocumentPath = selectedFilePath,
                StartDate = dateStartStatusRisk.SelectedDate ?? DateTime.Now,
                EndDate = dateEndStatusRisk.SelectedDate
            };
            riskGroup.Save();
        }
        private void AddSVO()
        {
            SVOContext svo = new SVOContext
            {
                StudentId = student.Id,
                Prikaz = txtStatusSVO.Text,
                DocumentPath = selectedFilePath,
                StartDate = dateStartStatusSVO.SelectedDate ?? DateTime.Now,
                EndDate = dateEndStatusSVO.SelectedDate
            };
            svo.Save();
        }
        private void AddSPPP()
        {
            SPPPContext sppp = new SPPPContext
            {
                StudentId = student.Id,
                CallReason = txtOsnovanieSPPP.Text,
                Note = txtNoteSPPP.Text,
                EmployeesPresent = txtEmployeesSPPP.Text,
                RepresentativesPresent = txtRepresentative.Text,
                ReasonCall = txtReasonSPPP.Text,
                Decision = txtConclusion.Text,
                DocumentPath = selectedFilePath,
                Date = dateSPPP.SelectedDate ?? DateTime.Now
            };
            sppp.Save();
        }
        private void AddHostel()
        {
            if (cmbRoom.SelectedItem is RoomContext room)
            {
                HostelContext hostel = new HostelContext
                {
                    StudentId = student.Id,
                    RoomId = room.Id,
                    Note = txtNoteHostel.Text,
                    DocumentPath = selectedFilePath,
                    StartDate = dateStartHostel.SelectedDate ?? DateTime.Now,
                    EndDate = dateEndHostel.SelectedDate
                };
                hostel.Save();
            }
            else
            {
                MessageBox.Show("Выберите комнату");
            }
        }
        private void LoadRooms()
        {
            cmbRoom.Items.Clear();
            var roomContext = new RoomContext();
            var rooms = roomContext.AllRooms();
            foreach (var room in rooms)
            {
                cmbRoom.Items.Add(room);
            }
            cmbRoom.DisplayMemberPath = "Name";
            cmbRoom.SelectedValuePath = "Id";
            if (cmbRoom.Items.Count > 0)
                cmbRoom.SelectedIndex = 0;
        }

        private void AddScholarship()
        {
            ScholarshipContext scholarship = new ScholarshipContext
            {
                StudentId = student.Id,
                Prikaz = txtDocumentScholarship.Text,
                DocumentPath = selectedFilePath,
                StartDate = dateStartScholarship.SelectedDate ?? DateTime.Now,
                EndDate = dateEndScholarship.SelectedDate
            };
            scholarship.Save();
        }
        private void btnCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
