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
        private readonly Owner _owner;
        private readonly RentalContractService _contractService;
        private readonly PaymentService _paymentService;

        public PaymentPage(Owner owner)
        {
            InitializeComponent();
            _owner = owner;
            _contractService = new RentalContractService();
            _paymentService = new PaymentService();
            LoadContracts();
        }

        private void LoadContracts()
        {
            var unpaid = _contractService.GetByOwnerId(_owner.OwnerId)
                .Where(c => c.Status == "Đã trả")
                .ToList();

            dgContracts.ItemsSource = unpaid;
        }

        private void BtnPay_Click(object sender, RoutedEventArgs e)
        {
            if (dgContracts.SelectedItem is not RentalContract contract)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng.");
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận thanh toán {contract.TotalAmount:C} cho hợp đồng #{contract.ContractId}?",
                                          "Xác nhận thanh toán", MessageBoxButton.YesNo);

            if (confirm == MessageBoxResult.Yes)
            {
                var payment = new Payment
                {
                    ContractId = contract.ContractId,
                    AmountPaid = contract.TotalAmount,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    Note = "Thanh toán bởi khách hàng"
                };

                _paymentService.Create(payment);

                contract.Status = "Đã thanh toán";
                _contractService.Update(contract);

                LoadContracts();
                MessageBox.Show("Thanh toán thành công!");
            }
        }
    }
}
