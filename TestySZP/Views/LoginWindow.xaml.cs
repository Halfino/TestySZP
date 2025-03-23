using System.Windows;

namespace TestySZP.Views
{
    public partial class LoginWindow : Window
    {
        private const string ValidPassword = "8407";

        public LoginWindow()
        {
            InitializeComponent();
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
    }
}
