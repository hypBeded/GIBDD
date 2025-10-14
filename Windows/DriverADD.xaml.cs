using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            string FN = FullName.Text; //ФИО
            string PI = PasportInfo.Text; //Паспортные данные
            string PN = PhoneNumber.Text; // Номер телефона
            string mail = Email.Text ; // почта
            string remark = Zamechanie.Text ;// замечание
            string RegA = RegAdress.Text; //адрес регистр
            string ResA = ResAdress.Text; //адрес проживание
            string POW = PlaceOfWork.Text; //Место работы
            string post = Post.Text; //Должность
            byte image;










            using (var conect = new SqliteConnection("Data Source=GIBDD.db"))
            {
                conect.Open();
                SqliteCommand cmd = new SqliteCommand();
                cmd.Connection = conect;
                SqliteCommand sqliteCommand = new SqliteCommand();

                

               
                   



                
            }

        }
        public class User
        {
            public int id { get; set; }
            public string FullName { get; set; }
            public string email { get; set; }

            public int PasportInfo { get; set; }
            public int PhoneNubmer { get; set; }
            public byte[] Image { get; set; }
            public string Zamechania { get; set; }
            public string RegAddress { get; set; }
            public string ResidentialAddress { get; set; }
            public string PlaceOfWork { get; set; }
            public string post { get; set; }


        
        }
       














        private void DriverImage_Add(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
