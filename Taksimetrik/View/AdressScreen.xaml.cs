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
using Taksimetrik.Model;
using Windows.Storage;

namespace Taksimetrik.View
{
    public sealed partial class AdressScreen : Page
    {
        List<Adress> adrestList = new List<Adress>();


        public AdressScreen()
        {
            this.InitializeComponent();

            ListAdress();
        }


        private void ListAdress()
        {
            var settings = ApplicationData.Current.RoamingSettings;

            Object definationList = settings.Values["DefinationList"];
            Object latitudeList = settings.Values["LatitudeList"];
            Object longitudeList = settings.Values["LongitudeList"];

            if (definationList != null)
            {
                string[] definationArray = definationList.ToString().Split('@');
                string[] latitudeArray = latitudeList.ToString().Split('@');
                string[] longitudeArray = longitudeList.ToString().Split('@');

                for (int i = 1; i < definationArray.Length; i++)
                {
                    var item = new Adress();

                    item.Defination = definationArray[i];
                    item.Latitute = latitudeArray[i];
                    item.Longitude = longitudeArray[i];

                    list.Items.Add(item);
                }
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ModelView.AddAddress));
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
