using DataAccess.Models;
using Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    public partial class EquipmentPage : UserControl
    {
        private readonly EquipmentService _equipmentService;
        private List<Equipment> _allEquipments;

        public EquipmentPage()
        {
            InitializeComponent();
            _equipmentService = new EquipmentService();
            LoadData();
        }

        private void LoadData()
        {
            _allEquipments = _equipmentService.GetAll()
                .Where(e => e.Status?.ToLower() == "active" && e.QuantityAvailable > 0)
                .ToList();

            dgEquipments.ItemsSource = _allEquipments;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.ToLower();
            dgEquipments.ItemsSource = _allEquipments
                .Where(e => e.Name?.ToLower().Contains(keyword) == true ||
                            e.Category?.ToLower().Contains(keyword) == true)
                .ToList();
        }
    }
}
