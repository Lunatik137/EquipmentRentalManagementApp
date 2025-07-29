using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EquipmentRentalManager
{
    public partial class InvoiceDetailWindow : Window
    {
        private readonly RentalDetailService _detailService;
        private readonly EquipmentService _equipmentService;

        public InvoiceDetailWindow(int contractId, RentalDetailService detailService, EquipmentService equipmentService)
        {
            InitializeComponent();
            _detailService = detailService;
            _equipmentService = equipmentService;
            LoadDetails(contractId);
        }

        private void LoadDetails(int contractId)
        {
            var details = _detailService.GetByContractId(contractId).ToList();
            if (!details.Any())
            {
                MessageBox.Show($"Không tìm thấy chi tiết cho hợp đồng {contractId}.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var displayDetails = details.Select(d => new
            {
                EquipmentName = _equipmentService.Get(d.EquipmentId ?? 0)?.Name ?? "Không xác định",
                Quantity = d.Quantity ?? 0,
                RatePerDay = d.RatePerDay ?? 0m
            }).ToList();
            dgDetails.ItemsSource = displayDetails;
        }
    }
}