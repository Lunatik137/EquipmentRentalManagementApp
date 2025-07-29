using DataAccess.Models;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadPage(new CreateRentalPage(user));
            UpdateUIForRole();
        }

        private void LoadPage(UserControl page)
        {
            MainContent.Content = page;
        }

        private void UpdateUIForRole()
        {
            if (_currentUser != null && new UserService().IsOwner(_currentUser))
            {
                BtnStaffManagement.Visibility = Visibility.Visible;
            }
        }

        private void BtnCreateRental_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(new CreateRentalPage(_currentUser));
        }

        private void BtnEquipment_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(new EquipmentPage(_currentUser));
        }

        private void BtnStaffManagement_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(new StaffManagementPage(_currentUser));
        }

        private void BtnContractList_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(new ContractListPage(_currentUser));
        }

        private void BtnInvoiceList_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(new InvoiceListPage(_currentUser));
        }

        private void BtnPayment_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(new PaymentPage(_currentUser));
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(new ProfilePage(_currentUser));
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}