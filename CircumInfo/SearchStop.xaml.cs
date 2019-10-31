using CircumInfo.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento per la pagina base è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace CircumInfo
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class SearchStop : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<TrainStop> treni;
        private IEnumerable<string> suggerimenti;

        public SearchStop()
        {
            this.InitializeComponent();
#if WINDOWS_UWP
            System.Diagnostics.Debug.WriteLine("WINDOWS 10");
#endif
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
            treni = await DBSource.getStops();
            suggerimenti = treni.OrderBy(a => a.Name).Select(x => x.Name);
            SearchBox.ItemsSource = treni.OrderBy(a => a.Name).Select(x => x.Name);

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


        private void Go_Click(object sender, RoutedEventArgs e)
        {
            searchResults();
            //var StationId = (TrainStop)SearchBox.sele
            //if (!Frame.Navigate(typeof(SectionPage), StationId))
            //{

            //}
        }

        private async void searchResults()
        {
            int idStazione = searchByName(SearchBox.Text);
            if (idStazione == 0)
            {
                if (SearchBox.Text == "noAdsPlz!")
                {
                    Settings.Spot = false;
                    MessageDialog msg = new MessageDialog("Pubblicità disattivata.");
                    await msg.ShowAsync();
                } else {
                    MessageDialog msg = new MessageDialog("Controlla che il nome inserito corrisponda a quello di una stazione.");
                    msg.Title = "Stazione non trovata";
                    await msg.ShowAsync();
                }
            }
            else
            {
                if (!Frame.Navigate(typeof(SectionPage), idStazione))
                {

                }
            }
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (suggerimenti == null)
            {
                return;
            }
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                try
                {
                    SearchBox.ItemsSource = (sender.Text.Length > 1) ? suggerimenti.Where(x => x.ToLower().Contains(sender.Text.ToLower())) : null;// new string[] {"Ricerca..."};
                    //await Task<string[]>.Run(() => {return this.getSuggestions(sender.Text);});
                }
                catch (ArgumentNullException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

            }
        }

        private int searchByName(string query)
        {
            try
            {
                TrainStop stazione = treni.Single(x => x.Name.Equals(query, StringComparison.OrdinalIgnoreCase));
                return stazione.Id;
            } catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return 0;
            }
        }

        private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.ToString() == "Enter")
            {
                searchResults();
            }
        }

        private void SuggestionsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("CERCO");
            searchResults();
        }

        private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (!args.SelectedItem.ToString().Equals("Ricerca..."))
            {
                SearchBox.Text = args.SelectedItem.ToString();
                searchResults();
            } else
            {
                SearchBox.Text = "";
            }
            
        }

        private void SearchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.ToString() == "Enter" && SearchBox.Text != "")
            {
                searchResults();
            }
        }
    }
}
