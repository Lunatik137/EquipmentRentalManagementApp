using Service;
using Services;
using System.Windows;

namespace EquipmentRentalManager
{
    public partial class LoginWindow : Window
    {
        private OwnerService _loggedInOwner;

        public LoginWindow()
        {
            InitializeComponent();
            _loggedInOwner = new OwnerService();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Password;

            var owner = _loggedInOwner.Login(username, password);
            if (owner != null)
            {
                MessageBox.Show($"Welcome, {owner.FullName}!", "Login Successful");
                MainWindow main = new MainWindow(owner);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập trước.");
                return;
            }

            var ownerService = new OwnerService();
            var owner = ownerService.GetByUsername(username);

            if (owner == null)
            {
                MessageBox.Show("Không tìm thấy tài khoản.");
                return;
            }

            var resetWindow = new ResetPasswordWindow(owner);
            resetWindow.ShowDialog();
        }

    }
}
