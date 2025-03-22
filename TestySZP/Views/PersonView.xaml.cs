using System.Windows.Controls;

namespace TestySZP.Views
{
    public partial class PersonView : UserControl  // ✅ Musí být `partial`
    {
        public PersonView()
        {
            InitializeComponent();  // ✅ Toto musí být dostupné
        }
    }
}
