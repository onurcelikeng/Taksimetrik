using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Taksimetrik.View
{
    public sealed partial class StartScreen : Page
    {

        public StartScreen()
        {
            this.InitializeComponent();
        }


        public async void Message(string body, string head)
        {
            MessageDialog dialog = new MessageDialog(body, head);
            dialog.Commands.Add(new UICommand("Kapat"));
            await dialog.ShowAsync();
        }

        private void grid1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(View.PriceScreen));
        }

        private void grid3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(View.StationScreen));
        }

        private void grid5_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(View.CardScreen));
        }

        private void grid7_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(View.AdressScreen));
           //Message("Sık kullandığınız konumları kaydedebileceğiniz adreslerinizi kaydetme özelliği", "Pek yakında");
        }

    }
}
