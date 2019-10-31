using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CircumInfo.Common;

// Il modello di elemento per la finestra di dialogo del contenuto è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace CircumInfo
{
    public sealed partial class FilterDialog : ContentDialog
    {
        public FilterDialog()
        {
            this.InitializeComponent();
            Filtro.IsChecked = !DBSource.TempMostraTutto;
            Direzione.ItemsSource = DBSource.TempDirezioni;
            try
            {
                Direzione.SelectedIndex = 0;
            }
            catch (Exception) { }
            
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            DBSource.TempMostraTutto = !(bool)Filtro.IsChecked;
            if ((string)Direzione.SelectedItem == "Tutte")
                DBSource.tempDirezione = "";
            else
                try
                {
                    DBSource.tempDirezione = (string)Direzione.SelectedItem;
                }
                catch (Exception)
                {
                    DBSource.tempDirezione = "";
                }

                
            DBSource.DialogOk = true;
            System.Diagnostics.Debug.WriteLine("DIREZIONE: " + (string)Direzione.SelectedItem);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            DBSource.DialogOk = false;
        }

        private void Filtro_Checked(object sender, RoutedEventArgs e)
        {
            DBSource.TempMostraTutto = !(bool)Filtro.IsChecked;
        }
    }
}
