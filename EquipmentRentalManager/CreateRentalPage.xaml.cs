using DataAccess.Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRentalManager
{
    public partial class CreateRentalPage : UserControl
    {
        private readonly User _owner;
        private readonly EquipmentService _equipmentService;
        private readonly RentalContractService _contractService;
        private readonly RentalDetailService _detailService;
        private readonly EquipmentRentalManagementContext _context;

        private List<Equipment> _originalEquipments;
        private List<Equipment> _uiEquipments; 
        private List<SelectedEquipmentItem> _selectedEquipments = new();

        public CreateRentalPage(User owner)
        {
            InitializeComponent();
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _equipmentService = new EquipmentService();
            _contractService = new RentalContractService();
            _detailService = new RentalDetailService();
            _context = new EquipmentRentalManagementContext();
            LoadEquipments();
        }

        private void LoadEquipments()
        {
            _originalEquipments = _equipmentService.GetAll().Where(e => e.QuantityAvailable > 0 && e.Status == true).ToList();
            _uiEquipments = _originalEquipments.Select(e => new Equipment
            {
                EquipmentId = e.EquipmentId,
                Name = e.Name,
                Category = e.Category,
                DailyRate = e.DailyRate,
                QuantityAvailable = e.QuantityAvailable,
                Status = e.Status
            }).ToList();
            dgEquipments.ItemsSource = _uiEquipments;
        }

        private void BtnAddToList_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipments.SelectedItem is not Equipment selected)
            {
                MessageBox.Show("Chọn thiết bị để thêm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var uiSelected = _uiEquipments.First(e => e.EquipmentId == selected.EquipmentId);
            if (quantity > uiSelected.QuantityAvailable)
            {
                MessageBox.Show("Không đủ thiết bị.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existing = _selectedEquipments.FirstOrDefault(e => e.Equipment.EquipmentId == selected.EquipmentId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                _selectedEquipments.Add(new SelectedEquipmentItem
                {
                    Equipment = selected,
                    Quantity = quantity
                });
            }

            uiSelected.QuantityAvailable -= quantity;
            RefreshSelectedGrid();
            txtQuantity.Clear();
        }

        private void RefreshSelectedGrid()
        {
            dgSelected.ItemsSource = null;
            dgSelected.ItemsSource = _selectedEquipments;
            dgEquipments.Items.Refresh();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is SelectedEquipmentItem selectedItem)
            {
                var uiEquipment = _uiEquipments.First(e => e.EquipmentId == selectedItem.Equipment.EquipmentId);
                uiEquipment.QuantityAvailable += selectedItem.Quantity;
                _selectedEquipments.Remove(selectedItem);
                RefreshSelectedGrid();
            }
        }

        private void CreateContract_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEquipments.Count == 0)
            {
                MessageBox.Show("Chưa có thiết bị nào được chọn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Chọn ngày bắt đầu và kết thúc.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpEndDate.SelectedDate <= dpStartDate.SelectedDate)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var contract = new RentalContract
                {
                    UserId = _owner.UserId,
                    StartDate = DateOnly.FromDateTime(dpStartDate.SelectedDate.Value),
                    EndDate = DateOnly.FromDateTime(dpEndDate.SelectedDate.Value),
                    Status = "Đang thuê",
                    TotalAmount = 0
                };

                _contractService.Create(contract);
                int newContractId = contract.ContractId;

                if (newContractId <= 0)
                {
                    throw new Exception("Không thể lấy ContractId sau khi tạo.");
                }

                bool allSuccess = true;
                decimal totalAmount = 0;
                var days = (dpEndDate.SelectedDate.Value - dpStartDate.SelectedDate.Value).Days;

                foreach (var item in _selectedEquipments)
                {
                    var originalEquipment = _originalEquipments.First(e => e.EquipmentId == item.Equipment.EquipmentId);
                    if (originalEquipment.QuantityAvailable < item.Quantity)
                    {
                        allSuccess = false;
                        continue;
                    }

                    var detail = new RentalDetail
                    {
                        ContractId = newContractId,
                        EquipmentId = item.Equipment.EquipmentId,
                        Quantity = item.Quantity,
                        RatePerDay = item.Equipment.DailyRate ?? 0m
                    };

                    _detailService.Create(detail);
                    totalAmount += item.Quantity * (item.Equipment.DailyRate ?? 0m) * days;
                    originalEquipment.QuantityAvailable -= item.Quantity; 
                    _context.Entry(originalEquipment).State = EntityState.Modified; 
                }

                if (allSuccess)
                {
                    contract.TotalAmount = totalAmount;
                    _contractService.Update(contract);
                    _context.SaveChanges();
                    transaction.Commit();
                    MessageBox.Show($"Hợp đồng #{newContractId} đã được tạo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    _selectedEquipments.Clear();
                    LoadEquipments(); 
                    RefreshSelectedGrid();
                    txtQuantity.Clear();
                    dpStartDate.SelectedDate = null;
                    dpEndDate.SelectedDate = null;
                }
                else
                {
                    throw new Exception("Một số thiết bị không đủ điều kiện thuê.");
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                LoadEquipments();
                RefreshSelectedGrid();
                MessageBox.Show($"Lỗi khi tạo hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private class SelectedEquipmentItem
        {
            public Equipment Equipment { get; set; }
            public int Quantity { get; set; }
        }
    }
}