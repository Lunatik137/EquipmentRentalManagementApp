using DataAccess.Models;
using Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    public partial class PaymentPage : UserControl
    {
        private readonly User _owner;
        private readonly RentalContractService _contractService;
        private readonly PaymentService _paymentService;
        private readonly UserService _userService;

        public PaymentPage(User owner)
        {
            InitializeComponent();
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _contractService = new RentalContractService();
            _paymentService = new PaymentService();
            _userService = new UserService();
            LoadContracts();
        }

        private void LoadContracts()
        {
            dgContracts.ItemsSource = _contractService.GetAll()
                .Where(c => c.Status == "Đã trả")
                .ToList();
        }

        private void BtnPay_Click(object sender, RoutedEventArgs e)
        {
            if (dgContracts.SelectedItem is not RentalContract contract)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (contract.TotalAmount == null)
            {
                MessageBox.Show("Tổng tiền không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận thanh toán {contract.TotalAmount:N0} VNĐ cho hợp đồng #{contract.ContractId}?",
                                          "Xác nhận thanh toán", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                var payment = new Payment
                {
                    ContractId = contract.ContractId,
                    AmountPaid = contract.TotalAmount ?? 0m,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    Note = "Thanh toán bởi khách hàng"
                };

                _paymentService.Create(payment);
                contract.Status = "Đã thanh toán";
                _contractService.Update(contract);
                LoadContracts();
                MessageBox.Show("Thanh toán thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}