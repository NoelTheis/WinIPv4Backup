using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using WinIPv4Backup.Classes;

namespace WinIPv4Backup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            tbInterface.Text = "";

            var networkInterfaces = NetworkInterfaceManager.GetNetworkInterfaceList();
            cbInterfaces.ItemsSource = networkInterfaces;

        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            var networkInterface = cbInterfaces.SelectedItem as NetworkInterface;
            if (networkInterface != null)
            {
                FileInfo fileInfo = new FileInfo(Environment.CurrentDirectory + @"\ethernet.json");
                NetworkInterfaceManager.SaveIPv4ToFile(networkInterface, fileInfo);
            }

        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            var networkInterface = cbInterfaces.SelectedItem as NetworkInterface;
            if (networkInterface != null)
            {
                FileInfo fileInfo = new FileInfo(Environment.CurrentDirectory + @"\ethernet.json");
                NetworkInterfaceManager.LoadIPv4FromFile(networkInterface, fileInfo);
            }
        }

        private void cbInterfaces_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var networkInterface = cbInterfaces.SelectedItem as NetworkInterface;
            if (networkInterface != null)
            {
                btnRestore.IsEnabled = true;
                btnBackup.IsEnabled = true;
                tbInterface.Text = NetworkInterfaceManager.FormatInterfaceToString(networkInterface);
            }
            else
            {
                btnRestore.IsEnabled = false;
                btnBackup.IsEnabled = false;
            }
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            StringBuilder exceptionMessage = new StringBuilder();
            StringBuilder exceptionLog = new StringBuilder();
            if (e != null && e.Message != null)
            {
                exceptionMessage.AppendLine(e.Message);
                exceptionLog.AppendLine(e.ToString());

                if (e.InnerException != null && e.InnerException.Message != null)
                {
                    exceptionMessage.AppendLine(e.InnerException.Message);
                    exceptionLog.AppendLine(e.InnerException.ToString());
                }
            }

            string path = Environment.CurrentDirectory + @"\" + "_UnhandledExceptionLog.txt";
            File.WriteAllText(path, exceptionLog.ToString());
        }
    }
}
