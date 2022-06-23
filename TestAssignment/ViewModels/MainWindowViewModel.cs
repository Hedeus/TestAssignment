using System.IO;
using System.Net;
using System.Windows.Input;
using TestAssignment.Infrastructure.Commands;
using TestAssignment.ViewModels.Base;

namespace TestAssignment.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Title : string - Заголовок вікна
        private string _Title = "Головне вікно";
        public string Title { get => _Title; set => Set(ref _Title, value); }
        #endregion

        #region Status : string - Стан
        private string _Status = "Готово";
        public string Status { get => _Status; set => Set(ref _Status, value); }
        #endregion
        /*-------------------------------------Методи-------------------------------------------*/

        private string LoadData()
        {
            string url = "https://www.cryptingup.com/api/assets?size=10";
            //string url = "https://api.coincap.io/v2/assets";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string response;
            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }
            return response;
        }

        /*-------------------------------------Команди-------------------------------------------*/
        #region MyRegion

        private ICommand _LoadDataCommand;
        public ICommand LoadDataCommand => _LoadDataCommand
            ??= new LambdaCommand(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);
        private bool CanLoadDataCommandExecute() => true;
        private void OnLoadDataCommandExecuted()
        {
            Status = "Кнопку натиснуто";
            string response = LoadData();
        }

        #endregion

        /*-----------------------------------Конструктор-------------------------------------------*/

        public MainWindowViewModel()
        {

        }
    }
}
