using DataAccess.Models;
using Service;
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
    /// <summary>
    /// Interaction logic for ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        private Owner _owner;

        public ResetPasswordWindow(Owner owner)
        {
            InitializeComponent();
            _owner = owner;
        }

        private void BtnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            var input = txtContact.Text.Trim();
            var newPass = txtNewPassword.Password;

            if ((input != _owner.Email && input != _owner.Phone) || string.IsNullOrEmpty(newPass))
            {
                MessageBox.Show("Thông tin xác minh không chính xác hoặc mật khẩu trống.");
                return;
            }

            _owner.Password = newPass;
            var service = new OwnerService();
            service.Update(_owner);

            MessageBox.Show("Mật khẩu đã được cập nhật.");
            this.Close();
        }
    }

}
