using System.IO;
using System.Text;
using System;
using System.Windows;
using GraphPartitorApp.Models;
using System.Windows.Documents;
using System.Collections.Generic;
using ProgrammInterface;
using System.Windows.Shapes;
using System.Linq;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Threading;

namespace GraphPartitorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int[,]? matrix { get; set; }
        private IPartiteGraph? _partitor { get; set; }

        bool _isProcess = false;
        bool _isDark = false;

        (List<Vertex>, List<Vertex>) partitedGraph { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnBrowseAssemblyClick(object sender, RoutedEventArgs e)
        {
            string path;
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Filter = "Файлы сборки (*.dll)|*.dll";

            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;

                try
                {
                    await Task.Run(() =>
                    {
                        _partitor = Algorithm.GetPartitior(path);
                    });
                    BrowseButton.IsEnabled = true;
                    AssemblyPath.Text = "Загружена сборка: " + path;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при загрузке сборки");
                }
            }
        }

        private async void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            string path;
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls";

            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;

                try
                {
                    await Task.Run(() =>
                    {
                        matrix = ExcelToMatrixAdapter.GetMatrix(path);
                    });

                    ToFilePath.Text = "Загружен файл: " + path;
                    ProcessFileButton.IsEnabled = !_isProcess;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при обработке файла");
                }
            }
        }

        private async void OnProcessFileClick(object sender, RoutedEventArgs e)
        {
            int checksCount;
            var a = "";
            var b = "";

            checksCount = int.TryParse(InputText.Text, out checksCount) && int.Parse(InputText.Text) > 0
                ? int.Parse(InputText.Text)
                : 1;

            ProcessFileButton.IsEnabled = false;

            _isProcess = true;

            await Task.Run(() =>
            {
                partitedGraph = _partitor!.PartiteGraph(matrix!, checksCount);

                a = string.Join(", ", partitedGraph.Item1.Select(x => x.Id));
                b = string.Join(", ", partitedGraph.Item2.Select(x => x.Id));
            });

            _isProcess = false;

            ProcessFileButton.IsEnabled = true;

            partA.Text = a;
            partB.Text = b;
        }

        private void ThemeSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (!_isDark)
            {
                Foreground = new SolidColorBrush(Colors.White);
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444"));

                BrowseAssembly.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3A3B4"));
                BrowseAssembly.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff7581"));

                BrowseButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3A3B4"));
                BrowseButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff7581"));

                ProcessFileButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3A3B4"));
                ProcessFileButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff7581"));

                partApre.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A668B"));
                partBpre.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EBAE9A"));

                ThemeSwitch.Content = "\u2600Светлая тема";
                ThemeSwitch.Background = new SolidColorBrush(Colors.White);
                ThemeSwitch.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                Foreground = new SolidColorBrush(Colors.Black);
                Background = new SolidColorBrush(Colors.White);

                BrowseAssembly.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3EE6B8"));
                BrowseAssembly.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7CF542"));

                BrowseButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3EE6B8"));
                BrowseButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7CF542"));

                ProcessFileButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3EE6B8"));
                ProcessFileButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7CF542"));

                partApre.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC799"));
                partBpre.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9DB0F3"));

                ThemeSwitch.Content = char.ConvertFromUtf32(0x1F314) + "Тёмная тема";
                ThemeSwitch.Background = new SolidColorBrush(Colors.Black);
                ThemeSwitch.Foreground = new SolidColorBrush(Colors.White);
            }

            _isDark = !_isDark;
        }
    }
}
