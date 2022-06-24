using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using TestAssignment.Infrastructure.Commands;
using TestAssignment.Models;
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

        #region AssetsList : AssetsRoot - Сторінка зі списком криптовалюти
        private AssetsRoot _AssetsList = new AssetsRoot();
        public AssetsRoot AssetsList { get => _AssetsList; set => Set(ref _AssetsList, value); }
        #endregion

        #region CanLoadData : bool - Можливысть завантаження даних
        private bool _CanLoadData = true;
        public bool CanLoadData {  get => _CanLoadData; set => Set(ref _CanLoadData, value); }
        #endregion
        /*-------------------------------------Методи-------------------------------------------*/

        private AssetsRoot LoadData()
        {
            string url = "https://www.cryptingup.com/api/assets?size=10";
            //string url = "https://api.coincap.io/v2/assets";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                AssetsRoot assets = JsonConvert.DeserializeObject<AssetsRoot>(response);
                return assets;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Не вдалось отримати даны про приптовалюту", ex);
            }
        }

        private async Task<AssetsRoot> LoadDataAsync()
        {            
            string url = "https://www.cryptingup.com/api/assets?size=10";
            //string url = "https://api.coincap.io/v2/assets";
            if (AssetsList.Next != "")
                url += "&&start=" + AssetsList.Next;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                AssetsRoot assets = JsonConvert.DeserializeObject<AssetsRoot>(response);                
                return assets;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Не вдалось отримати даны про приптовалюту", ex);
            }            
        }


        /*-------------------------------------Команди-------------------------------------------*/
        #region MyRegion

        private ICommand _LoadDataCommand;
        public ICommand LoadDataCommand => _LoadDataCommand
            ??= new LambdaCommandAsync(OnLoadDataCommandExecuted, CanLoadDataCommandExecute);
        private bool CanLoadDataCommandExecute() => CanLoadData;
        private async Task OnLoadDataCommandExecuted()
        {
            //CanLoadData = !CanLoadData;
            Status = "Кнопку натиснуто";
            AssetsList = await LoadDataAsync();
            //CanLoadData = !CanLoadData;
        }

        #endregion

        /*-----------------------------------Конструктор-------------------------------------------*/

        public MainWindowViewModel()
        {

        }
    }
}
