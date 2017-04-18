using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Taksimetrik.Model;
using Windows.ApplicationModel.Calls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Taksimetrik.View
{
    public sealed partial class CardScreen : Page
    {
        List<Card> cardList = new List<Card>();


        public CardScreen()
        {
            this.InitializeComponent();

            ListCard();
        }


        private void ListCard()
        {
            var settings = ApplicationData.Current.RoamingSettings;

            Object nameList = settings.Values["NameList"];
            Object phoneList = settings.Values["PhoneList"];

            if (nameList != null && phoneList != null)
            {
                string[] nameArray = nameList.ToString().Split('@');
                string[] phoneArray = phoneList.ToString().Split('@');

                for (int i = 1; i < nameArray.Length; i++)
                {
                    var item = new Card();
                    item.Name = nameArray[i];
                    item.PhoneNumber = phoneArray[i];

                    list.Items.Add(item);
                }
            }
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = (Card)list.SelectedItem;

            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.Calls.CallsPhoneContract", 1, 0))
            {
                PhoneCallManager.ShowPhoneCallUI(value.PhoneNumber, value.Name);
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ModelView.AddPerson));
        }

    }
}
