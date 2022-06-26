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

        #region SelectedAsset : Asset - Обрана криптовалюта
        private Asset _SelectedAsset;
        public Asset SelectedAsset { get => _SelectedAsset; set => Set(ref _SelectedAsset, value); }
        #endregion

        #region MarketsList : MarketsRoot - Сторінка зі списком торгівельних майданчиків
        private MarketsRoot _MarketsList = new MarketsRoot();
        public MarketsRoot MarketsList { get => _MarketsList; set => Set(ref _MarketsList, value); }
        #endregion

        #region SelectedMarket : Market - Обраний торгівельний майданчик
        private Market _SelectedMarket;
        public Market SelectedMarket { get => _SelectedMarket; set => Set(ref _SelectedMarket, value); }
        #endregion

        #region SelectedTab : int - Обрана вкладка
        private int _SelectedTab = 0;
        public int SelectedTab { get => _SelectedTab; set => Set(ref _SelectedTab, value); }
        #endregion

        #region BlockTab : bool - Блокування неактивної вкладки
        private bool _BlockTab = false;
        public bool BlockTab { get => _BlockTab; set => Set(ref _BlockTab, value); }
        #endregion

        /*-------------------------------------Методи-------------------------------------------*/

        #region LoadAssetsData - Синхронний метод завантаження даних про валюту //не використовується
        private AssetsRoot LoadAssetsData()
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
        #endregion

        #region LoadAssetsDataAsync - Асинхронний метод завантаження даних про валюту
        private async Task<AssetsRoot> LoadAssetsDataAsync()
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
        #endregion

        #region LoadMarketDataAsync - Асинхронний метод завантаження даних про торгівельні майданчики
        private async Task<MarketsRoot> LoadMarketDataAsync(string asset = null)
        {
            if (asset == null) return null;
            string url = "https://www.cryptingup.com/api/assets/" + asset + "/markets?size=10";            
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                MarketsRoot markets = JsonConvert.DeserializeObject<MarketsRoot>(response);
                return markets;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Не вдалось отримати даны про приптовалюту", ex);
            }
        }
        #endregion

        /*-------------------------------------Команди-------------------------------------------*/
        #region LoadAssetsDataCommand - Команда завантаження даних про валюту
        private ICommand _LoadAssetsDataCommand;
        public ICommand LoadAssetsDataCommand => _LoadAssetsDataCommand
            ??= new LambdaCommandAsync(OnLoadAssetsDataCommandExecuted, CanLoadAssetsDataCommandExecute);
        private bool CanLoadAssetsDataCommandExecute() => CanLoadData;
        private async Task OnLoadAssetsDataCommandExecuted()
        {
            //CanLoadData = !CanLoadData;
            Status = "Кнопку натиснуто";
            AssetsList = await LoadAssetsDataAsync();
            //CanLoadData = !CanLoadData;
        }
        #endregion

        #region LoadAssetsDataCommand - Команда завантаження даних про валюту
        private ICommand _LoadMarketDataCommand;
        public ICommand LoadMarketDataCommand => _LoadMarketDataCommand
            ??= new LambdaCommandAsync(OnLoadMarketDataCommandExecuted, CanLoadMarketDataCommandExecute);
        private bool CanLoadMarketDataCommandExecute() => !(SelectedAsset is null);
        private async Task OnLoadMarketDataCommandExecuted()
        {
            //CanLoadData = !CanLoadData;
            Status = "Кнопку 'Де купити' натиснуто";
            MarketsList = await LoadMarketDataAsync(SelectedAsset.AssetId);
            SelectedTab = 1;
            //CanLoadData = !CanLoadData;
        }
        #endregion

        /*-----------------------------------Конструктор-------------------------------------------*/

        public MainWindowViewModel()
        {

        }
    }
}
