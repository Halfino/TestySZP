using System.IO;
using System.Windows;
using TestySZP.Views;

namespace TestySZP
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests"));
            var loginWindow = new LoginWindow();
            // Ukážeme jako dialog
            bool? result = loginWindow.ShowDialog();

            if (result == true)
            {
                var main = new MainWindow();
                MainWindow = main;
                main.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}
