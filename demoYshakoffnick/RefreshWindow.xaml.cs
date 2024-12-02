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
using System.Windows.Shapes;

namespace demoYshakoffnick
{
    public partial class RefreshWindow : Window
    {
        private Bookings _currentRequest;
        public RefreshWindow(Bookings request)
        {
            InitializeComponent();
            _currentRequest = request;
            StatusComboBox.ItemsSource = YshakoffEntities.GetContext().BookingStatus.ToList();
            WorkerComboBox.ItemsSource = YshakoffEntities.GetContext().Trainers.ToList();
            FaultTypeComboBox.ItemsSource = YshakoffEntities.GetContext().TrainingTypes.ToList();
            ClientComboBox.ItemsSource = YshakoffEntities.GetContext().Equipment.ToList();
            // Презагрузкаданных
            StatusComboBox.SelectedItem = request.BookingStatus;
            WorkerComboBox.SelectedItem = request.Trainers;
            FaultTypeComboBox.SelectedItem = request.TrainingTypes;
            ClientComboBox.SelectedItem = request.Equipment;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Здесь код для обновления данных в базе
            var context = YshakoffEntities.GetContext();
            _currentRequest.status_id = ((BookingStatus)StatusComboBox.SelectedItem).status_id;
            _currentRequest.trainer_id = ((Trainers)WorkerComboBox.SelectedItem).trainer_id;
            _currentRequest.training_type_id = ((TrainingTypes)FaultTypeComboBox.SelectedItem).training_type_id;
            _currentRequest.member_id = ((Equipment)ClientComboBox.SelectedItem).equipment_id;
            context.SaveChanges();
            MessageBox.Show("Данные заявки обновлены");
            this.Close();
        }
    }
}

