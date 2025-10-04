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
using Microsoft.Data.Sqlite;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using(var conect = new SqliteConnection("Data Source=GIBDD.db"))
            {
                conect.Open();
                SqliteCommand command = new SqliteCommand();
                command.Connection = conect;
                command.CommandText = "CREATE TABLE Drivers(_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Full_name TEXT NOT NULL, Pasport_Date TEXT NOT NULL, Statement TEXT NOT NULL, PTS TEXT NOT NULL, STS TEXT NOT NULL, Contract TEXT NOT NULL)";
                command.ExecuteNonQuery();
            }
        }
    }
}
