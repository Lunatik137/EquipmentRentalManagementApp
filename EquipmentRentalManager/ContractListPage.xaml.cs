using DataAccess.Models;
using Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    public partial class ContractListPage : UserControl
    {
        private readonly Owner _owner;
        private readonly RentalContractService _contractService;
        private readonly RentalDetailService _detailService;

        private List<RentalContract> _contracts;

        public ContractListPage(Owner owner)
        {
            InitializeComponent();
            _owner = owner;
            _contractService = new RentalContractService();
            _detailService = new RentalDetailService();
            LoadContracts();
        }

        private void LoadContracts()
        {
            _contracts = _contractService.GetByOwnerId(_owner.OwnerId);
            dgContracts.ItemsSource = _contracts;
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (dgContracts.SelectedItem is not RentalContract selected)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng.");
                return;
            }

            if (selected.Status == "Đã trả" || selected.Status == "Đã thanh toán")
            {
                MessageBox.Show("Thiết bị trong hợp đồng này đã được trả hoặc đã thanh toán.");
                return;
            }

            var confirm = MessageBox.Show("Bạn chắc chắn muốn trả thiết bị?", "Xác nhận", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.Yes)
            {
                var details = _detailService.GetByContractId(selected.ContractId);
                foreach (var d in details)
                {
                    _detailService.ReturnRentalDetail(d.RentalDetailId);
                }

                selected.Status = "Đã trả";
                _contractService.Update(selected);

                LoadContracts();
                MessageBox.Show("Đã trả thiết bị thành công.");
            }
        }
    }
}
