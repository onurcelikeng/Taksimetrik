using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Services.Maps;
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

namespace Taksimetrik.View
{
    public sealed partial class PriceScreen : Page
    {
        private bool flag = false;
        MapIcon icon = new MapIcon();
        Geopoint myLocation;
        Geopoint target;


        public PriceScreen()
        {
            this.InitializeComponent();

            getMyLocation();
        }


        public async void getMyLocation()
        {
            try
            {
                map.MapElements.Clear();
                progress.IsIndeterminate = true;

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

                progress.IsIndeterminate = false;
            }

            catch (Exception)
            {
                progress.IsIndeterminate = false;
                Message("Konum ayarlarınız kapalı.", "Bir hata oluştu :(");
            }
        }

        private async Task requestDistance(Geopoint g1, Geopoint g2)
        {
            try
            {
                if (target == null)
                {
                    Message("Gideceğiniz noktayı haritadan seçmeyi unuttunuz.", "Bir hata oluştu :(");
                }

                else
                {
                    progress.IsIndeterminate = true;

                    string url = "https://maps.googleapis.com/maps/api/directions/json?origin=" + g1.Position.Latitude.ToString().Replace(",", ".") + "," + g1.Position.Longitude.ToString().Replace(",", ".") + "&destination=" + g2.Position.Latitude.ToString().Replace(",", ".") + "," + g2.Position.Longitude.ToString().Replace(",", ".") + "&mode=driving&language=tr-tr&key=+AIzaSyCeBdq7rr-R7w7vZCXscLWgEDb3oO9CUhw";

                    HttpClient client = new HttpClient();
                    var data = await client.GetStringAsync(url);
                    var obj = JsonConvert.DeserializeObject<Model.Price>(data);

                    string getDistance = obj.routes[0].legs[0].distance.text;
                    string getDuration = obj.routes[0].legs[0].duration.text;
                    string text = "Mesafe : " + getDistance + "\n" + "Süre : " + getDuration + "\n" + "Tahmini Fiyat : " + getPrice(getDistance).ToString() + " TL";

                    Message(text, "Bildirim");

                    progress.IsIndeterminate = false;
                }
            }

            catch (Exception)
            {
                progress.IsIndeterminate = false;

                if (NetworkInformation.GetInternetConnectionProfile() == null)
                {
                    Message("Herhangi bir ağ bağlantısı bulunamadı. Telefon ayarlarınızı kontrol edin ve yeniden deneyin.", "Bir hata oluştu :(");
                }

                else
                {
                    Message("Bağlantı zaman aşımına uğradı. Bağlantınızı kontrol edip tekrar deneyiniz.", "Bir hata oluştu :(");
                }
            }
        }

        private double getPrice(string distance)
        {
            string[] array = distance.Split(' ');
            double price = 3.0 + (2.45 * Convert.ToDouble(array[0]));
            return price;
        }

        private void PinToMap(double position_X, double position_Y)
        {
            BasicGeoposition pos = new BasicGeoposition();
            pos.Latitude = position_X;
            pos.Longitude = position_Y;

            icon.Location = new Geopoint(pos);
            icon.NormalizedAnchorPoint = new Point() { X = 0.32, Y = 0.78 };
            icon.Visible = true;
            icon.ZIndex = int.MaxValue;
            icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/targetPin.png", UriKind.RelativeOrAbsolute));

            map.MapElements.Add(icon);
        }

        public async void Message(string body, string head)
        {
            MessageDialog dialog = new MessageDialog(body, head);
            dialog.Commands.Add(new UICommand("Kapat"));
            await dialog.ShowAsync();
        }

        private async void map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            try
            {
                if (flag == false)
                {
                    target = new Geopoint(args.Location.Position);
                    MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(target);

                    var resultText = new StringBuilder();

                    //if (result.Status == MapLocationFinderStatus.Success)
                    {
                        resultText.AppendLine(result.Locations[0].Address.Town + ", " + result.Locations[0].Address.Country);
                    }

                    PinToMap(target.Position.Latitude, target.Position.Longitude);
                    flag = true;
                }
            }
            catch (Exception)
            {

            }

        }

        private void targetButton_Click(object sender, RoutedEventArgs e)
        {
            getMyLocation();
        }

        private async void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            await requestDistance(myLocation, target);
        }

        private void singleClearScreen_Click(object sender, RoutedEventArgs e)
        {
            map.MapElements.Remove(icon);
            target = null;
            flag = false;
        }

        private void allClearScreen_Click(object sender, RoutedEventArgs e)
        {
            map.MapElements.Clear();
            target = null;
            flag = false;
        }

    }
}
