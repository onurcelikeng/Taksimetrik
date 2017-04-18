using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Taksimetrik.ModelView
{
    public sealed partial class AddPerson : Page
    {

        public AddPerson()
        {
            this.InitializeComponent();
        }


        public async void Message(string body, string head)
        {
            MessageDialog dialog = new MessageDialog(body, head);
            dialog.Commands.Add(new UICommand("Kapat"));
            await dialog.ShowAsync();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = ApplicationData.Current.RoamingSettings;

            string getName = name.Text;
            string getPhone = phone.Text;

            if(getName != "" | getPhone != "")
            {
                Object nameList = settings.Values["NameList"];
                Object phoneList = settings.Values["PhoneList"];

                nameList = nameList + "@" + getName;
                phoneList = phoneList + "@" + getPhone;

                settings.Values["NameList"] = nameList;
                settings.Values["PhoneList"] = phoneList;

                Message(getName + " isimli taksici başarılı bir şekilde kaydedildi.", "İşlem tamam");
            }

            else
            {
                Message("Lütfen bilgileri eksiksiz giriniz", "Bir hata oluştu :(");
            }
            
        }

    }
}
