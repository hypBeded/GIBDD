using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            if (PasportInfo.Text != null)
            {
                Regex regex = new Regex(@"^\d{11}$");

                if (!regex.IsMatch(PasportInfo.Text))
                {
                    MessageBox.Show("Неверный формат. Используйте  11 цифр.");
                    return;
                }
            }
            int PI = Convert.ToInt32(PasportInfo.Text); //Паспортные данные
                                                        //
            if (PhoneNumber.Text != null)
            {
                // Паттерн: 11 цифр
                Regex regex = new Regex(@"^\d{11}$");
                if (!regex.IsMatch(PhoneNumber.Text))
                {
                    MessageBox.Show("Неверный формат номера телефона. Используйте 11 цифр.");
                    return;
                }
            }
            int PN = Convert.ToInt32(PhoneNumber.Text); //номер телефона
                                                        //

            if (!ValidateEmail(Email.Text))
            {
                MessageBox.Show("Некорректный email.", "Ошибка");
                return;
            }
            string mail = Email.Text; // почта
            string remark = Zamechanie.Text;// замечание
            //
            string city1 = City1.Text;
            string city2 = City2.Text;
            //
            string adress1 = Adress1.Text;
            string adress2 = Adress2.Text;
            //
            string RegA = ($"{city1}, {adress1}");    //адрес регистр
            string ResA = ($"{city2}, {adress2}"); //адрес проживание
            string POW = PlaceOfWork.Text; //Место работы
            string post = Post.Text; //Должность
            byte[] image = imageBytes;
            //
            using (var conect = new SqliteConnection("Data Source=GIBDD.db"))
            {
                try
                {
                    conect.Open();
                    var command = conect.CreateCommand();
                    command.CommandText = "Insert INTO Drivers" +
                        "(Full_Name, Pasporta_data,Phone_number,Email,Remark,Adres_regitr,Adres_residinatial,Workplace,Post,Photo) " +
                        "values  (@Full_Name,@Pasporta_data,@Phone_number,@Email,@Remark,@Adres_regitr,@Adres_residinatial,@Workplace,@Post,@Photo)";

                    command.Parameters.AddWithValue("@Full_Name", FN);
                    command.Parameters.AddWithValue("@Pasporta_data", PI);
                    command.Parameters.AddWithValue("@Phone_number", PN);
                    command.Parameters.AddWithValue("@Email", mail);
                    command.Parameters.AddWithValue("@Remark", remark);
                    command.Parameters.AddWithValue("@Adres_regitr", RegA);
                    command.Parameters.AddWithValue("@Adres_residinatial", ResA);
                    command.Parameters.AddWithValue("@Workplace", POW);
                    command.Parameters.AddWithValue("@Post", post);
                    command.Parameters.AddWithValue("@Photo", image);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Запись добавлена");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Убедитесь что вы заполнили все " + ex.Message);
                    return;
                }
            }

            FullName.Text = string.Empty; //ФИО
            PasportInfo.Text = string.Empty;  //"серия и номер без пробела";  //Паспортные данные
            PhoneNumber.Text = string.Empty; //"Номер телефона без символов";  // Номер телефона
            Email.Text = string.Empty; ; // почта
            Zamechanie.Text = string.Empty; // замечание
            //                                
            City1.Text = string.Empty;
            City2.Text = string.Empty;
            //
            Adress1.Text = string.Empty;
            Adress2.Text = string.Empty;
            //

            PlaceOfWork.Text = string.Empty; ; //Место работы
            Post.Text = string.Empty; ; //Должность

            string filePath = "/Image2.png";
            var bitmap = new BitmapImage(new Uri(filePath, UriKind.Relative));
            DriverImage.Source = bitmap;

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


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PasportInfo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //        var textBox = (TextBox)sender;
            //if (textBox != null)
            //{
            //string currentText = textBox.Text;

            //// Получаем новый текст после вставки
            //string newText = currentText.Insert(textBox.SelectionStart, e.Text);

            //// Проверяем, соответствует ли новый текст формату: ровно 11 цифр
            //Regex regex = new Regex(@"^\d{11}$");

            //// Если новый текст не соответствует паттерну, отменяем ввод
            //e.Handled = !regex.IsMatch(newText);
        }
    

        private void PhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //var textBox = (TextBox)sender;
            //if (textBox != null)
            //{
            //    string currentText = textBox.Text;

            //    // Получаем новый текст после вставки
            //    string newText = currentText.Insert(textBox.SelectionStart, e.Text);

            //    // Проверяем, соответствует ли новый текст формату: ровно 11 цифр
            //    Regex regex = new Regex(@"^\d{11}$");

            //    // Если новый текст не соответствует паттерну, отменяем ввод
            //    e.Handled = !regex.IsMatch(newText);
            //}
        }



        private readonly string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        public bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            return Regex.IsMatch(email, emailPattern);
        }

        private void PasportInfo_GotFocus(object sender, RoutedEventArgs e)
        {
            //if (PasportInfo.Text == "серия и номер без пробела")
            //{
            //    PasportInfo.Text = string.Empty;
            //}

        }

        private void PasportInfo_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(PasportInfo.Text)) {
            //    PasportInfo.Text = "серия и номер без пробела";
            //}
        }

        private void PhoneNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            //if (PhoneNumber.Text == "Номер телефона без символов")
            //{
            //    PhoneNumber.Text = string.Empty;
            //}
        }

        private void PhoneNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(PhoneNumber.Text))
            //{
            //    PhoneNumber.Text = "Номер телефона без символов";
            //}
        }
    }
    
    }

