using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento per la pagina vuota è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace CircumInfo
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class Register : Page
    {
        public bool PassOk;
        public bool UserOk;
        public bool MailOk;

        public Register()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Richiamato quando la pagina sta per essere visualizzata in un Frame.
        /// </summary>
        /// <param name="e">Dati dell'evento in cui vengono descritte le modalità con cui la pagina è stata raggiunta.
        /// Questo parametro viene in genere utilizzato per configurare la pagina.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(PasswordBox.Password.Length < 8)
            {
                PassOk = false;
                //PasswordBox.Style = (Style)Application.Current.Resources["PasswordBoxError"];
            } else
            {
                //PasswordBox.Style = (Style)Application.Current.Resources["PasswordBoxCorrect"];
            }
        }

        private void NicknameBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void MailBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void RepeatPass_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
