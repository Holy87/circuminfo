using CircumInfo.Common;
using CircumInfo.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di applicazione hub è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=391641

namespace CircumInfo
{
    /// <summary>
    /// Pagina in cui vengono visualizzati i dettagli per un singolo elemento all'interno di un gruppo.
    /// </summary>
    public sealed partial class ItemPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private string nomeLinea;
        public ObservableCollection<TrainStop> trains;

        public ItemPage()
        {
            this.InitializeComponent();
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);

            if (Settings.Spot)
            {
                Ad.Visibility = Windows.UI.Xaml.Visibility.Visible;
                UpdateLayout();
            }
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
        /// Popola la pagina con il contenuto passato durante la navigazione. Vengono inoltre forniti eventuali stati
        /// salvati durante la ricreazione di una pagina in una sessione precedente.
        /// </summary>
        /// <param name="sender">
        /// Origine dell'evento. In genere <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Dati evento che forniscono il parametro di navigazione passato a
        /// <see cref="Frame.Navigate(Type, Object)"/> quando la pagina è stata inizialmente richiesta e
        /// un dizionario di stato mantenuto da questa pagina nel corso di una sessione
        /// precedente.  Lo stato è null la prima volta che viene visitata una pagina.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: creare un modello dati appropriato per il DOMINIO problematico per sostituire i dati di esempio
            //var item = await SampleDataSource.GetItemAsync((string)e.NavigationParameter);
            //this.DefaultViewModel["Item"] = item;
            nomeLinea = (string)e.NavigationParameter;
            getData();
        }

        /// <summary>
        /// Mantiene lo stato associato a questa pagina in caso di sospensione dell'applicazione o se la
        /// viene scartata dalla cache di navigazione.  I valori devono essere conformi ai requisiti di
        /// serializzazione di <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">Origine dell'evento. In genere <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Dati di evento che forniscono un dizionario vuoto da popolare con
        /// uno stato serializzabile.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: salvare qui lo stato univoco della pagina.
        }

        private async void getData()
        {
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Linea linea = await DBSource.getLinea(nomeLinea);
            trains = new ObservableCollection<TrainStop>();
            try
            {
                foreach (TrainStop trainstop in await linea.Fermate)
                {
                    trains.Add(trainstop);
                }
            } catch (Exception)
            {
                //NIENTE
            }
            
            itemListView.ItemsSource = trains;
            NomeLinea.Text = linea.Name;
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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

        private void itemListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var StationId = ((TrainStop)e.ClickedItem).Id;
            if (!Frame.Navigate(typeof(SectionPage), StationId))
            {
                
            }
        }
    }
}
