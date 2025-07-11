using System.Windows;
using System.Windows.Controls;
using Services;
using DataAccess.Models;

namespace EquipmentRentalManager
{
    public partial class EquipmentPage : UserControl
    {
        private readonly EquipmentService _equipmentService;

        public EquipmentPage()
        {
            InitializeComponent();
            _equipmentService = new EquipmentService();
            LoadEquipments();
        }

        private void LoadEquipments()
        {
            dgEquipments.ItemsSource = _equipmentService.GetAll();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng thêm thiết bị đang được phát triển.");
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipments.SelectedItem is Equipment selected)
            {
                MessageBox.Show($"Sửa thiết bị: {selected.Name}");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipments.SelectedItem is Equipment selected)
            {
                var confirm = MessageBox.Show($"Xác nhận xóa thiết bị: {selected.Name}?", "Xác nhận", MessageBoxButton.YesNo);
                if (confirm == MessageBoxResult.Yes)
                {
                    _equipmentService.Delete(selected.EquipmentId);
                    LoadEquipments();
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                dgEquipments.ItemsSource = _equipmentService.GetAll();
            }
            else
            {
                dgEquipments.ItemsSource = _equipmentService.SearchByName(keyword);
            }
        }


    }
}
