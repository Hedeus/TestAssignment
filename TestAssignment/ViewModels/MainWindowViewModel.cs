using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using TestAssignment.Infrastructure.Commands;
using TestAssignment.Infrastructure.Commands.Base;
using TestAssignment.Models;
using TestAssignment.ViewModels.Base;

namespace TestAssignment.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        /*-------------------------------------Поля-------------------------------------------*/

        #region Поля

        #region Title : string - Заголовок вікна
        private string _Title = "Головне вікно";
        public string Title { get => _Title; set => Set(ref _Title, value); }
        #endregion

        #region Status : string - Стан
        private string _Status = "0";
        public string Status { get => _Status; set => Set(ref _Status, value); }
        #endregion

        #region CanLoadData : bool - Можливість завантаження даних
        private bool _CanLoadData = true;
        public bool CanLoadData { get => _CanLoadData; set => Set(ref _CanLoadData, value); }
        #endregion

        #region CurrentAssetsPage : int - номер поточної сторінки криптовалюти
        private int _CurrentAssetsPage = 0;
        public int CurrentAssetsPage
        {
            get => _CurrentAssetsPage; set
            {
                if (Set(ref _CurrentAssetsPage, value))
                    _AssetsViewSource?.View.Refresh();
            }
        }
        #endregion

        #region AssetPagesCount : int - кількість сторінок валюти
        private int _AssetPagesCount = 0;
        public int AssetPagesCount
        {
            get => _AssetPagesCount; set
            {
                if (Set(ref _AssetPagesCount, value))
                    _AssetsViewSource?.View.Refresh();
            }
        }
        #endregion

        #region SelectedAsset : Asset - Обрана криптовалюта
        private Asset _SelectedAsset;
        public Asset SelectedAsset { get => _SelectedAsset; set => Set(ref _SelectedAsset, value); }
        #endregion

        #region MarketsList : MarketsRoot - Сторінка зі списком варіантів обміну
        private MarketsRoot _MarketsList = new MarketsRoot();
        public MarketsRoot MarketsList { get => _MarketsList; set => Set(ref _MarketsList, value); }
        #endregion

        #region SelectedMarket : Market - Обраний варіант обміну
        private Market _SelectedMarket;
        public Market SelectedMarket { get => _SelectedMarket; set => Set(ref _SelectedMarket, value); }
        #endregion

        #region SelectedTab : int - Обрана вкладка
        private int _SelectedTab = 0;
        public int SelectedTab { get => _SelectedTab; set => Set(ref _SelectedTab, value); }
        #endregion        

        #region CurExchange : Exchange - Інформація по торгівельну платформу
        private Exchange _CurExchange = new();
        public Exchange CurExchange { get => _CurExchange; set => Set(ref _CurExchange, value); }
        #endregion        

        #region _ItemsPerPage : int - (ЗАПЛАНОВАНО) кількість запісів на сторінці
        private int _ItemsPerPage = 15;
        #endregion

        #region _Next : string - наступна сторінка для завантаження в API
        private string _Next; 
        #endregion

        #region Filter : string - Рядок вільтру
        private string _Filter;
        public string Filter
        {
            get => _Filter;
            set
            {
                if (Set(ref _Filter, value))
                    _AssetsViewSource?.View.Refresh();
            }
        }
        #endregion

        #region Assets : ObservableCollection<Asset> - колекція завантажених валют
        private ObservableCollection<Asset> _Assets = new();
        public ObservableCollection<Asset> Assets
        {
            get => _Assets; set
            {
                if (Set(ref _Assets, value))
                {
                    _AssetsViewSource = new CollectionViewSource
                    {
                        Source = value
                    };

                    _AssetsViewSource.View.Filter = item =>
                    {
                        int index = Assets.IndexOf(item as Asset);
                        return index >= (CurrentAssetsPage - 1) * _ItemsPerPage && index < CurrentAssetsPage * _ItemsPerPage;
                    };
                    _AssetsViewSource.Filter += FilterAssets;
                    _AssetsViewSource.View.Refresh();

                    OnPropertyChanged(nameof(AssetsView));
                }
            }
        }
        #endregion

        #region _AssetsViewSource та AssetsView - елементи для відображення колекції з фільтрацією
        private CollectionViewSource _AssetsViewSource;
        public ICollectionView AssetsView => _AssetsViewSource?.View;
        #endregion

        #region IsFilterFocused : bool - Чи є рядок фільтрації в фокусі
        private bool _IsFilterFocused = false;
        public bool IsFilterFocused { get => _IsFilterFocused; set => Set(ref _IsFilterFocused, value); }
        #endregion

        #region НЕ ВИКОРИСТОВУЄТЬСЯ
        //#region _LoadedAssets : AssetsRoot - остання завантажена сторінка криптовалюти
        //private AssetsRoot _LoadedAssets = new();
        ////public AssetsRoot LoadedAssets { get => _LoadedAssets; set => Set(ref _LoadedAssets, value); }
        //#endregion

        //#region _AllLoadedAssets : List<AssetsRoot> - Усі завантажені сторінки криптовалюти
        //private List<AssetsRoot> _AllLoadedAssets = new();
        //#endregion

        //#region CurrentAssetList : List<Asset> - поточний список валюти
        //private List<Asset> _CurrentAssetList = new();
        //public List<Asset> CurrentAssetList { get => _CurrentAssetList; set => Set(ref _CurrentAssetList, value); }
        //#endregion 
        #endregion

        #endregion

        /*-------------------------------------Методи-------------------------------------------*/

        #region Методи

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
                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync().ConfigureAwait(false));
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

        #region LoadMarketDataAsync - Асинхронний метод завантаження даних про варіанти обміну
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
                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync().ConfigureAwait(false));
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

        #region LoadExchangeInfoAsync - Асинхронний метод завантаження даних про торгівельну платформу
        private async Task<ExchangeRoot> LoadExchangeInfoAsync(string exchandeId)
        {
            string url = "https://www.cryptingup.com/api/exchanges/" + exchandeId;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync().ConfigureAwait(false));
                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                ExchangeRoot exchange = JsonConvert.DeserializeObject<ExchangeRoot>(response);
                return exchange;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Не вдалось отримати даны про приптовалюту", ex);
            }
        }
        #endregion

        #region FilterAssets - метод, що забезпечує фільтрацію валюти
        void FilterAssets(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(Filter))
            {
                e.Accepted = true;
            }
            else
            {
                Asset asset = (Asset)e.Item;
                e.Accepted = asset.AssetId.ToUpper().Contains(Filter.ToUpper()) || asset.Name.ToUpper().Contains(Filter.ToUpper());
            }
        } 
        #endregion

        #endregion

        /*-------------------------------------Команди-------------------------------------------*/
        #region Команди

        #region LoadAssetsDataCommand - Команда завантаження даних про валюту
        private ICommand _LoadAssetsDataCommand;
        public ICommand LoadAssetsDataCommand => _LoadAssetsDataCommand
            ??= new LambdaCommandAsync(OnLoadAssetsDataCommandExecuted, CanLoadAssetsDataCommandExecute);
        private bool CanLoadAssetsDataCommandExecute() => CanLoadData;
        private async Task OnLoadAssetsDataCommandExecuted()
        {
            CanLoadData = false;            
            AssetsRoot assetsRoot = await LoadAssetsDataAsync();            
            CurrentAssetsPage = 1;
            AssetPagesCount = 1;
            Assets = new ObservableCollection<Asset>(assetsRoot.Assets);
            _Next = assetsRoot.Next;
            Filter = "";
            /*CanLoadData = await Task.Run(() => true).ConfigureAwait(false)*/;
            CanLoadData = true;            
            Status = Assets.Count.ToString();
        }
        #endregion

        #region LoadMoreAssetsCommand - Команда завантаження нової сторінки валюти
        private ICommand _LoadMoreAssetsCommand;
        public ICommand LoadMoreAssetsCommand => _LoadMoreAssetsCommand
            ??= new LambdaCommandAsync(OnLoadMoreAssetsCommandExecuted, CanLoadMoreAssetsCommandExecute);
        private bool CanLoadMoreAssetsCommandExecute() => CanLoadData;
        private async Task OnLoadMoreAssetsCommandExecuted()
        {
            CanLoadData = false;
            AssetsRoot assetRoot = await LoadAssetsDataAsync(_Next);            
            foreach (var asset in assetRoot.Assets)
                Assets.Add(asset);
            _Next = assetRoot.Next;
            CanLoadData = true;            
            Status = Assets.Count.ToString();
        }
        #endregion        

        #region LoadMarketDataCommand - Команда завантаження даних про варіанти обміну
        private ICommand _LoadMarketDataCommand;
        public ICommand LoadMarketDataCommand => _LoadMarketDataCommand
            ??= new LambdaCommandAsync(OnLoadMarketDataCommandExecuted, CanLoadMarketDataCommandExecute);
        private bool CanLoadMarketDataCommandExecute() => !(SelectedAsset is null);
        private async Task OnLoadMarketDataCommandExecuted()
        {
            //CanLoadData = !CanLoadData;            
            MarketsList = await LoadMarketDataAsync(SelectedAsset.AssetId);
            SelectedTab = 1;
            //CanLoadData = !CanLoadData;
        }
        #endregion

        #region HyperlinkCommand - Завантаження даних про обмінник та перехід на сторінку обмінника
        private ICommand _HyperlinkCommand;
        public ICommand HyperlinkCommand => _HyperlinkCommand
            ??= new LambdaCommandAsync(OnHyperlinkCommandExecuted, CanHyperlinkCommandExecute);
        private bool CanHyperlinkCommandExecute() => true;
        private async Task OnHyperlinkCommandExecuted()
        {
            string url = "ttps://www.cryptingup.com/api/exchanges/" + SelectedMarket.ExchangeId;
            ExchangeRoot ex = await LoadExchangeInfoAsync(SelectedMarket.ExchangeId).ConfigureAwait(false);
            CurExchange = ex.Exchange;
            Process.Start(new ProcessStartInfo { FileName = CurExchange.Website, UseShellExecute = true });
        }
        #endregion

        #region FilterResetCommand - Скидання фільтра
        private ICommand _FilterResetCommand;
        public ICommand FilterResetCommand => _FilterResetCommand
            ??= new LambdaCommand(OnFilterResetCommandExecuted, CanFilterResetCommandExecute);
        private bool CanFilterResetCommandExecute() => Filter?.Length > 0;
        private void OnFilterResetCommandExecuted()
        {
            Filter = "";
        }
        #endregion

        #endregion

        /*-----------------------------------Конструктор-------------------------------------------*/

        public MainWindowViewModel()
        {
            
        }
    }
}
