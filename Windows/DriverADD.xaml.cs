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
        
        byte[] imageBytes; //Временное хранение изображения в байтах
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string FN = FullName.Text; //ФИО
            int PN = Convert.ToInt32(PhoneNumber.Text); //Номер телефона

            //Паспортные данные-----------------------------------------------------------
            string Pi = $"{PasportInfo.Text}{PasportInfo1.Text} "; //Объединение паспортных данных в одну сплошную строку
            int PI = Convert.ToInt32(Pi);

            if (!ValidateEmail(Email.Text)) //Валидация почты
            {
                MessageBox.Show("Некорректный email.", "Ошибка");
                return;
            }
            //--------------------------------------------------------------------------------
            string mail = Email.Text; // почта
            string remark = Zamechanie.Text;// замечание
                                            //--------------------------------------------------------------------------------
            string city1 = City1.Text;
            string city2 = City2.Text;
            string adress1 = Adress1.Text;
            string adress2 = Adress2.Text;
            //--------------------------------------------------------------------------------
            string RegA = ($"{city1}, {adress1}"); //адрес регистр
            string ResA = ($"{city2}, {adress2}"); //адрес проживание
            string POW = PlaceOfWork.Text; //Место работы
            string post = Post.Text; //Должность
            byte[] image = imageBytes; //Изображение в байтах
            //--------------------------------------------------------------------------------
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
                catch (Exception ex) //Обработка исключений
                {
                    MessageBox.Show("Убедитесь что вы заполнили все " + ex.Message);
                    return;
                }
            }
            //Сброс всех полей после доабвления в таблицу------------------
            FullName.Text = string.Empty; //ФИО
            PasportInfo.Text = string.Empty; //Паспортные данные
            PhoneNumber.Text = string.Empty; // Номер телефона
            Email.Text = string.Empty; ; // почта
            Zamechanie.Text = string.Empty; // замечание
            PlaceOfWork.Text = string.Empty; ; //Место работы
            Post.Text = string.Empty; ; //Должность
                                        //Города-------------------------------------------------------                            
            City1.Text = string.Empty;
            City2.Text = string.Empty;
            //Адреса-------------------------------------------------------
            Adress1.Text = string.Empty;
            Adress2.Text = string.Empty;
            //Сброс фотографии---------------------------------------------
            string filePath = "/Image2.png";
            var bitmap = new BitmapImage(new Uri(filePath, UriKind.Relative));
            DriverImage.Source = bitmap;

        }
        private void DriverImage_Add(object sender, MouseButtonEventArgs e)
        { 
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.png;" // Фильтр .jpg и .png
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName; //Получение пути к изображению
                imageBytes = File.ReadAllBytes(filePath); //Преобразование изображения в байты

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
            }
        }
        private void PasportInfo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text) //Проверка на цифры
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }

            TextBox textBox = (TextBox)sender;  //Проверка на 4 символа
            string newText = textBox.Text + e.Text;
            if (newText.Length > 4)
            {
                e.Handled = true;
            }
        }
        private void PasportInfo1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)//Проверка на цифры
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }

            TextBox textBox = (TextBox)sender; //Проверка на 6 символов
            string newText = textBox.Text + e.Text;
            if (newText.Length > 6)
            {
                e.Handled = true;
            }
        }
        private void PhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)//Проверка на цифры
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }

            TextBox textBox = (TextBox)sender; //Проверка на 11 символов
            string newText = textBox.Text + e.Text;
            if (newText.Length > 11)
            {
                e.Handled = true;
            }
        }

        private readonly string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; //паттерн для проверки почты
        public bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            return Regex.IsMatch(email, emailPattern);
        } //метод для проверки почты
    }  
}

