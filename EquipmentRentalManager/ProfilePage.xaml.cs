using DataAccess.Models;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    public partial class ProfilePage : UserControl
    {
        private readonly User _owner;
        private readonly UserService _ownerService;

        public ProfilePage(User owner)
        {
            InitializeComponent();
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _ownerService = new UserService();
            LoadData();
        }

        private void LoadData()
        {
            if (_owner == null) return;
            txtName.Text = _owner.FullName ?? string.Empty;
            txtEmail.Text = _owner.Email ?? string.Empty;
            txtPhone.Text = _owner.Phone ?? string.Empty;
            txtPassword.Password = _owner.Password ?? string.Empty;
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ (phải là 10 chữ số).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _owner.FullName = name;
            _owner.Email = email;
            _owner.Phone = phone;
            _owner.Password = password;

            _ownerService.Update(_owner);
            MessageBox.Show("Cập nhật thông tin thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return !string.IsNullOrEmpty(phone) && System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{9}$");
        }
    }
}