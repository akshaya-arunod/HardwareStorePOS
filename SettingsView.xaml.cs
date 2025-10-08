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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HardwareStore.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }


        private void AdminCurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                // Save the password somewhere in your ViewModel or code-behind
                // For example:
                // ((SettingsViewModel)DataContext).AdminCurrentPassword = passwordBox.Password;
            }
        }

        private void AdminNewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                // ((SettingsViewModel)DataContext).AdminNewPassword = passwordBox.Password;
            }
        }

        private void CashierCurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                // ((SettingsViewModel)DataContext).CashierCurrentPassword = passwordBox.Password;
            }
        }

        private void CashierNewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                // ((SettingsViewModel)DataContext).CashierNewPassword = passwordBox.Password;
            }
        }
    }
}
