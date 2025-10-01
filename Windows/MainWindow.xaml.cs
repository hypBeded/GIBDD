using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
namespace Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int attemt = 0;
        public MainWindow()
        {
            InitializeComponent();
        }
        


        private void LogIn(object sender, RoutedEventArgs e)
        {
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


                            if (username == LogIn &&  password == PassWord )
                            {
                                mainmenu mainmenu = new mainmenu();
                                mainmenu.Show();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Неверный логин или пароль");

                            }
                        }
                        else
                        {
                            MessageBox.Show("Подключение к бд отсутствует");
                        }
                    }
                }

               
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                //string LogIn = LogIN.Text;
                //string PassWord = PassWorD.Text;

                //if (LogIn == "inspector" && PassWord == "inspector")
                //{
                //    mainmenu mainmenu = new mainmenu();
                //    mainmenu.Show();
                //    this.Close();
                //}
                //else
                //{
                //    attemt++;
                //    if (attemt >= 3)
                //    {
                //        MessageBox.Show("Вы заблокированы на 1 минуту");
                //    }
                //    else
                //    {
                //        MessageBox.Show("Попрбуйте вести пароль еще раз");
                //    }
                //}
            }
        }
    }
}