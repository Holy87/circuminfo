using CircumInfo.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Maps;
using System.Collections.ObjectModel;
using Windows.UI;

// Il modello di elemento per la pagina base è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace CircumInfo
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class MapStations : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public const string appbarTileId = "MapTile";
        private Geopoint location;
        private MapIcon userLocation;
        private bool onStation = false;
        public MapStations()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            Mappa.MapServiceToken = "As0jxcDkh1f0rVi8lWgry8ultbvEjTCfQ6PuBwSXAMTIeJIsN9ibwQdqRqJywErQ";
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
            MapInitialPosition();
            
            if (Settings.Spot)
            {
                Ad.Visibility = Windows.UI.Xaml.Visibility.Visible;
                UpdateLayout();
            }
        }

        private void initializeData()
        {
            SetUserLocation();
            loadStations();

        }

        private async void SetUserLocation()
        {
            var locationL = await getUserLocation();
            if (locationL == null)
            {
                return;
            }
            location = (Geopoint)locationL;
            setIconLocation();
            if (onStation == false)
                centerMap(false);
        }

        private void setIconLocation()
        {
            if (location == null)
            {
                return;
            }
            if (userLocation == null)
            {
                userLocation = new MapIcon();
                userLocation.Location = location;
                userLocation.NormalizedAnchorPoint = new Point(0.5, 1.0);
                userLocation.Title = "Posizione";
                userLocation.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Posizione.png"));
                Mappa.MapElements.Add(userLocation);
            }
            else
            {
                userLocation.Location = location;
            }
        }

        private async Task<Geopoint> getUserLocation()
        {
            TestoRicerca.Text = "Sto controllando dove sei...";
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //StatusBar statusBar = StatusBar.GetForCurrentView();
            Geopoint point = null;
            //await statusBar.ProgressIndicator.ShowAsync();
            try
            {
                
                Geoposition geop = await Settings.getPosition();
                point = new Geopoint(new BasicGeoposition()
                {
                    Latitude = geop.Coordinate.Point.Position.Latitude,
                    Longitude = geop.Coordinate.Point.Position.Longitude,
                });
                //progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                TestoRicerca.Text = "";
            }
            catch (DisabledPositionException ex)
            {
                TestoRicerca.Text = ex.Message;
                //progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            }
            catch (PositionNotFoundException ex)
            {
                TestoRicerca.Text = ex.Message;
                //progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                //progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                TestoRicerca.Text = "Non riesco a verificare la tua posizione";
            }
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //await statusBar.ProgressIndicator.HideAsync();
            return point;
            
        }

        /// <summary>
        /// Ottiene l'elemento <see cref="NavigationHelper"/> associato a questa <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Ottiene il modello di visualizzazione per questa <see cref="Page"/>.
        /// È possibile sostituirlo con un modello di visualizzazione fortemente tipizzato.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Popola la pagina con il contenuto passato durante la navigazione.  Vengono inoltre forniti eventuali stati
        /// salvati durante la ricreazione di una pagina in una sessione precedente.
        /// </summary>
        /// <param name="sender">
        /// Origine dell'evento. In genere <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Dati evento che forniscono il parametro di navigazione passato a
        /// <see cref="Frame.Navigate(Type, Object)"/> quando la pagina è stata inizialmente richiesta e
        /// un dizionario di stato mantenuto da questa pagina nel corso di una sessione
        /// precedente.  Lo stato è null la prima volta che viene visitata una pagina.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            
            if (e.NavigationParameter != null)
            {
                double[] coordinate = e.NavigationParameter as double[];
                onStation = true;
                centerMap(coordinate);
            }
            initializeData();
            
        }

        private async void centerMap(double[] coordinate)
        {
            System.Diagnostics.Debug.WriteLine("CENTRO");
            Geopoint location = new Geopoint(new BasicGeoposition()
            {
                Latitude = coordinate[0],
                Longitude = coordinate[1]
            });
            await Mappa.TrySetViewAsync(location, 15);  
        }

        /// <summary>
        /// Mantiene lo stato associato a questa pagina in caso di sospensione dell'applicazione o se la
        /// viene scartata dalla cache di navigazione.  I valori devono essere conformi ai requisiti di
        /// serializzazione di <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">Origine dell'evento. In genere <see cref="NavigationHelper"/></param>
        /// <param name="e">Dati di evento che forniscono un dizionario vuoto da popolare con
        /// uno stato serializzabile.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region Registrazione di NavigationHelper

        /// <summary>
        /// I metodi forniti in questa sezione vengono utilizzati per consentire a
        /// NavigationHelper di rispondere ai metodi di navigazione della pagina.
        /// <para>
        /// La logica specifica della pagina deve essere inserita nel gestore eventi per  
        /// <see cref="NavigationHelper.LoadState"/>
        /// e <see cref="NavigationHelper.SaveState"/>.
        /// Il parametro di navigazione è disponibile nel metodo LoadState 
        /// oltre allo stato della pagina conservato durante una sessione precedente.
        /// </para>
        /// </summary>
        /// <param name="e">Fornisce dati per i metodi di navigazione e
        /// i gestori eventi che non sono in grado di annullare la richiesta di navigazione.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private async void centerMap(bool refresh)
        {
            if (refresh)
            {
                await getUserLocation();
                if (location == null)
                {
                    MessageDialog msgbox = new MessageDialog("La posizione è disattivata. Attiva i dati di posizione dalle impostazioni del telefono.");
                    msgbox.Title = "Posizione disattivata";
                    await msgbox.ShowAsync();
                    return;
                }
            }
            await Mappa.TrySetViewAsync(location, 15);            
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            centerMap(true);
        }

        private void MapInitialPosition()
        {
            MapInitialPosition(40.850632, 14.275445);
        }

        private void MapInitialPosition(double lat, double lon)
        {
            Geopoint init = new Geopoint(new BasicGeoposition()
            {
                Latitude = lat,
                Longitude = lon
            });
            //await Mappa.TrySetViewAsync(init, 15);
            Mappa.Center = init;
            Mappa.ZoomLevel = 10;
        }

        //private void ToggleAppBarButton(bool showPinButton)
        //{
        //    if (showPinButton)
        //    {
        //        this.StartPin.Label = "Agg a start";
        //        this.StartPin.Icon = new SymbolIcon(Symbol.Pin);
        //    }
        //    else
        //    {
        //        this.StartPin.Label = "Rim da Start";
        //        this.StartPin.Icon = new SymbolIcon(Symbol.UnPin);
        //    }
        //    this.StartPin.UpdateLayout();
        //}

        private async void loadStations()
        {
            //ObservableCollection<TrainStop> stations = await DBSource.Stations;
            //foreach (TrainStop stop in stations)
            //{
            //    MapIcon icon = new MapIcon();
            //    icon.Title = stop.Name;
            //    icon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            //    icon.Location = new Geopoint(new BasicGeoposition()
            //        {
            //            Latitude = stop.Latitude,
            //            Longitude = stop.Longitude
            //        });
            //    icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Segnalino.png"));
            //    Mappa.MapElements.Add(icon);
            //}
            Puspins.ItemsSource = await DBSource.getStopsPreload();
            Puspins.ItemsSource = await DBSource.Stations;
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
        }

        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BitmapIcon border = sender as BitmapIcon;
            TrainStop stop = border.DataContext as TrainStop;
            if (!Frame.Navigate(typeof(SectionPage), stop.Id))
            {

            }
        }
    }
}
