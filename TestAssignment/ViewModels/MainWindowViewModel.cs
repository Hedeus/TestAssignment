using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        #region CanLoadData : bool - Можливысть завантаження даних
        private bool _CanLoadData = true;
        public bool CanLoadData { get => _CanLoadData; set => Set(ref _CanLoadData, value); }
        #endregion

        #region CurrentAssetsPage : int - номер поточної сторінки криптовалюти
        private int _CurrentAssetsPage = 0;
        public int CurrentAssetsPage { get => _CurrentAssetsPage; set => Set(ref _CurrentAssetsPage, value); }
        #endregion

        #region AssetPagesCount : int - кількість сторінок валюти
        private int _AssetPagesCount = 0;
        public int AssetPagesCount { get => _AssetPagesCount; set => Set(ref _AssetPagesCount, value); }
        #endregion

        #region _LoadedAssets : AssetsRoot - остання завантажена сторінка криптовалюти
        private AssetsRoot _LoadedAssets = new();
        //public AssetsRoot LoadedAssets { get => _LoadedAssets; set => Set(ref _LoadedAssets, value); }
        #endregion

        #region _AllLoadedAssets : List<AssetsRoot> - Усі завантажені сторінки криптовалюти
        private List<AssetsRoot> _AllLoadedAssets = new();
        #endregion

        #region CurrentAssetList : List<Asset> - поточний список валюти
        private List<Asset> _CurrentAssetList = new();
        public List<Asset> CurrentAssetList { get => _CurrentAssetList; set => Set(ref _CurrentAssetList, value); }
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
        private async Task<AssetsRoot> LoadAssetsDataAsync(string next = null)
        {
            string url = "https://www.cryptingup.com/api/assets?size=15";            
            if (!(next is null))
                url += "&&start=" + next;
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
                throw new InvalidOperationException("Не вдалось отримати дані про приптовалюту", ex);
            }
        }
        #endregion

        #region LoadMarketDataAsync - Асинхронний метод завантаження даних про торгівельні майданчики
        private async Task<MarketsRoot> LoadMarketDataAsync(string asset = null)
        {
            string url;
            if (asset == null)
            {
                url = "https://www.cryptingup.com/api/markets?size=10";
            }
            else
                url = "https://www.cryptingup.com/api/assets/" + asset + "/markets?size=15";            
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
            _LoadedAssets = await LoadAssetsDataAsync();
            _AllLoadedAssets.Clear();
            _AllLoadedAssets.Add(_LoadedAssets);
            CurrentAssetList = _LoadedAssets.Assets;
            CurrentAssetsPage = 1;
            AssetPagesCount = 1;
            //CanLoadData = !CanLoadData;            
        }
        #endregion

        #region LoadMoreAssetsCommand - Команда завантаження нової сторінки валюти
        private ICommand _LoadMoreAssetsCommand;
        public ICommand LoadMoreAssetsCommand => _LoadMoreAssetsCommand
            ??= new LambdaCommandAsync(OnLoadMoreAssetsCommandExecuted, CanLoadMoreAssetsCommandExecute);
        private bool CanLoadMoreAssetsCommandExecute() => CanLoadData;
        private async Task OnLoadMoreAssetsCommandExecuted()
        {
            //CanLoadData = !CanLoadData;
            CurrentAssetsPage++;
            if (CurrentAssetsPage <= _AllLoadedAssets.Count)
            {
                CurrentAssetList = _AllLoadedAssets[CurrentAssetsPage-1].Assets;                
                return;
            }
            Status = "Кнопку Вперед натиснуто";             
            _LoadedAssets = await LoadAssetsDataAsync(_AllLoadedAssets.Last().Next);            
            _AllLoadedAssets.Add(_LoadedAssets);
            CurrentAssetList = _LoadedAssets.Assets;            
            AssetPagesCount++;
            //CanLoadData = !CanLoadData;            
        }
        #endregion

        #region AssetsBackCommand - Повернення на попередню сторінку
        private ICommand _AssetsBackCommand;
        public ICommand AssetsBackCommand => _AssetsBackCommand
            ??= new LambdaCommand(OnAssetsBackCommandExecuted, CanAssetsBackCommandExecute);
        private bool CanAssetsBackCommandExecute() => CurrentAssetsPage > 1;
        private void OnAssetsBackCommandExecuted()
        {
            CurrentAssetsPage--;
            CurrentAssetList = _AllLoadedAssets[CurrentAssetsPage-1].Assets;
        }
        #endregion

        #region LoadMarketDataCommand - Команда завантаження даних про ринки
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
