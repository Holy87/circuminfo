using CircumInfo.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
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

// Il modello di elemento per la pagina base è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace CircumInfo
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class Impostazioni : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Impostazioni()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            posizione.IsOn = Settings.Location;
            Tutto.IsChecked = Settings.ShowAll;
            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
            ComboTreni.IsEnabled = (bool)!Tutto.IsChecked;
            ComboTreni.SelectedIndex = Settings.MaxTrains;
            if (Settings.Spot)
            {
                AdRemove.Visibility = Windows.UI.Xaml.Visibility.Visible;
                TestoAd.Visibility = Windows.UI.Xaml.Visibility.Visible;
                RectAd.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            //if (Settings.Spot)
            //    AdRemove.Visibility = Windows.UI.Xaml.Visibility.Visible;
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

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.Location = posizione.IsOn;
        }

        private void Tutto_Click(object sender, RoutedEventArgs e)
        {
            Settings.ShowAll = Tutto.IsChecked;
            ComboTreni.IsEnabled = (bool)!Tutto.IsChecked;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("Grazie per aver supportato l'applicazione!");
            dialog.Title = "Pubblicità rimosse";
            //try
            //{
                ProductPurchaseStatus status = await Shop.buyNoAds();
                switch (status)
                {
                    case ProductPurchaseStatus.Succeeded:
                        {
                            AdRemove.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            Settings.Spot = false;
                            break;
                        }
                    case ProductPurchaseStatus.AlreadyPurchased:
                        {
                            AdRemove.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            Settings.Spot = false;
                            break;
                        }
                    case ProductPurchaseStatus.NotPurchased:
                        {
                            dialog.Content = "Se vuoi cambiare idea, il pulsante è sempre qui :)";
                            dialog.Title = "Azione annullata";
                            break;
                        }
                }
            //}
            //catch (Exception ex)
            //{
            //    dialog.Title = "Errore acquisto";
            //    dialog.Content = "C'è stato un errore nell'acquisto del prodotto :( Forse non c'è connessione?";
            //    System.Diagnostics.Debug.WriteLine(ex.Message);
            //}
            
            await dialog.ShowAsync();
        }

        private void ComboTreni_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Settings.MaxTrains = ComboTreni.SelectedIndex;
            }
            catch (System.NullReferenceException)
            {
            }
            
        }
    }
}
