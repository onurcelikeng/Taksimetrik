using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Taksimetrik.ModelView
{
    public sealed partial class AddAddress : Page
    {
        Geopoint myLocation;


        public AddAddress()
        {
            this.InitializeComponent();

            getMyLocation();
        }


        public async void getMyLocation()
        {
            try
            {
                map.MapElements.Clear();

                var gl = new Geolocator() { DesiredAccuracy = PositionAccuracy.High };
                var location = await gl.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(5));
                myLocation = location.Coordinate.Point;

                var pin = new MapIcon()
                {
                    Location = location.Coordinate.Point,
                    Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/pin.png")),
                    NormalizedAnchorPoint = new Point() { X = 0.32, Y = 0.78 },
                };

                map.MapElements.Add(pin);
                await map.TrySetViewAsync(location.Coordinate.Point, 16, 0, 0, MapAnimationKind.Bow);
            }

            catch (Exception)
            {
                Message("Konum ayarlarınız kapalı.", "Bir hata oluştu :(");
            }
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

            if(myLocation != null)
            {
                string getDefination = adressTextBox.Text;
                string getLatitude = myLocation.Position.Latitude.ToString();
                string getLongitude = myLocation.Position.Longitude.ToString();

                if (getDefination != "")
                {
                    Object definationList = settings.Values["DefinationList"];
                    Object latitudeList = settings.Values["LatitudeList"];
                    Object longitudeList = settings.Values["LongitudeList"];

                    definationList = definationList + "@" + getDefination;
                    latitudeList = latitudeList + "@" + getLatitude;
                    longitudeList = longitudeList + "@" + getLongitude;

                    settings.Values["DefinationList"] = definationList;
                    settings.Values["LatitudeList"] = latitudeList;
                    settings.Values["LongitudeList"] = longitudeList;

                    Message(getDefination + " tanımlı adres başarılı bir şekilde kaydedildi.", "İşlem tamam");
                }

                else
                {
                    Message("Lütfen bilgileri eksiksiz giriniz", "Bir hata oluştu :(");
                }
            }

            else
            {
                Message("Konum bilginiz alınamadı.", "Bir hata oluştu :(");
            }
        }

        private void map_MapTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {

        }

    }
}
