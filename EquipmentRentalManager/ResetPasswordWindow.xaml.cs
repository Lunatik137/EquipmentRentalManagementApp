using DataAccess.Models;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    /// <summary>
    /// Interaction logic for ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        private readonly User _owner;
        private readonly UserService _userService;

        public ResetPasswordWindow(User owner)
        {
            InitializeComponent();
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _userService = new UserService();
        }

        private void BtnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            var input = txtContact.Text?.Trim() ?? string.Empty;
            var newPassword = txtNewPassword.Password;

            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin xác minh và mật khẩu mới.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (input != _owner.Email && input != _owner.Phone)
            {
                MessageBox.Show("Thông tin xác minh không chính xác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _owner.Password = newPassword;
            _userService.Update(_owner);
            MessageBox.Show("Mật khẩu đã được cập nhật thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}