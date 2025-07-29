using DataAccess.Models;
using Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    public partial class StaffManagementPage : UserControl
    {
        private readonly User _owner;
        private readonly UserService _userService;
        private User _selectedUser;

        public StaffManagementPage(User owner)
        {
            InitializeComponent();
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _userService = new UserService();
            LoadStaff();
            CheckPermissions();
        }

        private void LoadStaff()
        {
            dgStaff.ItemsSource = _userService.GetAll().Where(u => u.Role != "Owner" || u.UserId == _owner.UserId).ToList();
        }

        private void CheckPermissions()
        {
            if (!_userService.IsOwner(_owner))
            {
                BtnAdd.IsEnabled = false;
                BtnEdit.IsEnabled = false;
                BtnDelete.IsEnabled = false;
                MessageBox.Show("Bạn không có quyền quản lý nhân viên.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _selectedUser = null;
            editPanel.Visibility = Visibility.Visible;
            txtFullName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtPassword.Password = string.Empty;
            cbRole.SelectedIndex = 0;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgStaff.SelectedItem is not User user)
            {
                MessageBox.Show("Vui lòng chọn nhân viên để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _selectedUser = user;
            editPanel.Visibility = Visibility.Visible;
            txtFullName.Text = user.FullName ?? string.Empty;
            txtEmail.Text = user.Email ?? string.Empty;
            txtPhone.Text = user.Phone ?? string.Empty;
            txtPassword.Password = user.Password ?? string.Empty;
            cbRole.SelectedIndex = user.Role == "Owner" ? 1 : 0;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgStaff.SelectedItem is not User user || user.UserId == _owner.UserId)
            {
                MessageBox.Show("Vui lòng chọn nhân viên khác để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận xóa nhân viên: {user.FullName}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.Yes)
            {
                _userService.Delete(user.UserId);
                LoadStaff();
                MessageBox.Show("Xóa nhân viên thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Password;
            string role = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
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

            var user = _selectedUser ?? new User();
            user.FullName = fullName;
            user.Email = email;
            user.Phone = phone;
            user.Password = password;
            user.Role = role;

            if (_selectedUser == null)
            {
                _userService.Create(user);
                MessageBox.Show("Thêm nhân viên thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _userService.Update(user);
                MessageBox.Show("Cập nhật nhân viên thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            editPanel.Visibility = Visibility.Collapsed;
            LoadStaff();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            editPanel.Visibility = Visibility.Collapsed;
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