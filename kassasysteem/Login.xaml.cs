using kassasysteem.Helper;
using kassasysteem.Models;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
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

namespace kassasysteem
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();


            NewFontResolver fontResolver = new NewFontResolver();
            GlobalFontSettings.FontResolver = fontResolver;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (tbUsername.Text == "" || tbPassword.Password == "" || DrawerID.SelectedItem == null)
            {
                MessageBox.Show("Vul alle velden in");
            }
            else
            {

                ComboBoxItem CBiDrawerID = (ComboBoxItem)DrawerID.SelectedItem;
                int drawer_id = Convert.ToInt32(CBiDrawerID.Content);


                Access access = await ApiHelper.GetAccess(tbUsername.Text, tbPassword.Password);
                if (access != null)
                {

                    MainWindow mainWindow = new MainWindow(access, drawer_id);
                    mainWindow.Show();

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Onjuiste inlog gegevens");
                }
            }



        }
    }
}
