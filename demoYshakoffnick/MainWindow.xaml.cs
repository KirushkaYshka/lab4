using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshTechnoServiceDataGrid();
            ComboStatus.ItemsSource = YshakoffEntities.GetContext().BookingStatus.ToList();
            Box.Text = YshakoffEntities.GetContext().Bookings.Count(r => r.status_id == 2).ToString();
            Vis();
        }

        private void RefreshTechnoServiceDataGrid()
        {
            var context = YshakoffEntities.GetContext();
            var requestsWithRelations = context.Bookings
            .Include(r => r.Equipment)
            .Include(r => r.TrainingTypes)
            .Include(r => r.Members)
            .Include(r => r.Trainers)
            .ToList();

            demoYshakoffnick.ItemsSource = requestsWithRelations;
        }

        private void Vis()
        {
            switch (Authorization.authorizationRole)
            {
                case "Админ":
                    break;
                case "Препод":
                    BtnDelet.Visibility = Visibility.Collapsed;
                    break;
                case "Ученик":
                    BtnDelet.Visibility = Visibility.Collapsed;
                    break;
                default:
                    return;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedRequest = (sender as Button)?.DataContext as Bookings;
            if (selectedRequest == null)
            {
                RefreshWindow addEditWindow = new RefreshWindow(selectedRequest);
                if (addEditWindow.ShowDialog() == true)
                {
                    RefreshTechnoServiceDataGrid();
                }
            }
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (Authorization.authorizationRole == "Препод")
            {
                var selectedRequest = (sender as Button)?.DataContext as Bookings;
                if (selectedRequest != null)
                {
                    RefreshWindow addEditWindow = new RefreshWindow(selectedRequest);
                    if (addEditWindow.ShowDialog() == true)
                    {
                        RefreshTechnoServiceDataGrid();
                    }
                }
            }
            else
            {
                MessageBox.Show("Только тренер может редактировать записи.");
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddEditWindow addEditWindow = new AddEditWindow();
            if (addEditWindow.ShowDialog() == true)
            {
                RefreshTechnoServiceDataGrid();
            }
        }

        private void BtnDelet_Click(object sender, RoutedEventArgs e)
        {
            var servisForRemoving = demoYshakoffnick.SelectedItems.Cast<Bookings>().ToList();
            if (servisForRemoving.Any() && MessageBox.Show($"Выточно хотите удалить следующее{servisForRemoving.Count()}элемент ? ", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var context = YshakoffEntities.GetContext();
                context.Bookings.RemoveRange(servisForRemoving);
                context.SaveChanges();
                MessageBox.Show("Данные удалены");
                RefreshTechnoServiceDataGrid();
            }
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            RefreshTechnoServiceDataGrid();
        }

        private void BtnOut_Click(object sender, RoutedEventArgs e)
        {
            if (ComboStatus.SelectedItem is BookingStatus selectedStatus)
            {
                int selectedStatusId = selectedStatus.status_id;
                var context = YshakoffEntities.GetContext();
                demoYshakoffnick.ItemsSource = context.Bookings
                .Include(r => r.Equipment)
                .Include(r => r.TrainingTypes)
                .Include(r => r.Members)
                .Include(r => r.Trainers)
                .Where(r => r.status_id == selectedStatusId)
                .ToList();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchBox.Text.ToLower();
            var context = YshakoffEntities.GetContext();
            try
            {
                demoYshakoffnick.ItemsSource = context.Bookings
                .Include(r => r.Equipment)
                .Include(r => r.TrainingTypes)
                .Include(r => r.Members)
                .Include(r => r.Trainers)
                .Where(r =>
                r.booking_date.ToString().Contains(searchText) ||
                r.Equipment.equipment_name.ToLower().Contains(searchText) ||
                r.TrainingTypes.training_type_name.ToLower().Contains(searchText) ||
                r.Members.member_name.ToLower().Contains(searchText) ||
                r.BookingStatus.status_name.ToLower().Contains(searchText) ||
                r.Trainers.trainer_name.ToLower().Contains(searchText))
                .ToList();
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                // Логированиеилиотладкаисключения
                Console.WriteLine(ex.InnerException?.Message);
            }
        }

        private void BtnAuthorization_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow();
            authorizationWindow.Show();
            this.Close();
        }

    }
}
