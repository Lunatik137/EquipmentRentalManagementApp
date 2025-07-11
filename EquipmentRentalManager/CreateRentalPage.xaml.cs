using DataAccess.Models;
using Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace EquipmentRentalManager
{
    public partial class CreateRentalPage : UserControl
    {
        private readonly Owner _owner;
        private readonly EquipmentService _equipmentService;
        private readonly RentalContractService _contractService;
        private readonly RentalDetailService _detailService;

        public CreateRentalPage(Owner owner)
        {
            InitializeComponent();
            _owner = owner;
            _equipmentService = new EquipmentService();
            _contractService = new RentalContractService();
            _detailService = new RentalDetailService();
            LoadEquipments();
        }

        private void LoadEquipments()
        {
            dgEquipments.ItemsSource = _equipmentService.GetAvailable();
        }

        private void CreateContract_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipments.SelectedItem is not Equipment selected)
            {
                MessageBox.Show("Vui lòng chọn thiết bị.");
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ.");
                return;
            }

            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu và kết thúc.");
                return;
            }

            if (dpEndDate.SelectedDate <= dpStartDate.SelectedDate)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu.");
                return;
            }

            var contract = new RentalContract
            {
                OwnerId = _owner.OwnerId,
                StartDate = DateOnly.FromDateTime(dpStartDate.SelectedDate.Value),
                EndDate = DateOnly.FromDateTime(dpEndDate.SelectedDate.Value),
                Status = "Đang thuê",
                TotalAmount = 0
            };
            _contractService.Create(contract);

            var detail = new RentalDetail
            {
                ContractId = contract.ContractId,
                EquipmentId = selected.EquipmentId,
                Quantity = quantity,
                RatePerDay = selected.DailyRate
            };

            bool success = _detailService.CreateRentalDetailWithInventoryCheck(detail);

            if (success)
            {
                MessageBox.Show("Tạo hợp đồng thuê thành công!");
                LoadEquipments();
                txtQuantity.Clear();
                dpStartDate.SelectedDate = null;
                dpEndDate.SelectedDate = null;
            }
            else
            {
                MessageBox.Show("Không thể thuê thiết bị. Thiết bị không đủ số lượng hoặc không hoạt động.");
            }
        }
    }
}
