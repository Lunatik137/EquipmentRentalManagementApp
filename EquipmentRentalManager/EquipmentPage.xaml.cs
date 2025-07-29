using DataAccess.Models;
using Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    public partial class EquipmentPage : UserControl
    {
        private readonly EquipmentService _equipmentService;
        private readonly UserService _userService;
        private readonly User _loggedInUser;
        private Equipment _selectedEquipment;

        public EquipmentPage(User user = null)
        {
            InitializeComponent();
            _equipmentService = new EquipmentService();
            _userService = new UserService();
            _loggedInUser = user;
            LoadEquipments();
            CheckPermissions();
        }

        private void LoadEquipments()
        {
            dgEquipments.ItemsSource = _equipmentService.GetAll().ToList();
        }

        private void CheckPermissions()
        {
            if (_loggedInUser != null && !_userService.IsOwner(_loggedInUser))
            {
                BtnCreate.IsEnabled = false;
                BtnUpdate.IsEnabled = false;
                BtnDelete.IsEnabled = false;
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            _selectedEquipment = null;
            editPanel.Visibility = Visibility.Visible;
            ClearForm();
            txtPurchaseDate.Text = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd"); // Điền ngày hiện tại
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipments.SelectedItem is not Equipment equipment)
            {
                MessageBox.Show("Vui lòng chọn thiết bị để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _selectedEquipment = equipment;
            editPanel.Visibility = Visibility.Visible;
            txtName.Text = equipment.Name ?? string.Empty;
            txtDescription.Text = equipment.Description ?? string.Empty;
            txtCategory.Text = equipment.Category ?? string.Empty;
            txtStatus.Text = equipment.Status ? "1" : "0";
            txtQuantityAvailable.Text = equipment.QuantityAvailable.ToString();
            txtDailyRate.Text = equipment.DailyRate?.ToString("N2") ?? "0.00";
            txtPurchaseDate.Text = equipment.PurchaseDate?.ToString("yyyy-MM-dd") ?? DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd"); // Điền ngày hiện tại nếu rỗng
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser == null || !_userService.IsOwner(_loggedInUser)) return;

            if (dgEquipments.SelectedItem is not Equipment equipment)
            {
                MessageBox.Show("Vui lòng chọn thiết bị để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận xóa thiết bị: {equipment.Name}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.Yes)
            {
                _equipmentService.Delete(equipment.EquipmentId);
                LoadEquipments();
                MessageBox.Show("Xóa thiết bị thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser == null || !_userService.IsOwner(_loggedInUser)) return;

            if (!ValidateForm()) return;

            var equipment = _selectedEquipment ?? new Equipment();
            equipment.Name = txtName.Text.Trim();
            equipment.Description = txtDescription.Text.Trim();
            equipment.Category = txtCategory.Text.Trim();

            // Kiểm tra và gán giá trị Status
            string statusText = txtStatus.Text.Trim();
            if (statusText == "0")
            {
                equipment.Status = false; // Inactive
            }
            else if (statusText == "1")
            {
                equipment.Status = true; // Active
            }
            else
            {
                MessageBox.Show("Trạng thái phải là 0 (Inactive) hoặc 1 (Active).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtQuantityAvailable.Text.Trim(), out int quantity) || quantity < 0)
            {
                MessageBox.Show("Số lượng không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            equipment.QuantityAvailable = quantity;

            if (!decimal.TryParse(txtDailyRate.Text.Trim(), out decimal dailyRate) || dailyRate < 0)
            {
                MessageBox.Show("Giá thuê không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            equipment.DailyRate = dailyRate;

            if (!DateOnly.TryParse(txtPurchaseDate.Text.Trim(), out DateOnly purchaseDate))
            {
                MessageBox.Show("Ngày mua không hợp lệ (yyyy-MM-dd).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate ngày phải là ngày hôm nay
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            if (purchaseDate != today)
            {
                MessageBox.Show($"Ngày mua phải là ngày hôm nay ({today:yyyy-MM-dd}).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            equipment.PurchaseDate = purchaseDate;

            if (_selectedEquipment == null)
            {
                _equipmentService.Create(equipment);
                MessageBox.Show("Thêm thiết bị thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _equipmentService.Update(equipment);
                MessageBox.Show("Cập nhật thiết bị thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            editPanel.Visibility = Visibility.Collapsed;
            LoadEquipments();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            editPanel.Visibility = Visibility.Collapsed;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            dgEquipments.ItemsSource = string.IsNullOrEmpty(keyword)
                ? _equipmentService.GetAll().ToList()
                : _equipmentService.SearchByName(keyword).ToList();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtCategory.Text) || string.IsNullOrEmpty(txtStatus.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin bắt buộc (Tên, Loại, Trạng thái).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            string statusText = txtStatus.Text.Trim();
            if (statusText != "0" && statusText != "1")
            {
                MessageBox.Show("Trạng thái phải là 0 (Inactive) hoặc 1 (Active).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtCategory.Text = string.Empty;
            txtStatus.Text = "1";
            txtQuantityAvailable.Text = "0";
            txtDailyRate.Text = "0.00";
            txtPurchaseDate.Text = string.Empty;
        }
    }
}