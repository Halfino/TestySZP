using System.Windows;
using System.Windows.Input;
using TestySZP.Views;
using TestySZP.Helpers;

namespace TestySZP.ViewModels
{
    public class MainViewModel
    {
        public ICommand OpenPersonManagementCommand { get; }
        public ICommand OpenQuestionManagementCommand { get; }
        public ICommand OpenTestGenerationCommand { get; }

        public MainViewModel()
        {
            OpenPersonManagementCommand = new RelayCommand(_ => OpenPersonManagement());
            OpenQuestionManagementCommand = new RelayCommand(_ => OpenQuestionManagement());

        }

        private void OpenPersonManagement()
        {
            PersonWindow personWindow = new PersonWindow();
            personWindow.ShowDialog();
        }

        private void OpenQuestionManagement()
        {
            QuestionWindow questionWindow = new QuestionWindow();
            questionWindow.ShowDialog();
        }
        

    }
}
