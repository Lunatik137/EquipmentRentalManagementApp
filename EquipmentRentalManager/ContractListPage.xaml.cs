using DataAccess.Models;
using Repositories;
using Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    public partial class ContractListPage : UserControl
    {
        private readonly RentalContractService _contractService;
        private readonly UserService _userService;
        private readonly EquipmentService _equipmentService;
        private readonly RentalDetailService _detailService;
        private readonly User _loggedInUser;
        private RentalContract _selectedContract;

        public ContractListPage(User user)
        {
            InitializeComponent();
            _contractService = new RentalContractService();
            _userService = new UserService();
            _equipmentService = new EquipmentService();
            _detailService = new RentalDetailService();
            _loggedInUser = user;
            LoadContracts();
            dgContracts.SelectionChanged += dgContracts_SelectionChanged; // Đảm bảo lắng nghe sự kiện chọn hợp đồng
            CheckPermissions(); // Gọi ban đầu để cập nhật nút
        }

        private void LoadContracts()
        {
            DateOnly str = DateOnly.ParseExact("01/07/2025", "dd/MM/yyyy");
            DateOnly end = DateOnly.ParseExact("31/07/2025", "dd/MM/yyyy");
            dgContracts.ItemsSource = _contractService.GetAll().Where(c => c.StartDate >= str && c.EndDate <= end).ToList();
            int totalContracts = dgContracts.Items.Count;
            count.Text = totalContracts.ToString();
        }

        private void CheckPermissions()
        {
            if (_loggedInUser == null) return;

            bool isOwner = _userService.IsOwner(_loggedInUser);
            BtnDelete.IsEnabled = false;
            BtnReturn.IsEnabled = false;

            BtnDelete.Visibility = isOwner ? Visibility.Visible : Visibility.Collapsed;

            if (dgContracts.SelectedItem is RentalContract selectedContract)
            {
                if (isOwner)
                {
                    BtnDelete.IsEnabled = true;
                    BtnReturn.IsEnabled = selectedContract.Status == "Đang thuê"; 
                }
                else
                {
                    BtnReturn.IsEnabled = (selectedContract.Status == "Đang thuê") && (selectedContract.UserId != _loggedInUser.UserId);
                }
            }
        }

        private void dgContracts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckPermissions();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser == null) return;

            if (dgContracts.SelectedItem is not RentalContract contract)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!BtnDelete.IsEnabled)
            {
                MessageBox.Show("Bạn không có quyền xóa hợp đồng này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var paymentService = new PaymentService();
            if (paymentService.GetByContractId(contract.ContractId).Any())
            {
                MessageBox.Show("Hợp đồng có thanh toán liên quan, không thể xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận xóa hợp đồng: #{contract.ContractId}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.Yes)
            {
                var details = _detailService.GetByContractId(contract.ContractId);
                if (contract.Status == "Đang thuê")
                {
                    foreach (var detail in details)
                    {
                        if (detail.EquipmentId.HasValue)
                        {
                            var equipment = _equipmentService.Get(detail.EquipmentId.Value);
                            if (equipment != null)
                            {
                                equipment.QuantityAvailable += detail.Quantity ?? 0;
                                _equipmentService.Update(equipment);
                            }
                        }
                    }
                }

                foreach (var detail in details)
                {
                    _detailService.Delete(detail.RentalDetailId);
                }

                _contractService.Delete(contract.ContractId);
                LoadContracts();
                MessageBox.Show("Xóa hợp đồng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser == null) return;

            if (!ValidateForm()) return;

            var contract = _selectedContract ?? new RentalContract();
            contract.StartDate = DateOnly.FromDateTime(dpStartDate.SelectedDate.Value);
            contract.EndDate = DateOnly.FromDateTime(dpEndDate.SelectedDate.Value);
            contract.Status = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            try
            {
                if (_selectedContract == null)
                {
                    contract.UserId = _loggedInUser.UserId;
                    _contractService.Create(contract);
                }
                else
                {
                    _contractService.Update(contract);
                }
                _contractService.RecalculateTotalAmount(contract.ContractId); // Tự động tính lại tổng tiền (cải tiến để chính xác)
                MessageBox.Show(_selectedContract == null ? "Thêm hợp đồng thành công." : "Cập nhật hợp đồng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            editPanel.Visibility = Visibility.Collapsed;
            LoadContracts();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            editPanel.Visibility = Visibility.Collapsed;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            dgContracts.ItemsSource = string.IsNullOrEmpty(keyword)
                ? _contractService.GetAll().ToList()
                : _contractService.SearchByUserId(_loggedInUser.UserId, keyword).ToList();
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (dgContracts.SelectedItem is not RentalContract contract || contract.Status != "Đang thuê")
            {
                MessageBox.Show("Vui lòng chọn hợp đồng 'Đang thuê' để trả thiết bị.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!BtnReturn.IsEnabled)
            {
                MessageBox.Show("Bạn không có quyền trả thiết bị cho hợp đồng này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận trả thiết bị cho hợp đồng: #{contract.ContractId}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.Yes)
            {
                var repoField = _contractService.GetType().GetField("_repo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (repoField == null)
                {
                    MessageBox.Show("Lỗi hệ thống: Không thể truy cập repository.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var repo = repoField.GetValue(_contractService) as RentalContractRepo;
                if (repo == null)
                {
                    MessageBox.Show("Lỗi hệ thống: Repository không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var contextField = repo.GetType().GetField("context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (contextField == null)
                {
                    MessageBox.Show("Lỗi hệ thống: Không thể truy cập context.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var context = contextField.GetValue(repo) as EquipmentRentalManagementContext;
                if (context == null)
                {
                    MessageBox.Show("Lỗi hệ thống: Context không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using var transaction = context.Database.BeginTransaction();
                try
                {
                    var details = _detailService.GetByContractId(contract.ContractId).ToList();
                    foreach (var detail in details)
                    {
                        if (detail.EquipmentId.HasValue)
                        {
                            var equipment = _equipmentService.Get(detail.EquipmentId.Value);
                            if (equipment != null)
                            {
                                equipment.QuantityAvailable += detail.Quantity ?? 0;
                                _equipmentService.Update(equipment);
                            }
                        }
                    }

                    contract.Status = "Đã trả";
                    contract.ReturnDate = DateOnly.FromDateTime(DateTime.Now); 
                    _contractService.Update(contract);

                    transaction.Commit();
                    LoadContracts();
                    MessageBox.Show("Trả thiết bị thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Lỗi khi trả thiết bị: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateForm()
        {
            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null || string.IsNullOrEmpty(txtTotalAmount.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (dpEndDate.SelectedDate <= dpStartDate.SelectedDate)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}