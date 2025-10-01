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
                SqliteCommand command = new SqliteCommand();
                command.Connection = conect;
                //command.CommandText = "INSERT INTO Inspector (Login, Password) VALUES ('Inspector', 'Inspector')";
                //command.ExecuteNonQuery();

                string LogIn = LogIN.Text;
                string PassWord = PassWorD.Text;

                if (LogIn == "inspector" && PassWord == "inspector")
                {
                    mainmenu mainmenu = new mainmenu();
                    mainmenu.Show();
                    this.Close();
                }
                else
                {
                    attemt++;
                    if (attemt >= 3)
                    {
                        MessageBox.Show("Вы заблокированы на 1 минуту");
                    }
                    else
                    {
                        MessageBox.Show("Попрбуйте вести пароль еще раз");
                    }
                }
            }
        }
    }
}