using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DoctorBookingWPF
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Patient> patients = new();
        private ObservableCollection<Doctor> doctors = new();
        private ObservableCollection<Appointment> appointments = new();

        private int nextPatientId = 1;
        private int nextDoctorId = 1;
        private int nextAppointmentId = 1;

        public MainWindow()
        {
            InitializeComponent();

            patients.Add(new Patient
            {
                Id = nextPatientId++,
                FullName = "Иванов Иван",
                Phone = "89990000001"
            });

            doctors.Add(new Doctor
            {
                Id = nextDoctorId++,
                FullName = "Петров Пётр",
                Specialization = "Терапевт",
                Cabinet = "101"
            });

            RefreshTables();
        }

        private void RefreshTables()
        {
            PatientsGrid.ItemsSource = null;
            PatientsGrid.ItemsSource = patients;

            DoctorsGrid.ItemsSource = null;
            DoctorsGrid.ItemsSource = doctors;

            AppointmentsGrid.ItemsSource = null;
            AppointmentsGrid.ItemsSource = appointments;
        }

        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            string name = PatientNameBox.Text.Trim();
            string phone = PatientPhoneBox.Text.Trim();

            if (name == "" || phone == "")
            {
                MessageBox.Show("Введите ФИО и телефон пациента.");
                return;
            }

            patients.Add(new Patient
            {
                Id = nextPatientId++,
                FullName = name,
                Phone = phone
            });

            PatientNameBox.Clear();
            PatientPhoneBox.Clear();
            RefreshTables();
            StatusText.Text = "Пациент добавлен.";
        }

        private void DeletePatient_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsGrid.SelectedItem is not Patient patient)
            {
                MessageBox.Show("Выберите пациента в таблице.");
                return;
            }

            if (appointments.Any(a => a.PatientId == patient.Id))
            {
                MessageBox.Show("Нельзя удалить пациента, у которого есть записи.");
                return;
            }

            patients.Remove(patient);
            RefreshTables();
            StatusText.Text = "Пациент удалён.";
        }

        private void PatientSearch_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            if (PatientsGrid == null)
                return;

            string search = PatientSearchBox.Text.Trim();

            PatientsGrid.ItemsSource = patients
                .Where(p => p.FullName.Contains(
                    search, StringComparison.OrdinalIgnoreCase)
                    || p.Phone.Contains(
                    search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void AddDoctor_Click(object sender, RoutedEventArgs e)
        {
            string name = DoctorNameBox.Text.Trim();
            string specialty = DoctorSpecialtyBox.Text.Trim();
            string cabinet = DoctorCabinetBox.Text.Trim();

            if (name == "" || specialty == "" || cabinet == "")
            {
                MessageBox.Show("Заполните все поля врача.");
                return;
            }

            doctors.Add(new Doctor
            {
                Id = nextDoctorId++,
                FullName = name,
                Specialization = specialty,
                Cabinet = cabinet
            });

            DoctorNameBox.Clear();
            DoctorSpecialtyBox.Clear();
            DoctorCabinetBox.Clear();

            RefreshTables();
            StatusText.Text = "Врач добавлен.";
        }

        private void
            AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(AppointmentPatientIdBox.Text, out int patientId)
                || !int.TryParse(AppointmentDoctorIdBox.Text, out int doctorId))
            {
                MessageBox.Show("Введите числовые ID пациента и врача.");
                return;
            }

            if (!patients.Any(p => p.Id == patientId)
                || !doctors.Any(d => d.Id == doctorId))
            {
                MessageBox.Show("Пациент или врач с таким ID не найден.");
                return;
            }

            if (!DateTime.TryParse(
                AppointmentDateBox.Text, out DateTime date))
            {
                MessageBox.Show("Введите дату и время, например 29.10.2026 14:30.");
                return;
            }

            if (appointments.Any(a =>
                a.DoctorId == doctorId &&
                a.Date == date &&
                a.Status != "Отменён"))
            {
                MessageBox.Show("Врач уже занят в это время.");
                return;
            }

            appointments.Add(new Appointment
            {
                Id = nextAppointmentId++,
                PatientId = patientId,
                DoctorId = doctorId,
                PatientName = patients.First(p => p.Id == patientId).FullName,
                DoctorName = doctors.First(d => d.Id == doctorId).FullName,
                Date = date,
                Status = "Запланирован"
            });

            RefreshTables();
            StatusText.Text = "Запись создана.";
        }

        private void CancelAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentsGrid.SelectedItem is not Appointment appointment)
            {
                MessageBox.Show("Выберите запись в таблице.");
                return;
            }

            appointment.Status = "Отменён";
            RefreshTables();
            StatusText.Text = "Запись отменена.";
        }
    }

    public class Patient
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
    }

    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Specialization { get; set; } = "";
        public string Cabinet { get; set; } = "";
    }

    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string PatientName { get; set; } = "";
        public string DoctorName { get; set; } = "";
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Запланирован";
    }
}