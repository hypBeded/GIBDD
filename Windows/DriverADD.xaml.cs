using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
using static System.Net.Mime.MediaTypeNames;

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























        byte[] imageBytes;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string FN = FullName.Text; //ФИО
            string PI = PasportInfo.Text; //Паспортные данные
            string PN = PhoneNumber.Text; // Номер телефона
            string mail = Email.Text; // почта
            string remark = Zamechanie.Text;// замечание
            string RegA = RegAdress.Text; //адрес регистр
            string ResA = ResAdress.Text; //адрес проживание
            string POW = PlaceOfWork.Text; //Место работы
            string post = Post.Text; //Должность
            byte[] image = imageBytes;










            using (var conect = new SqliteConnection("Data Source=GIBDD.db"))
            {
                try
                {
                    conect.Open();
                    var command = conect.CreateCommand();
                    command.CommandText = "Insert INTO Drivers" +
                        "(Full_Name, Pasporta_data,Phone_number,Email,Remark,Adres_regitr,Adres_residinatia,Workplace,Post,Photo) " +
                        "values  (@Full_Name,@Pasporta_data,@Phone_number,@Email,@Remark,@Adres_regitr,@Adres_residinatia,@Workplace,@Post,@Photo";

                    command.Parameters.AddWithValue("@Full_Name", FN);
                    command.Parameters.AddWithValue("@Pasporta_data", PI);
                    command.Parameters.AddWithValue("@Phone_number", PN);
                    command.Parameters.AddWithValue("@Email", mail);
                    command.Parameters.AddWithValue("@Remark", remark);
                    command.Parameters.AddWithValue("@Adres_regitr", RegA);
                    command.Parameters.AddWithValue("@Adres_residinatia", ResA);
                    command.Parameters.AddWithValue("@Workplace", POW);
                    command.Parameters.AddWithValue("@Post", post);
                    command.Parameters.AddWithValue("@Photo", image);

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Убедитесь что вы заполнили все");
                }
            }
        }















       
        private void DriverImage_Add(object sender, MouseButtonEventArgs e)
        {

            // Создаем экземпляр OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.png;" // Фильтр для изображений
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                imageBytes = File.ReadAllBytes(filePath);

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze(); // Замораживаем изображение для использования в UI
                    DriverImage.Source = bitmap; // Заменяем текущее изображение
                }

                MessageBox.Show("Изображение загружено!");
            }
        }

        private void RegAdress_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
