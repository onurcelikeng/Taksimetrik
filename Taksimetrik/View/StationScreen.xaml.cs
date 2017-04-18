using System;
using Windows.UI.Popups;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Windows.UI.Xaml;
using Taksimetrik.Model;
using Windows.Networking.Connectivity;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel.Calls;

namespace Taksimetrik.View
{
    public sealed partial class StationScreen : Page
    {
        List<Place> stationList = new List<Place>();
        Geopoint myLocation;


        public StationScreen()
        {
            this.InitializeComponent();

            getMyLocation();
        }


        private async void getMyLocation()
        {
            try
            {
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

        private async Task getStationPlaces(Geopoint g)
        {
            try
            {
                progress.IsIndeterminate = true;

                string url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + g.Position.Latitude.ToString().Replace(",", ".") + "," + g.Position.Longitude.ToString().Replace(",", ".") + "&radius=3000&types=taxi_stand&key=AIzaSyDSuWOakAmCKEzeDfPq3fRfuISKu0nhjmU";

                HttpClient client = new HttpClient();
                var data = await client.GetStringAsync(url);
                var obj = JsonConvert.DeserializeObject<Model.Station>(data);

                for (int i = 0; i < obj.results.Count; i++)
                {
                    if (obj.results[i] == null) break;
                    stationList.Add(obj.results[i]);
            
                    PinToMap(obj.results[i].geometry, obj.results[i].name);
                }

                var jayway = new Geopoint(new BasicGeoposition() { Latitude = obj.results[0].geometry.location.lat, Longitude = obj.results[0].geometry.location.lng });
                await map.TrySetViewAsync(jayway, 15, 0, 0, MapAnimationKind.Bow);

                progress.IsIndeterminate = false;
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

        private async void findPlacebyId(String placeId)
        {
            try
            {
                progress.IsIndeterminate = true;

                string url = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + placeId + "&key=AIzaSyDSuWOakAmCKEzeDfPq3fRfuISKu0nhjmU";

                HttpClient client = new HttpClient();
                var data = await client.GetStringAsync(url);
                var obj = JsonConvert.DeserializeObject<Model.PlaceDetails>(data);

                string phone = obj.result.international_phone_number;
                string adress = obj.result.formatted_address;
                InfoScreen(adress, phone, obj.result.name);

                progress.IsIndeterminate = false;
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

        private void PinToMap(Geometry g, String name)
        {
            BasicGeoposition pos = new BasicGeoposition();
            pos.Latitude = g.location.lat;
            pos.Longitude = g.location.lng;
            MapIcon icon = new MapIcon();

            icon.Location = new Geopoint(pos);
            icon.NormalizedAnchorPoint = new Point() { X = 0.32, Y = 0.78 };
            icon.Visible = true;
            icon.Title = name;
            icon.ZIndex = int.MaxValue;
            icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/stations.png", UriKind.RelativeOrAbsolute));

            map.MapElements.Add(icon);
        }

        public async void Message(string body, string head)
        {
            MessageDialog dialog = new MessageDialog(body, head);
            dialog.Commands.Add(new UICommand("Kapat"));
            await dialog.ShowAsync();
        }

        private async void InfoScreen(string adress, string phone, string header)
        {
            try
            {
                MessageDialog dialog = new MessageDialog(adress + "\n" + phone, header);
                var button_OK = new UICommand("Ara");
                var button_CANCEL = new UICommand("Vazgeç");

                dialog.Commands.Add(button_OK);
                dialog.Commands.Add(button_CANCEL);
                IUICommand result = await dialog.ShowAsync();

                if (result != null && result.Label == "Ara")
                {
                    if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.Calls.CallsPhoneContract", 1, 0))
                    {
                        PhoneCallManager.ShowPhoneCallUI(phone.Trim(), header);
                    }
                }
            }

            catch (Exception)
            {

            }
        }

        #region Button Events

        private void targetButton_Click(object sender, RoutedEventArgs e)
        {
            map.MapElements.Clear();
            getMyLocation();
        }

        private async void stationButton_Click(object sender, RoutedEventArgs e)
        {
            await getStationPlaces(myLocation);
        }

        private void clearScreen_Click(object sender, RoutedEventArgs e)
        {
            map.MapElements.Clear();
        }

        private void map_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            try
            {
                MapIcon ıcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
                string iconName = ıcon.Title;

                Place place = new Place();

                for (int i = 0; i < stationList.Count; i++)
                {
                    if (iconName == stationList[i].name)
                    {
                        place = stationList[i];
                        break;
                    }
                }

                findPlacebyId(place.place_id);
            }

            catch (Exception)
            {

            }
        }

        #endregion

    }
}
