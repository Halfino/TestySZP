using System.Data.Entity;
using System.Windows;
using System.Windows.Input;
using TestySZP.Data;

namespace TestySZP.Views
{
    public partial class LoginWindow : Window
    {
        private const string ValidPassword = "8407";

        public LoginWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            string licenseText = "Copyright 2025 nrtm.Jan KORANDA - VÚ 8407\r\n\r\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\r\n\r\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\r\n\r\nTHE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
            MessageBox.Show(licenseText, "Licenční ujednání", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == ValidPassword)
            {
                this.Hide();
                // Otevři hlavní okno
                var main = new MainWindow();
                main.Show();

                // Zavři přihlašovací okno

            }
            else
            {
                MessageBox.Show("Nesprávné heslo.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        protected override void OnClosed(System.EventArgs e)
        {
            // Pokud není otevřené žádné jiné okno → ukončit aplikaci
            if (Application.Current.Windows.Count == 1)
            {
                Application.Current.Shutdown();
            }

            base.OnClosed(e);
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_Click(sender, e);
            }
        }
    }
}
