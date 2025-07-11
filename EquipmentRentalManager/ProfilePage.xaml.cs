using DataAccess.Models;
using Service;
using Services;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentRentalManager
{
    public partial class ProfilePage : UserControl
    {
        private readonly Owner _owner;
        private readonly OwnerService _ownerService;

        public ProfilePage(Owner owner)
        {
            InitializeComponent();
            _owner = owner;
            _ownerService = new OwnerService();
            LoadData();
        }

        private void LoadData()
        {
            txtName.Text = _owner.FullName;
            txtEmail.Text = _owner.Email;
            txtPhone.Text = _owner.Phone;
            txtPassword.Password = _owner.Password;
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ.");
                return;
            }

            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ (phải là 10 chữ số).");
                return;
            }

            _owner.FullName = txtName.Text.Trim();
            _owner.Email = email;
            _owner.Phone = phone;
            _owner.Password = txtPassword.Password;

            _ownerService.Update(_owner);
            MessageBox.Show("Cập nhật thông tin thành công.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{9}$");
        }

    }
}
