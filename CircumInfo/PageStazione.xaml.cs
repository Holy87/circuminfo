using CircumInfo.Common;
using CircumInfo.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.StartScreen;
using Windows.ApplicationModel;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.ApplicationModel.Email;
using System.Threading.Tasks;

// Il modello di applicazione hub è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkId=391641

namespace CircumInfo
{
    public sealed partial class SectionPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        public TrainStop trainStop;
        public IEnumerable<Partenza> partenze;
        private bool showAll = (bool)Settings.ShowAll;

        public string AppbarTileId
        {
            get
            {
                return "stazione" + trainStop.Id;
            }
        }

        public SectionPage()
        {
            this.InitializeComponent();
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Application.Current.Resuming += new EventHandler<object>(App_Resuming);
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
        }

        private void App_Resuming(object sender, object e)
        {
            ToggleAppBarButton(!SecondaryTile.Exists(AppbarTileId));
            System.Diagnostics.Debug.WriteLine("RESUMING");
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
        /// salvati durante la nuova creazione di una pagina in una sessione precedente.
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
            // TODO: creare un modello dati appropriato per il DOMINIO problematico per sostituire i dati di esempio.
            //var group = await SampleDataSource.GetGroupAsync((string)e.NavigationParameter);
            //this.DefaultViewModel["Group"] = group;
            //MessageDialog msg = new MessageDialog(e.NavigationParameter.ToString());
            //await msg.ShowAsync();
            int stopId = (int)e.NavigationParameter;
            trainStop = await DBSource.getStop(stopId);
            ToggleAppBarButton(!SecondaryTile.Exists(AppbarTileId));
            updateFavButton(!Settings.inFavs(trainStop.Id));
            Titolo.Text = trainStop.Name;
            getPartenze((bool)Settings.ShowAll);
            if (Settings.Spot)
            {
                Ad.Visibility = Windows.UI.Xaml.Visibility.Visible;
                UpdateLayout();
            }
            
            
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

        private async void getPartenze(bool show)
        {
            try
            {
                progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                if (show)
                    partenze = await DBSource.getPartenze(trainStop.Id);
                else
                    partenze = await DBSource.getPartenze(trainStop.Id, DateTime.Now);
                itemListView.ItemsSource = partenze;
                progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            catch (InvalidTrainStopException ex)
            {
                printErrorMessage(ex, ex.IDTreno);
            }
            
        }

        private async Task<IEnumerable<Partenza>> listaPartenze(bool show)
        {
            try
            {
                IEnumerable<Partenza> partenze;
                if (show)
                    partenze = await DBSource.getPartenze(trainStop.Id, true);
                else
                    partenze = await DBSource.getPartenze(trainStop.Id, DateTime.Now, true);
                return partenze;
            }
            catch (InvalidTrainStopException ex)
            {
                printErrorMessage(ex, ex.IDTreno);
                return null;
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
            System.Diagnostics.Debug.WriteLine("NAVIGATING");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            System.Diagnostics.Debug.WriteLine("NAV2");
        }

        #endregion

        private void itemListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var trainID = "";
            try
            {
                trainID = ((Partenza)e.ClickedItem).ID;
                Frame.Navigate(typeof(TrainPage), trainID);
            } catch (Exception ex) {
                printErrorMessage(ex, (string)trainID);
            }
            
        }

        private async void printErrorMessage(Exception ex, string trainID)
        {
            MessageDialog dialog = new MessageDialog("Evidentemente c'è un problema per questo treno. Vuoi segnalare il malfunzionamento?");
            dialog.Title = "Errore";
            UICommand v1 = new UICommand("Sì");
            v1.Id = "1";
            UICommand v2 = new UICommand("No");
            v2.Id = "2";
            dialog.Commands.Add(v1);
            dialog.Commands.Add(v2);
            IUICommand result = await dialog.ShowAsync();
            if (result != null && result.Id.ToString() == "1")
            {
                EmailMessage mail = new EmailMessage();
                mail.Subject = "Segnalazione di un errore nell'applicazione";
                mail.To.Add(new EmailRecipient("holy87@outlook.it", "Circum Info"));
                mail.Body = "C'è un errore nell'orario della stazione di " + trainStop.Name + ", controlla il treno " + trainID + "\n" + ex.Message;
                await EmailManager.ShowComposeNewEmailAsync(mail);
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            bool lvte = !SecondaryTile.Exists(this.AppbarTileId);
            if (!lvte)
            {
                removeTile();
            }
            else
            {
                addTile();
            }
            
        }

        private async void removeTile()
        {
            SecondaryTile tileSecondaria = new SecondaryTile(this.AppbarTileId);
            bool unpinned = await tileSecondaria.RequestDeleteAsync();
            ToggleAppBarButton(unpinned);
        }

        private async void addTile()
        {
            SecondaryTile tileSecondaria = new SecondaryTile();
            tileSecondaria.Arguments = getArguments();
            tileSecondaria.DisplayName = trainStop.Name;
            tileSecondaria.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/TileS150.png");
            tileSecondaria.TileId = AppbarTileId;
            tileSecondaria.VisualElements.ShowNameOnSquare150x150Logo = true;
            tileSecondaria.VisualElements.ForegroundText = ForegroundText.Dark;
            ToggleAppBarButton(false);
            await tileSecondaria.RequestCreateAsync();
            
        }

        private string getArguments()
        {
            return "st," + trainStop.Id;
        }

        /*
         * Questo metodo serve a cambiare la grafica del comando pin a start
         * **/
        private void ToggleAppBarButton(bool showPinButton)
        {
            if (showPinButton)
            {
                this.Pinna.Label = "Aggiungi a Start";
                this.Pinna.Icon = new SymbolIcon(Symbol.Pin);
            }
            else
            {
                this.Pinna.Label = "Rim. da Start";
                this.Pinna.Icon = new SymbolIcon(Symbol.UnPin);
            }
            this.CommandBar.UpdateLayout();
        }

        private void Preferiti_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.inFavs(trainStop.Id))
            {
                Settings.removeFavs(trainStop.Id);
                updateFavButton(true);
            }
            else
            {
                Settings.addFavs(trainStop);
                updateFavButton(false);
            }
        }

        private void updateFavButton(bool p)
        {
            if (p)
            {
                this.Preferiti.Label = "Agg. ai preferiti";
                this.Preferiti.Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///Assets/appbar.star.add.png") };
            }
            else
            {
                this.Preferiti.Label = "Rim dai preferiti";
                this.Preferiti.Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///Assets/appbar.star.minus.png") };
            }
            this.CommandBar.UpdateLayout();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HubPage));
            Frame.BackStack.Clear();
        }

        private void Mappa_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MapStations), new double[] { trainStop.Latitude, trainStop.Longitude });
        }

        private async void VediTutto_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<string> direzioni = new ObservableCollection<string>();
            direzioni.Add("Tutte");
            IEnumerable<Partenza> part;
            try
            {
                part = await listaPartenze(true);
            } catch
            {
                return;
            }
            string[] dire = part.Select(dirz => dirz.Direction).ToArray();
            foreach (string direzione in dire)
            {
                if (!direzioni.Contains(direzione))
                    direzioni.Add(direzione);
            }
            DBSource.TempDirezioni = direzioni;
            DBSource.TempMostraTutto = showAll;
            DBSource.DialogOk = false;
            FilterDialog filtro = new FilterDialog();
            try
            {
                await filtro.ShowAsync();
                if (DBSource.DialogOk)
                {
                    System.Diagnostics.Debug.WriteLine("OKDIALOG");
                    System.Diagnostics.Debug.WriteLine(DBSource.tempDirezione);
                    showAll = DBSource.TempMostraTutto;
                    System.Diagnostics.Debug.WriteLine("MOSTRATUTTO = " + showAll);
                    if (DBSource.TempMostraTutto)
                        partenze = await DBSource.getPartenze(trainStop.Id, DBSource.tempDirezione);
                    else
                        partenze = await DBSource.getPartenze(trainStop.Id, DateTime.Now, DBSource.tempDirezione);
                    itemListView.ItemsSource = partenze;
                }
            }
            catch (Exception) { }

        }

        private async void Segnale_Click(object sender, RoutedEventArgs e)
        {
            EmailMessage mail = new EmailMessage();
            mail.Subject = "Segnalazione di orario Treni";
            mail.To.Add(new EmailRecipient("holy87@outlook.it", "Treni Circumvesuviana"));
            mail.Body = "C'è un errore nell'orario della stazione di " + trainStop.Name;
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }
    }
}
