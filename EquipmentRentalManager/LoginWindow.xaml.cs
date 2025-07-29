using Services;
using System.Windows;

namespace EquipmentRentalManager
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text?.Trim() ?? string.Empty;
            var password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _userService.GetByUsername(username);
            if (user != null && user.Password == password) 
            {
                MessageBox.Show($"Welcome, {user.FullName}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow main = new MainWindow(user);
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
            var username = txtUsername.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _userService.GetByUsername(username);
            if (user == null)
            {
                MessageBox.Show("Không tìm thấy tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resetWindow = new ResetPasswordWindow(user);
            if (resetWindow != null)
            {
                resetWindow.ShowDialog();
            }
        }
    }
}