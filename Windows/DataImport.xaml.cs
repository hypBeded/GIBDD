using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
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
                // Вызов метода импорта данных из JSON файла
                ImportDataFromJson(filesPath);
            }
        }
        // Основной метод импорта данных из JSON файла
        private void ImportDataFromJson(string filePath)
        {
            try
            {
                // Чтение всего содержимого JSON файла
                string jsonContent = System.IO.File.ReadAllText(filePath);

                // Десериализация JSON в JsonDocument для работы с динамическими данными
                using (JsonDocument document = JsonDocument.Parse(jsonContent))
                {
                    // Получение корневого элемента (предполагается, что это массив)
                    JsonElement root = document.RootElement;

                    // Проверка, что корневой элемент является массивом
                    if (root.ValueKind != JsonValueKind.Array)
                    {
                        MessageBox.Show("Ошибка: JSON файл должен содержать массив объектов");
                        return;
                    }

                    int processedCount = 0; // Счетчик обработанных записей
                    int totalCount = root.GetArrayLength(); // Общее количество записей в файле

                    // Построчная обработка каждого штрафа из массива
                    foreach (JsonElement fineElement in root.EnumerateArray())
                    {
                        try
                        {
                            // Обработка одной записи о штрафе
                            ProcessSingleFine(fineElement);
                            processedCount++;
                        }
                        catch (Exception ex)
                        {
                            // Обработка ошибок для отдельной записи
                            Console.WriteLine($"Ошибка обработки штрафа: {ex.Message}");
                        }
                    }

                    // Показ результата обработки
                    MessageBox.Show($"Успешно обработано {processedCount} из {totalCount} штрафов");
                }
            }
            catch (JsonException jsonEx)
            {
                // Обработка ошибок формата JSON
                MessageBox.Show($"Ошибка формата JSON: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                // Обработка общих ошибок импорта
                MessageBox.Show($"Ошибка импорта: {ex.Message}");
            }
        }

        // Метод обработки одной записи о штрафе
        private void ProcessSingleFine(JsonElement fineElement)
        {
            // Извлечение данных из JSON элемента с проверкой на наличие полей
            string postNum = GetStringProperty(fineElement, "PostNum");          // Номер авто или путь к фото
            string postDate = GetStringProperty(fineElement, "PostDate");        // Дата нарушения
            int sum = GetIntProperty(fineElement, "Sum");                        // Сумма штрафа
            int koapCode = GetIntProperty(fineElement, "KoapCode");              // Код статьи КоАП
            string koapText = GetStringProperty(fineElement, "KoapText");        // Текст статьи
            string address = GetStringProperty(fineElement, "Address");          // Адрес нарушения
            string dataCar = GetStringProperty(fineElement, "DataCar");          // Данные авто
            int dataDriver = GetIntProperty(fineElement, "DataDriver");          // Данные водителя

            // Проверка наличия обязательных полей
            if (string.IsNullOrEmpty(postDate) || sum == 0 || koapCode == 0 || string.IsNullOrEmpty(koapText))
            {
                throw new Exception("Отсутствуют обязательные поля в записи штрафа");
            }

            // Проверка, является ли поле PostNum путем к фотографии
            bool isPhoto = IsPhotoPath(postNum);

            if (isPhoto)
            {
                // Если это фото - открываем диалог для ручного распознавания номера
                ShowRecognitionDialog(postNum, postDate, sum, koapCode, koapText, address, dataCar, dataDriver);
            }
            else
            {
                // Если это обычный номер - сразу добавляем в базу данных
                InsertFineToDatabase(postNum, postDate, sum, koapCode, koapText, address, dataCar, dataDriver);
            }
        }

        // Вспомогательный метод для получения строкового свойства из JsonElement
        private string GetStringProperty(JsonElement element, string propertyName)
        {
            // Проверка наличия свойства в JSON объекте
            if (element.TryGetProperty(propertyName, out JsonElement property) &&
                property.ValueKind != JsonValueKind.Null)
            {
                return property.GetString() ?? ""; // Возврат строки или пустой строки если null
            }
            return ""; // Возврат пустой строки если свойство отсутствует
        }

        // Вспомогательный метод для получения целочисленного свойства из JsonElement
        private int GetIntProperty(JsonElement element, string propertyName)
        {
            // Проверка наличия свойства в JSON объекте
            if (element.TryGetProperty(propertyName, out JsonElement property) &&
                property.ValueKind != JsonValueKind.Null)
            {
                // Попытка получить значение как Int32
                if (property.TryGetInt32(out int value))
                {
                    return value;
                }
            }
            return 0; // Возврат 0 если свойство отсутствует или не является числом
        }

        // Метод проверки, является ли строка путем к изображению
        private bool IsPhotoPath(string postNum)
        {
            if (string.IsNullOrEmpty(postNum))
                return false;

            // Приведение к нижнему регистру для унификации проверки
            string lowerPostNum = postNum.ToLower();

            // Проверка различных форматов указания фотографий
            return lowerPostNum.StartsWith("photo:") ||
                   lowerPostNum.StartsWith("image:") ||
                   lowerPostNum.EndsWith(".jpg") ||
                   lowerPostNum.EndsWith(".png") ||
                   lowerPostNum.EndsWith(".jpeg") ||
                   lowerPostNum.EndsWith(".bmp") ||
                   lowerPostNum.EndsWith(".gif");
        }

        // Метод отображения диалога распознавания номера с фотографии
        private void ShowRecognitionDialog(string photoPath, string postDate, int sum, int koapCode,
                                         string koapText, string address, string dataCar, int dataDriver)
        {
            // Создание нового окна для диалога распознавания
            Window dialog = new Window();
            dialog.Title = "Распознавание номера автомобиля с фотографии";
            dialog.Width = 600;
            dialog.Height = 500;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Owner = this;

            // Создание основной сетки диалогового окна
            System.Windows.Controls.Grid dialogGrid = new System.Windows.Controls.Grid();
            dialogGrid.Margin = new Thickness(10);

            // Определение строк сетки
            dialogGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = GridLength.Auto }); // Заголовок
            dialogGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }); // Область фото
            dialogGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = GridLength.Auto }); // Метка поля ввода
            dialogGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = GridLength.Auto }); // Поле ввода
            dialogGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = GridLength.Auto }); // Панель кнопок

            // Заголовок с информацией о файле
            System.Windows.Controls.TextBlock headerText = new System.Windows.Controls.TextBlock();
            headerText.Text = $"Фотография: {System.IO.Path.GetFileName(photoPath)}";
            headerText.FontWeight = FontWeights.Bold;
            headerText.Margin = new Thickness(0, 0, 0, 10);
            System.Windows.Controls.Grid.SetRow(headerText, 0);

            // Область для отображения фотографии
            System.Windows.Controls.Image photoImage = new System.Windows.Controls.Image();
            photoImage.Margin = new Thickness(0, 0, 0, 10);
            photoImage.Stretch = System.Windows.Media.Stretch.Uniform; // Сохранять пропорции
            photoImage.HorizontalAlignment = HorizontalAlignment.Center;
            photoImage.VerticalAlignment = VerticalAlignment.Center;
            System.Windows.Controls.Grid.SetRow(photoImage, 1);

            try
            {
                // Очистка префиксов photo: и image: если они есть
                string cleanPath = photoPath;
                if (photoPath.StartsWith("photo:"))
                    cleanPath = photoPath.Substring(6); // Убираем "photo:"
                else if (photoPath.StartsWith("image:"))
                    cleanPath = photoPath.Substring(6); // Убираем "image:"

                // Проверка существования файла
                if (System.IO.File.Exists(cleanPath))
                {
                    // Загрузка изображения из файла
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(cleanPath, UriKind.RelativeOrAbsolute);
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    photoImage.Source = bitmap;
                }
                else
                {
                    // Если файл не найден, показываем заглушку
                    ShowImageNotFound(photoImage, cleanPath);
                }
            }
            catch (Exception ex)
            {
                // В случае ошибки загрузки изображения показываем сообщение об ошибке
                ShowImageError(photoImage, ex.Message);
            }

            // Метка для поля ввода номера
            System.Windows.Controls.TextBlock inputLabel = new System.Windows.Controls.TextBlock();
            inputLabel.Text = "Введите номер автомобиля с фотографии:";
            inputLabel.Margin = new Thickness(0, 0, 0, 5);
            System.Windows.Controls.Grid.SetRow(inputLabel, 2);

            // Поле для ввода распознанного номера
            System.Windows.Controls.TextBox numberTextBox = new System.Windows.Controls.TextBox();
            numberTextBox.Margin = new Thickness(0, 0, 0, 10);
            numberTextBox.Height = 25;
            System.Windows.Controls.Grid.SetRow(numberTextBox, 3);

            // Панель с кнопками действий
            System.Windows.Controls.StackPanel buttonPanel = new System.Windows.Controls.StackPanel();
            buttonPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            buttonPanel.HorizontalAlignment = HorizontalAlignment.Right;
            System.Windows.Controls.Grid.SetRow(buttonPanel, 4);

            // Кнопка сохранения с распознанным номером
            System.Windows.Controls.Button saveButton = new System.Windows.Controls.Button();
            saveButton.Content = "Сохранить номер";
            saveButton.Width = 120;
            saveButton.Height = 30;
            saveButton.Margin = new Thickness(0, 0, 10, 0);
            saveButton.Click += (s, e) =>
            {
                string recognizedNumber = numberTextBox.Text.Trim();

                // Проверка введенного номера
                if (string.IsNullOrEmpty(recognizedNumber))
                {
                    MessageBox.Show("Пожалуйста, введите номер автомобиля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Сохранение записи с распознанным номером
                InsertFineToDatabase(recognizedNumber, postDate, sum, koapCode, koapText, address, dataCar, dataDriver);
                dialog.DialogResult = true;
            };

            // Кнопка отправки на ручное рассмотрение (если номер не распознается)
            System.Windows.Controls.Button manualReviewButton = new System.Windows.Controls.Button();
            manualReviewButton.Content = "Не распознать";
            manualReviewButton.Width = 100;
            manualReviewButton.Height = 30;
            manualReviewButton.Margin = new Thickness(0, 0, 10, 0);
            manualReviewButton.Click += (s, e) =>
            {
                // Сохранение записи с NULL вместо номера
                InsertFineToDatabase(null, postDate, sum, koapCode, koapText, address, dataCar, dataDriver);

                // Отправка на дополнительное рассмотрение
                SendForManualReview(photoPath, postDate, sum, koapCode, koapText);

                dialog.DialogResult = true;
            };

            // Кнопка отмены обработки текущей записи
            System.Windows.Controls.Button cancelButton = new System.Windows.Controls.Button();
            cancelButton.Content = "Отмена";
            cancelButton.Width = 80;
            cancelButton.Height = 30;
            cancelButton.Click += (s, e) =>
            {
                dialog.DialogResult = false;
            };

            // Добавление кнопок на панель
            buttonPanel.Children.Add(saveButton);
            buttonPanel.Children.Add(manualReviewButton);
            buttonPanel.Children.Add(cancelButton);

            // Добавление всех элементов в сетку диалога
            dialogGrid.Children.Add(headerText);
            dialogGrid.Children.Add(photoImage);
            dialogGrid.Children.Add(inputLabel);
            dialogGrid.Children.Add(numberTextBox);
            dialogGrid.Children.Add(buttonPanel);

            dialog.Content = dialogGrid;

            // Показ диалогового окна и ожидание результата
            bool? result = dialog.ShowDialog();
        }

        // Метод для отображения сообщения "Файл не найден"
        private void ShowImageNotFound(System.Windows.Controls.Image imageControl, string filePath)
        {
            var border = new System.Windows.Controls.Border();
            border.BorderBrush = System.Windows.Media.Brushes.Red;
            border.BorderThickness = new Thickness(2);
            border.Background = System.Windows.Media.Brushes.LightYellow;
            border.Margin = new Thickness(0, 0, 0, 10);

            var stackPanel = new System.Windows.Controls.StackPanel();
            stackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Center;
            stackPanel.Margin = new Thickness(10);

            var warningIcon = new System.Windows.Controls.TextBlock();
            warningIcon.Text = "⚠️";
            warningIcon.FontSize = 24;
            warningIcon.HorizontalAlignment = HorizontalAlignment.Center;

            var messageText = new System.Windows.Controls.TextBlock();
            messageText.Text = $"Файл не найден:\n{filePath}";
            messageText.TextWrapping = TextWrapping.Wrap;
            messageText.TextAlignment = TextAlignment.Center;
            messageText.FontWeight = FontWeights.Bold;

            stackPanel.Children.Add(warningIcon);
            stackPanel.Children.Add(messageText);
            border.Child = stackPanel;

            // Заменяем Image на Border с сообщением об ошибке
            var parent = imageControl.Parent as System.Windows.Controls.Grid;
            if (parent != null)
            {
                int row = System.Windows.Controls.Grid.GetRow(imageControl);
                parent.Children.Remove(imageControl);
                System.Windows.Controls.Grid.SetRow(border, row);
                parent.Children.Add(border);
            }
        }

        // Метод для отображения сообщения об ошибке загрузки
        private void ShowImageError(System.Windows.Controls.Image imageControl, string errorMessage)
        {
            var border = new System.Windows.Controls.Border();
            border.BorderBrush = System.Windows.Media.Brushes.Red;
            border.BorderThickness = new Thickness(2);
            border.Background = System.Windows.Media.Brushes.LightPink;
            border.Margin = new Thickness(0, 0, 0, 10);

            var stackPanel = new System.Windows.Controls.StackPanel();
            stackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Center;
            stackPanel.Margin = new Thickness(10);

            var errorIcon = new System.Windows.Controls.TextBlock();
            errorIcon.Text = "❌";
            errorIcon.FontSize = 24;
            errorIcon.HorizontalAlignment = HorizontalAlignment.Center;

            var messageText = new System.Windows.Controls.TextBlock();
            messageText.Text = $"Ошибка загрузки:\n{errorMessage}";
            messageText.TextWrapping = TextWrapping.Wrap;
            messageText.TextAlignment = TextAlignment.Center;
            messageText.FontWeight = FontWeights.Bold;

            stackPanel.Children.Add(errorIcon);
            stackPanel.Children.Add(messageText);
            border.Child = stackPanel;

            // Заменяем Image на Border с сообщением об ошибке
            var parent = imageControl.Parent as System.Windows.Controls.Grid;
            if (parent != null)
            {
                int row = System.Windows.Controls.Grid.GetRow(imageControl);
                parent.Children.Remove(imageControl);
                System.Windows.Controls.Grid.SetRow(border, row);
                parent.Children.Add(border);
            }
        }
        // Метод вставки данных о штрафе в базу данных
        private void InsertFineToDatabase(string postNum, string postDate, int sum, int koapCode,
                                        string koapText, string address, string dataCar, int dataDriver)
        {
            // Использование подключения к базе данных
            using (var connect = new SqliteConnection("Data Source=GIBDD.db"))
            {
                connect.Open(); // Открытие подключения

                // SQL запрос для вставки данных
                string insertSql = @"
                INSERT INTO Fines (PostNum, PostDate, Sum, KoapCode, KoapText, Address, DataCar, DataDriver)
                VALUES (@PostNum, @PostDate, @Sum, @KoapCode, @KoapText, @Address, @DataCar, @DataDriver)";

                // Создание и настройка команды SQL
                using (var command = new SqliteCommand(insertSql, connect))
                {
                    // Добавление параметров с проверкой на NULL
                    if (postNum == null)
                    {
                        command.Parameters.AddWithValue("@PostNum", DBNull.Value); // NULL если номер не распознан
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@PostNum", postNum); // Распознанный номер
                    }

                    // Добавление остальных параметров
                    command.Parameters.AddWithValue("@PostDate", postDate);
                    command.Parameters.AddWithValue("@Sum", sum);
                    command.Parameters.AddWithValue("@KoapCode", koapCode);
                    command.Parameters.AddWithValue("@KoapText", koapText);
                    command.Parameters.AddWithValue("@Address", address ?? "");
                    command.Parameters.AddWithValue("@DataCar", dataCar ?? "");
                    command.Parameters.AddWithValue("@DataDriver", dataDriver);

                    // Выполнение команды вставки
                    command.ExecuteNonQuery();
                }
            }
        }

        // Метод отправки записи на ручное рассмотрение
        private void SendForManualReview(string photoPath, string postDate, int sum, int koapCode, string koapText)
        {
            // В реальном приложении здесь может быть:
            // - Отправка уведомления
            // - Запись в лог
            // - Добавление в очередь на ручную обработку

            // Эмуляция отправки на рассмотрение
            MessageBox.Show($"Запись отправлена на дополнительное рассмотрение.\nФото: {System.IO.Path.GetFileName(photoPath)}",
                           "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
