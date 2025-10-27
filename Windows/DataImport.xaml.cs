using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для DataImport.xaml
    /// </summary>
    public partial class DataImport : Window
    {
        private string filesPath;
        public DataImport()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files|*.json;" // Фильтр .json
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                filesPath = openFileDialog.FileName;
            }
            using (var conect = new SqliteConnection("Data Source=GIBDD.db"))
            {
                conect.Open();

            //    SqliteCommand cmd = new SqliteCommand();
            //    cmd.Connection = conect;

            //    //cmd.CommandText = "CREATE TABLE Penalties(_id INTEGER NOT NULL PRIMARY KEY UNIQUE, postNum INTEGER NOT NULL, postDate, TEXT NOT NULL, suma INTEGER NOT NULL, koapCode TEXT NOT NULL, koapText TEXT NOT NULL, address TEXT NOT NULL, license_plate_number , paid INTEGER NOT NULL CHECK(paid IN (0,1))";
            //    cmd.ExecuteNonQuery();

                }
        }
        private class fine
        {
            public int id {  get; set; }
            public int postNum { get; set; }
            public string postDate { get; set; }
            public int sum { get; set; }
            public int koapCode { get; set; }
            public string koapText { get; set; }
            public string address { get; set; }
            public string dataCar { get; set; }
            public int dataDriver { get; set; }

        }
    }
}
