using DataAccess.Models;
using System.Windows;

namespace EquipmentRentalManager
{
    public partial class MainWindow : Window
    {
        private readonly Owner _loggedInOwner;

        public MainWindow(Owner owner)
        {
            InitializeComponent();
            _loggedInOwner = owner;

            MainContent.Content = new EquipmentPage();
        }

        private void BtnEquipment_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EquipmentPage();
        }

        private void BtnCreateRental_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CreateRentalPage(_loggedInOwner);
        }

        private void BtnContracts_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ContractListPage(_loggedInOwner);
        }

        private void BtnPayments_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new PaymentPage(_loggedInOwner);
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ProfilePage(_loggedInOwner);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}
