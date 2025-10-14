using Microsoft.Data.Sqlite;
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
    /// Логика взаимодействия для DriverADD.xaml
    /// </summary>
    public partial class DriverADD : Window
    {
        public DriverADD()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var conect = new SqliteConnection("Data Source=GIBDD.db"))
            {
                conect.Open();
                SqliteCommand cmd = new SqliteCommand();
                cmd.Connection = conect;
                
            }

        }

       

        private void DriverImage_Add(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
