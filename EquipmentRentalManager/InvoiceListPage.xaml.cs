using DataAccess.Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    public partial class InvoiceListPage : UserControl
    {
        private readonly RentalContractService _contractService;
        private readonly RentalDetailService _detailService;
        private readonly EquipmentService _equipmentService;
        private readonly User _loggedInUser;

        public InvoiceListPage(User user)
        {
            InitializeComponent();
            _contractService = new RentalContractService();
            _detailService = new RentalDetailService();
            _equipmentService = new EquipmentService();
            _loggedInUser = user;
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            var contracts = _contractService.GetAll().ToList();
            var invoiceItems = contracts.Select(contract =>
            {
                var details = _detailService.GetByContractId(contract.ContractId).ToList();
                var equipmentNames = details.Select(d => _equipmentService.Get(d.EquipmentId ?? 0)?.Name ?? "Không xác định").ToList();
                return new InvoiceViewModel
                {
                    ContractId = contract.ContractId,
                    StartDate = contract.StartDate,
                    ReturnDate = contract.ReturnDate,
                    TotalAmount = contract.TotalAmount ?? 0m,
                    Status = contract.Status,
                    EquipmentList = equipmentNames.Any() ? string.Join(", ", equipmentNames) : "Không có dữ liệu thiết bị",
                    TotalQuantity = details.Sum(d => d.Quantity ?? 0),
                    LateFee = CalculateLateFee(contract)
                };
            }).ToList();
            dgInvoices.ItemsSource = invoiceItems;
        }

        private decimal CalculateLateFee(RentalContract contract)
        {
            if (contract.Status == "Đã trả" && contract.ReturnDate.HasValue && contract.EndDate.HasValue && contract.ReturnDate > contract.EndDate)
            {
                var lateDays = (contract.ReturnDate.Value.ToDateTime(TimeOnly.MinValue) - contract.EndDate.Value.ToDateTime(TimeOnly.MinValue)).Days;
                var details = _detailService.GetByContractId(contract.ContractId).ToList();
                decimal lateFee = 0;
                foreach (var detail in details)
                {
                    var equipment = _equipmentService.Get(detail.EquipmentId ?? 0);
                    if (equipment != null)
                    {
                        lateFee += (detail.Quantity ?? 0) * (equipment.DailyRate ?? 0m) * lateDays * 1.5m;
                    }
                }
                return lateFee;
            }
            else if (contract.Status == "Đang thuê" && contract.EndDate.HasValue && DateOnly.FromDateTime(DateTime.Now) > contract.EndDate)
            {
                var lateDays = (DateOnly.FromDateTime(DateTime.Now).ToDateTime(TimeOnly.MinValue) - contract.EndDate.Value.ToDateTime(TimeOnly.MinValue)).Days;
                var details = _detailService.GetByContractId(contract.ContractId).ToList();
                decimal lateFee = 0;
                foreach (var detail in details)
                {
                    var equipment = _equipmentService.Get(detail.EquipmentId ?? 0);
                    if (equipment != null)
                    {
                        lateFee += (detail.Quantity ?? 0) * (equipment.DailyRate ?? 0m) * lateDays * 1.5m;
                    }
                }
                return lateFee;
            }
            return 0m;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            LoadInvoices();
        }


        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (dgInvoices.SelectedItem is not InvoiceViewModel invoice)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng để xem chi tiết.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var detailWindow = new InvoiceDetailWindow(invoice.ContractId, _detailService, _equipmentService);
            detailWindow.ShowDialog();
        }

        private void BtnFilterByDate_Click(object sender, RoutedEventArgs e)
        {
            DateOnly? from = dpStartDate.SelectedDate.HasValue ? DateOnly.FromDateTime(dpStartDate.SelectedDate.Value) : null;
            DateOnly? to = dpEndDate.SelectedDate.HasValue ? DateOnly.FromDateTime(dpEndDate.SelectedDate.Value) : null;

            if (from.HasValue && to.HasValue && to < from)
            {
                MessageBox.Show("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var contracts = _contractService.GetAll().ToList();

            if (from.HasValue)
                contracts = contracts.Where(c => c.StartDate >= from.Value).ToList();
            if (to.HasValue)
                contracts = contracts.Where(c => c.StartDate <= to.Value).ToList();

            var invoiceItems = contracts.Select(contract =>
            {
                var details = _detailService.GetByContractId(contract.ContractId).ToList();
                var equipmentNames = details.Select(d => _equipmentService.Get(d.EquipmentId ?? 0)?.Name ?? "Không xác định").ToList();

                return new InvoiceViewModel
                {
                    ContractId = contract.ContractId,
                    StartDate = contract.StartDate,
                    ReturnDate = contract.ReturnDate,
                    TotalAmount = contract.TotalAmount ?? 0m,
                    Status = contract.Status,
                    EquipmentList = equipmentNames.Any() ? string.Join(", ", equipmentNames) : "Không có dữ liệu thiết bị",
                    TotalQuantity = details.Sum(d => d.Quantity ?? 0),
                    LateFee = CalculateLateFee(contract)
                };
            }).ToList();

            dgInvoices.ItemsSource = invoiceItems;
        }



        private class InvoiceViewModel
        {
            public int ContractId { get; set; }
            public DateOnly? StartDate { get; set; }
            public DateOnly? ReturnDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; }
            public string EquipmentList { get; set; }
            public int TotalQuantity { get; set; }
            public decimal LateFee { get; set; }
        }
    }
}