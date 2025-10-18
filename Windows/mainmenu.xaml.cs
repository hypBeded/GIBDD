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

namespace Windows
{
    /// <summary>
    /// Логика взаимодействия для mainmenu.xaml
    /// </summary>
    public partial class mainmenu : Window
    {
        public mainmenu()
        {
            InitializeComponent();
        }

        private void DriverADD_Click(object sender, RoutedEventArgs e)
        {
            DriverADD driverADD = new DriverADD();
            driverADD.Show();
            this.Close();
        }

        private void DataImport_Click(object sender, RoutedEventArgs e)
        {
            DataImport dataImport = new DataImport();
            dataImport.Show();
            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
