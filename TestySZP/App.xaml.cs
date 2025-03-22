using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;
using TestySZP.Data;

namespace TestySZP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DatabaseHelper.InitializeDatabase(); // ✅ Inicializace DB při startu aplikace
                                                 // ✅ Nastavení české kultury pro zobrazení datumu
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("cs-CZ");
        }
    }

}
