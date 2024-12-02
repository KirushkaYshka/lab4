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
    /// <summary>
    /// Логика взаимодействия для AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        private Bookings request = new Bookings();
        private Equipment equipment = new Equipment();
        private Members client = new Members();
        private TrainingTypes faultType = new TrainingTypes();
        private Trainers trainers = new Trainers();
        public AddEditWindow()
        {
            InitializeComponent();
            DataContext = request;
            ComboStatus.ItemsSource = YshakoffEntities.GetContext().BookingStatus.ToList();
        }
        private void BtnSave_Click(Object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();

            if (request.equipment_id == null)
                error.AppendLine("Введите номер заявки!");
            else if (!int.TryParse(request.equipment_id.ToString(),
            out int applicationNumber) || applicationNumber <= 0)
                error.AppendLine("Номер заявки должен иметь положительное и не отрицательное значение!");
            if (YshakoffEntities.GetContext().Members.Any(row => row.member_id == request.member_id))
                error.AppendLine("Номер заявки уже существует!");
            if (ComboStatus.SelectedItem != null && ComboStatus.SelectedItem is BookingStatus selectedStatus)
                request.status_id = selectedStatus.status_id;
            else error.AppendLine("Выберите статус!");
            if (string.IsNullOrWhiteSpace(EquipmentTextBox.Text))
                error.AppendLine("Укажите оборудование!");
            if (string.IsNullOrWhiteSpace(ClientTextBox.Text))
                error.AppendLine("Укажите клиента!");
            if (string.IsNullOrWhiteSpace(FaultTypeTextBox.Text))
                error.AppendLine("Укажите имя тренера!");

            if (error.Length > 0)
            {
                MessageBox.Show(error.ToString(), "Предупреждение!", MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }
            try
            {
                var context = YshakoffEntities.GetContext();

                equipment.equipment_name = EquipmentTextBox.Text;
                client.member_name = ClientTextBox.Text;
                faultType.training_type_name = EquipmentTextBox.Text;
                trainers.trainer_name = FaultTypeTextBox.Text;

                context.Trainers.Add(trainers);
                context.Equipment.Add(equipment);
                context.Members.Add(client);
                context.TrainingTypes.Add(faultType);
                context.SaveChanges();

                var trainersId = trainers.trainer_id;
                var equipmentId = equipment.equipment_id;
                var clientId = client.member_id;
                var faultTypeId = faultType.training_type_id;

                request.trainer_id = trainersId;
                request.equipment_id = equipmentId;
                request.member_id = clientId;
                request.training_type_id = faultTypeId;

                context.Bookings.Add(request);
                context.SaveChanges();
                MessageBox.Show("Информация сохранена");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}

