using System.Windows;
using TestySZP.Views;

namespace TestySZP
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
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
