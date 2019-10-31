using CircumInfo.Common;
using CircumInfo.Data;
using Microsoft.Advertising.Mobile.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Store;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Il modello di applicazione hub è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkId=391641

namespace CircumInfo
{
    /// <summary>
    /// Pagina in cui viene visualizzata una raccolta raggruppata di elementi.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private TextBlock testoControllo;
        private ListView listView, preferiti;
        private bool positionNotFound = false;
        private AppBarButton refresh;
        private int nearestStopId = 0;

        public HubPage()
        {
            
            this.InitializeComponent();
            //CommandBar.PrimaryCommands.Remove(Accept);
            // L'hub è supportato solo in orientamento verticale
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);

            
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
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: creare un modello dati appropriato per il DOMINIO problematico per sostituire i dati di esempio
            var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = sampleDataGroups;
            checkMessages();
            if (!Settings.Location)
            {
                Hub.DefaultSectionIndex = 1;
            }
            else
            {
                Hub.DefaultSectionIndex = 0;
            }
            checkVote();
        }

        private async void checkVote()
        {
            if (Settings.AppOpened == 6)
            {
                MessageDialog dialog = new MessageDialog("Che ne dici di dire agli altri ciò che pensi dell'app?");
                dialog.Title = "Dai un parere";
                UICommand v1 = new UICommand("OK!");
                v1.Id = "1";
                UICommand v2 = new UICommand("Non ora");
                v2.Id = "2";
                dialog.Commands.Add(v1);
                dialog.Commands.Add(v2);
                IUICommand result = await dialog.ShowAsync();
                if (result != null)
                {
                    if (result.Id.ToString() == "1")
                    {
                        Settings.AppOpened++;
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
                    }
                    else
                    {
                        Settings.AppOpened = -10;
                    }
                }
            }
            
        }
        /// <summary>
        /// Mostra un messaggio da ME.
        /// </summary>
        private async void checkMessages()
        {
            Message messaggio = await MessageSystem.checkMessages();
            if (messaggio != null)
            {
                MessageDialog dialog = new MessageDialog(messaggio.Text);
                dialog.Title = messaggio.Title;
                Settings.LastMessage = messaggio.ID;
                await dialog.ShowAsync();
            }
        }

        private async void NearestStop()
        {
            nearestStopId = 0;
            ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            testoControllo.Text = "Sto ottenendo la posizione...";
            try
            {
                Geoposition geoposition = await Settings.getPosition();
                Geopoint punto = new Geopoint(new BasicGeoposition()
                {
                    Latitude = geoposition.Coordinate.Point.Position.Latitude,
                    Longitude = geoposition.Coordinate.Point.Position.Longitude
                });
                testoControllo.Text = "Sto cercando la stazione più vicina...";
                TrainStop nearest = Settings.nearestStop(punto, await DBSource.Stations);
                nearestStopId = nearest.Id;
                testoControllo.Text = nearest.Name;
                listView.ItemsSource = await DBSource.getPartenze(nearest.Id, DateTime.Now, false, 7);
                positionNotFound = false;
                ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            } catch (DisabledPositionException ex) {
                testoControllo.Text = ex.Message;
                positionNotFound = true;
                ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            catch (PositionNotFoundException ex)
            {
                testoControllo.Text = ex.Message;
                positionNotFound = true;
                ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                
            }
            catch(ArgumentOutOfRangeException)
            {
                testoControllo.Text = "Errore. Non ci sono dati.";
                positionNotFound = false;
                ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            catch(System.UnauthorizedAccessException)
            {
                testoControllo.Text = "Posizione non disponibile.";
                positionNotFound = true;
                ProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            if (refresh != null)
            {
                refresh.IsEnabled = true;
            }
            System.Diagnostics.Debug.WriteLine(positionNotFound);
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
            // TODO: salvare qui lo stato univoco della pagina.
        }

        /// <summary>
        /// Mostra i dettagli di un gruppo su cui si è fatto clic in <see cref="SectionPage"/>.
        /// </summary>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var groupId = ((Linea)e.ClickedItem).Name;
            if (!Frame.Navigate(typeof(ItemPage), groupId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        /// <summary>
        /// Mostra i dettagli di un elemento su cui si è fatto clic in <see cref="ItemPage"/>
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Passa alla pagina di destinazione appropriata, configurando la nuova pagina
            // mediante il passaggio delle informazioni richieste come parametro di navigazione
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        #region Registrazione di NavigationHelper

        /// <summary>
        /// I metodi forniti in questa sezione vengono utilizzati per consentire a
        /// NavigationHelper di rispondere ai metodi di navigazione della pagina.
        /// <para>
        /// La logica specifica della pagina deve essere inserita nei gestori eventi per
        /// <see cref="NavigationHelper.LoadState"/>
        /// e <see cref="NavigationHelper.SaveState"/>.
        /// Il parametro di navigazione è disponibile nel metodo LoadState
        /// oltre allo stato della pagina conservato durante una sessione precedente.
        /// </para>
        /// </summary>
        /// <param name="e">Dati dell'evento in cui vengono descritte le modalità con cui la pagina è stata raggiunta.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(SearchStop)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(Impostazioni)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(MapStations)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(About)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void PositionCheck_Loaded(object sender, RoutedEventArgs e)
        {
            testoControllo = (TextBlock)sender;
            NearestStop();
        }

        private void PartenzeVicine_Loaded(object sender, RoutedEventArgs e)
        {
            listView = (ListView)sender;
        }

        private void Preferiti_Loaded(object sender, RoutedEventArgs e)
        {
            preferiti = (ListView)sender;
            Settings.caricaPreferiti();
            preferiti.ItemsSource = Settings.preferiti;
        }

        private void Preferiti_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            if (item != null)
            {
                TrainStop stazione = item.DataContext as TrainStop;
                Settings.removeFavs(stazione.Id);
            }
            
        }

        private void PartenzeVicine_ItemClick(object sender, ItemClickEventArgs e)
        {
            var trainID = ((Partenza)e.ClickedItem).ID;
            if (!Frame.Navigate(typeof(TrainPage), trainID))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private void Preferiti_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (preferiti.ReorderMode == ListViewReorderMode.Enabled)
                return;
            var StationId = ((TrainStop)e.ClickedItem).Id;
            if (!Frame.Navigate(typeof(SectionPage), StationId))
            {

            }
        }

        private async void AppBarButton_Click_4(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
        }

        private async void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            ListView elencoLinee = (ListView)sender;
            elencoLinee.ItemsSource = await DBSource.getTrattePreLoad();
            elencoLinee.ItemsSource = await DBSource.Tratte;
            ObservableCollection<Linea> trat = await DBSource.Tratte;
            System.Diagnostics.Debug.WriteLine(trat.Count);
        }

        private void PositionCheck_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (nearestStopId != 0)
            {
                Frame.Navigate(typeof(SectionPage), nearestStopId);
            }
        }

        private void Reorder_Click(object sender, RoutedEventArgs e)
        {
            preferiti.ReorderMode = ListViewReorderMode.Enabled;
            setCommandBarButtons(false);
        }

        private void setCommandBarButtons(bool state)
        {
            if (state)
            {
                CommandBar.PrimaryCommands.Add(Finder);
                CommandBar.PrimaryCommands.Add(Mapper);
                //CommandBar.PrimaryCommands.Remove(Accept);
                Linee.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Partenze.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                CommandBar.PrimaryCommands.Remove(Finder);
                CommandBar.PrimaryCommands.Remove(Mapper);
                //CommandBar.PrimaryCommands.Add(Accept);
                Linee.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Partenze.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            
        }

        private void Preferiti_LostFocus(object sender, RoutedEventArgs e)
        {
            preferiti.ReorderMode = ListViewReorderMode.Disabled;
        }

        private void Preferiti_Tapped(object sender, TappedRoutedEventArgs e)
        {
            preferiti.ReorderMode = ListViewReorderMode.Disabled;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Settings.position = null;
            refresh.IsEnabled = false;
            NearestStop();
        }

        private void Refresh_Loaded(object sender, RoutedEventArgs e)
        {
            refresh = sender as AppBarButton;
        }

        private void Ad_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Spot)
            {
                AdControl control = (AdControl)sender;
                control.Visibility = Windows.UI.Xaml.Visibility.Visible;
                UpdateLayout();
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            Settings.salvaPreferiti();
            preferiti.ReorderMode = ListViewReorderMode.Disabled;
            setCommandBarButtons(true);
        }




    }
}
