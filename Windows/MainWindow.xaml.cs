using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _attempts = 0;
        private DispatcherTimer _unblockTimer;

        private void StartUnblockTimer()
        {
            _unblockTimer = new DispatcherTimer();
            _unblockTimer.Interval = TimeSpan.FromSeconds(1); // Проверка каждую секунду
            _unblockTimer.Tick += CheckUnblockStatus;
            _unblockTimer.Start();
        }
        private void CheckUnblockStatus(object sender, EventArgs e)
        {
            if (DateTime.Now >= Properties.Settings.Default.BlockedUntil)
            {
                Properties.Settings.Default.IsBlocked = false;
                Properties.Settings.Default.BlockedUntil = DateTime.MinValue; // Сброс времени блокировки
                Properties.Settings.Default.Save(); // Сохранение состояния
                _unblockTimer.Stop(); // Остановка таймера
                MessageBox.Show("Вы разблокированы");
            }
        }

        public MainWindow()
        {

            InitializeComponent();

            if (Properties.Settings.Default.IsBlocked && Properties.Settings.Default.BlockedUntil > DateTime.Now)
            {

                StartUnblockTimer();
            }
            else
            {
                Properties.Settings.Default.IsBlocked = false; // Убедитесь, что состояние разблокировано
            }
        }
        


        private void LogIn(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.IsBlocked) //Не пускаем, если IsBlocked = true
            {
                MessageBox.Show($"Вы заблокированы. Пожалуйста, подождите до {Properties.Settings.Default.BlockedUntil.ToString("HH:mm:ss")}");
                return;
                
            }
            

                using (var conect = new SqliteConnection("Data Source=GIBDD.db"))
                {
                    conect.Open();

                    string query = "select login, password from Inspector where login = @login";

                    using (var command = new SqliteCommand(query, conect))
                    {
                        command.Parameters.AddWithValue("@login", "Inspector");
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string username = reader["login"].ToString();
                                string password = reader["password"].ToString();

                                string LogIn = LogIN.Text;
                                string PassWord = PassWorD.Text;


                                if (username == LogIn && password == PassWord)
                                {
                                    mainmenu mainmenu = new mainmenu();
                                    mainmenu.Show();
                                    this.Close();
                                }
                                else
                                {
                                    _attempts++;
                                    MessageBox.Show("Неверный логин или пароль");
                                    if (_attempts >= 3) //Ставим IsBlocked = True если 3 раза неправильно ввели данные
                                    {
                                    Properties.Settings.Default.IsBlocked = true;
                                    Properties.Settings.Default.BlockedUntil = DateTime.Now.AddMinutes(1);
                                    Properties.Settings.Default.Save();
                                    StartUnblockTimer();
                                    MessageBox.Show("Вы заблокированы на минуту.");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Подключение к бд отсутствует");
                            }
                        }
                    }
                }
        }

        private void LogIN_GotFocus(object sender, RoutedEventArgs e)
        {
         if(LogIN.Text == "Логин")
            {
                LogIN.Text = string.Empty;
            }

        }
        private void LogIN_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LogIN.Text))
            {
                LogIN.Text = "Логин";
            }
        }
        private void PassWorD_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PassWorD.Text))
            {
                LogIN.Text = "Пароль";
            }
            
        }
        private void PassWorD_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PassWorD.Text == "Пароль")
            {
                PassWorD.Text = string.Empty;
            }
        }
    }
}