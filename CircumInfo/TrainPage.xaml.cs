using CircumInfo.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Email;
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
    public sealed partial class TrainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public TrainPage()
        {
            this.InitializeComponent();

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
            string trainId = (string)e.NavigationParameter;
            try
            {
                getStops(trainId);
            }
            catch (Exception ex)
            {
                printErrorMessage(ex, (string)trainId);
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
                mail.Body = "C'è un errore nell'orario, controlla il treno " + trainID + "\n" + ex.Message;
                await EmailManager.ShowComposeNewEmailAsync(mail);
            }
        }

        private async void getStops(string traid)
        {
            Train treno = await DBSource.getTrain(traid);
            NomeTreno.Text = "Treno " + treno.ID;
            if (treno.Ferial == "Y")
                Title.Text = "FERMATE - ESCLUSO DOMENICA E FESTIVI";
            if (treno.Festivo)
                Title.Text = "FERMATE - SOLO DOMENICA E FESTIVI";
            if (treno.NoG == "Y")
                Title.Text += " - NON GARANTITO";
            itemListView.ItemsSource = treno.ArrayOfStop;
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

        private void itemListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var StationId = ((Stop)e.ClickedItem).StopID;
            if (!Frame.Navigate(typeof(SectionPage), StationId))
            {

            }
        }

    }
}
