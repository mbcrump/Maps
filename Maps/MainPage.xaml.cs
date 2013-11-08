using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using Maps.Resources;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Services;
using System.Text;

namespace Maps
{
    public partial class MainPage : PhoneApplicationPage
    {
        public RouteQuery rq = new RouteQuery();
        public StringBuilder sb = new StringBuilder();

        public MainPage()
        {
            InitializeComponent();
            this.map.Center = new GeoCoordinate(55.5833, 13.0333);
        }        

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            this.map.ZoomLevel = Math.Min(20, this.map.ZoomLevel + 1);
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            this.map.ZoomLevel = Math.Max(1, this.map.ZoomLevel - 1);
        }

        void Road_Click(object sender, EventArgs args)
        {
            map.CartographicMode = MapCartographicMode.Road;
        }

        void Aerial_Click(object sender, EventArgs args)
        {
            map.CartographicMode = MapCartographicMode.Aerial;
        }

        void Hybrid_Click(object sender, EventArgs args)
        {
            map.CartographicMode = MapCartographicMode.Hybrid;
        }

        void Terrain_Click(object sender, EventArgs args)
        {
            map.CartographicMode = MapCartographicMode.Terrain;
        }

        void Light_Click(object sender, EventArgs args)
        {
            map.ColorMode = MapColorMode.Light;
        }

        void Dark_Click(object sender, EventArgs args)
        {
            map.CartographicMode = MapCartographicMode.Road;
            map.ColorMode = MapColorMode.Dark;
        }

        void Landmark_Click(object sender, EventArgs args)
        {
            this.map.Center = new GeoCoordinate(37.792878, -122.39641);
            this.map.ZoomLevel = 16;
            this.map.LandmarksEnabled = true;
        }

        void Pedestrian_Click(object sender, EventArgs args)
        {
            this.map.Center = new GeoCoordinate(47.6097, -122.3331);
            this.map.ZoomLevel = 16;
            this.map.PedestrianFeaturesEnabled = true;
        }

        void MapLayers_Click(object sender, EventArgs args)
        {
            map.Layers.Add(new MapLayer()
            {
                new MapOverlay()
                {
                    GeoCoordinate=new GeoCoordinate(33.520661, -86.80249),
                    Content=new Ellipse
                    {
                        Fill=new SolidColorBrush(Colors.Red),
                        Width=20,
                        Height=20
                    }
                }
            });
            this.map.Center = new GeoCoordinate(33.520661, -86.80249);
        }

        void RequestDirections_Click(object sender, EventArgs args)
        {
            RequestDirections();
        }

        async void RequestDirections()
        {
            rq.QueryCompleted += routeQuery_QueryCompleted;

            if (!rq.IsBusy)
            {
                List<GeoCoordinate> routeCoordinates = new List<GeoCoordinate>();
                routeCoordinates.Add(new GeoCoordinate(48.860339, 2.337599));
                routeCoordinates.Add(new GeoCoordinate(48.8583, 2.2945));

                rq.Waypoints = routeCoordinates;
                rq.QueryAsync();

                map.Center = new GeoCoordinate(48.8583, 2.2945); //Center map on last coordinates
            }
        }

        private void routeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            Route theRoute = e.Result;
            MapRoute calculatedMapRoute = new MapRoute(theRoute);
            map.ZoomLevel = 17;
            map.AddRoute(calculatedMapRoute);

            //Used Only For Direction List Button

            sb.AppendLine("Distance to destination: " + e.Result.LengthInMeters);
            sb.AppendLine("Time to destination: " + e.Result.EstimatedDuration);
            foreach (var maneuver in e.Result.Legs.SelectMany(l => l.Maneuvers))
            {
                sb.AppendLine("At " + maneuver.StartGeoCoordinate + " " +
                                        maneuver.InstructionKind + ": " +
                                        maneuver.InstructionText + " for " +
                                        maneuver.LengthInMeters + " meters");
            }
        }

        private void DirectionList(object sender, EventArgs e)
        {
            MessageBox.Show(sb.ToString());
        }

        public void GeoCoding_Click(object sender, EventArgs e)
        {
            GeocodeQuery query = new GeocodeQuery()
            {
                GeoCoordinate = new GeoCoordinate(0, 0),
                SearchTerm = "Vulcan, Alabama"
            };
            query.QueryCompleted += query_QueryCompleted;
            query.QueryAsync();
        }
  
        private void query_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Vulcan Geocoding results...");
            foreach (var item in e.Result)
            {
                sb.AppendLine(item.GeoCoordinate.ToString());
                sb.AppendLine(item.Information.Name);
                sb.AppendLine(item.Information.Description);
                sb.AppendLine(item.Information.Address.BuildingFloor);
                sb.AppendLine(item.Information.Address.BuildingName);
                sb.AppendLine(item.Information.Address.BuildingRoom);
                sb.AppendLine(item.Information.Address.BuildingZone);
                sb.AppendLine(item.Information.Address.City);
                sb.AppendLine(item.Information.Address.Continent);
                sb.AppendLine(item.Information.Address.Country);
                sb.AppendLine(item.Information.Address.CountryCode);
                sb.AppendLine(item.Information.Address.County);
                sb.AppendLine(item.Information.Address.District);
                sb.AppendLine(item.Information.Address.HouseNumber);
                sb.AppendLine(item.Information.Address.Neighborhood);
                sb.AppendLine(item.Information.Address.PostalCode);
                sb.AppendLine(item.Information.Address.Province);
                sb.AppendLine(item.Information.Address.State);
                sb.AppendLine(item.Information.Address.StateCode);
                sb.AppendLine(item.Information.Address.Street);
                sb.AppendLine(item.Information.Address.Township);
            }
            MessageBox.Show(sb.ToString());
        }
    }
}