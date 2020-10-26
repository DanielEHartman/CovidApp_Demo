using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Com.Wang.Avi;
using RestSharp;
using Android.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using Com.Toptoche.Searchablespinnerlibrary;
using System.Linq;
using Xamarin.Essentials;

/*Author: Daniel Hartman
 * 
 * This project was made as a small demo and skill display.
 * I hope you like it!
 * 
 * This app requests, processes, stores and displays covid-19 data with the use of a free API found at;
 * https://rapidapi.com/axisbits-axisbits-default/api/covid-19-statistics .
 * 
 * 
*/

namespace CV19_Demo {
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity {

        #region Declaration of views
        Button btnRefresh;
        AVLoadingIndicatorView avi;
        TextView txtTotalNumber, txtTotalRecoveredNumber, txtTotalDeathsNumber, txtNewCasesNumber, txtGlobTotalNumber, txtGlobRecoveredNumber, txtGlobDeathsNumber, txtGlobNewCasesNumber, txtBackground;
        SearchableSpinner searchableSpinner;
        #endregion

        NumberFormatInfo nfi;
        SQLiteConnection db;

        private static string dbtag = "cv19db";
        private static object collisionLock = new object();

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            nfi = new CultureInfo("en-US", false).NumberFormat;



            #region Initialize Views
            //Instantiate views
            btnRefresh = FindViewById<Button>(Resource.Id.btnRefresh);
            avi = FindViewById<AVLoadingIndicatorView>(Resource.Id.aviLoader);
            txtTotalNumber = FindViewById<TextView>(Resource.Id.txtTotalNumber);
            txtTotalRecoveredNumber = FindViewById<TextView>(Resource.Id.txtTotalRecoveredNumber);
            txtTotalDeathsNumber = FindViewById<TextView>(Resource.Id.txtTotalDeathsNumber);
            txtNewCasesNumber = FindViewById<TextView>(Resource.Id.txtNewCasesNumber);
            searchableSpinner = FindViewById<SearchableSpinner>(Resource.Id.spinnerRegion);
            txtGlobTotalNumber = FindViewById<TextView>(Resource.Id.txtGlobTotalNumber);
            txtGlobRecoveredNumber = FindViewById<TextView>(Resource.Id.txtGlobRecoveredNumber);
            txtGlobDeathsNumber = FindViewById<TextView>(Resource.Id.txtGlobDeathsNumber);
            txtGlobNewCasesNumber = FindViewById<TextView>(Resource.Id.txtGlobNewCasesNumber);
            txtBackground = FindViewById<TextView>(Resource.Id.txtBackground);
            #endregion

            //assign events to views
            btnRefresh.Click += BtnRefresh_Click;
            searchableSpinner.ItemSelected += SearchableSpinner_ItemSelected;

            Log.Debug(dbtag, "Creating database, if it doesn't already exist");
            string dbPath = Path.Combine(
                 System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                 "cv19db.db3");
            db = new SQLiteConnection(dbPath);
            db.CreateTable<Items>();
            DisplayData();
        }
        protected override void OnPause() {
            base.OnPause();
            Preferences.Set("selectedIndex", searchableSpinner.SelectedItemPosition);
        }

        protected override void OnResume() {
            base.OnResume();
            searchableSpinner.SetSelection(Preferences.Get("selectedIndex", 0));
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Preferences.Get("lastRefresh", 0L) > 86400) {
                btnRefresh.PerformClick();
            }
        }

        private void SearchableSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            DisplayRegionData(false);
        }

        private void BtnRefresh_Click(object sender, System.EventArgs e) {
            getData();
        }

        private async void getData() {
            try {
                //Show loader
                avi.SmoothToShow();
                await Task.Run(async () => {

                    //Clear databse table
                    db.DropTable<Items>();
                    db.CreateTable<Items>();
                    #region Sync Data
                    //Retrieve Data from API using RestClient
                    var client = new RestClient("https://covid-19-statistics.p.rapidapi.com/reports");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("x-rapidapi-host", "covid-19-statistics.p.rapidapi.com");
                    request.AddHeader("x-rapidapi-key", "928c537734msh2a587a7f1f970d1p1acf1fjsn6b7fa9d4cbc9");
                    IRestResponse response = await client.ExecuteAsync(request);
                    //Ensure successful response
                    if (response.IsSuccessful) {
                        //Update last sync time
                        Preferences.Set("lastRefresh", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                        string kl = response.Content;
                        //Deserialize Json Array to Class Object
                        Root root = JsonConvert.DeserializeObject<Root>(response.Content);
                        List<Items> items = new List<Items>();
                        Log.Debug("timer", "start insert");
                        //Add each Object to SQLite Table
                        foreach (Datum data in root.data) {
                            var newItem = new Items();
                            newItem.date = data.date;
                            newItem.confirmed = data.confirmed;
                            newItem.confirmed_diff = data.confirmed_diff;
                            newItem.deaths = data.deaths;
                            newItem.recovered = data.recovered;
                            newItem.last_update = data.last_update;
                            newItem.active = data.active;
                            newItem.active_diff = data.active_diff;
                            newItem.regionName = data.region.name;
                            newItem.regionISO = data.region.iso;

                            if (!String.IsNullOrEmpty(data.region.province)) {
                                newItem.regionName += ", " + data.region.province;
                            }
                            items.Add(newItem);
                        }
                        db.InsertAll(items);
                        #endregion
                        //Hide Loader
                        RunOnUiThread(() => {
                            avi.SmoothToHide();
                        });
                        //Update Display
                        DisplayData();

                    }
                    else {
                        RunOnUiThread(() => {
                            //Display error upon unsuccessful response
                            Toast.MakeText(Application.Context, "Server Unresponsive, please try again later.", ToastLength.Long).Show();
                            avi.SmoothToHide();
                        });
                    }

                });
            }
            catch (Exception err) {
                Log.Error("get", err.ToString());
                Toast.MakeText(Application.Context, "An error has occured, please try again.", ToastLength.Long).Show();
                avi.SmoothToHide();
            }
        }

        private void DisplayData() {
            RunOnUiThread(() => {
                int totalcases = 0,
                totaldeaths = 0,
                totalrecoveries = 0,
                newcases = 0;
                //Lock Database incase of collision
                lock (collisionLock) {
                    //retrieve table and calculate totals for display
                    var table = db.Table<Items>();
                    List<string> datalist = new List<string>();
                    foreach (var s in table) {
                        datalist.Add(s.regionName);
                        totalcases += s.confirmed;
                        totaldeaths += s.deaths;
                        totalrecoveries += s.recovered;
                        newcases += s.confirmed_diff;
                    }
                    //Add items to region spinner
                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.support_simple_spinner_dropdown_item,
                            datalist.ToArray());
                    searchableSpinner.Adapter = adapter;
                    //Set textviews to display totals
                    txtGlobTotalNumber.Text = totalcases.ToString("#,0", nfi);
                    txtGlobDeathsNumber.Text = totaldeaths.ToString("#,0", nfi);
                    txtGlobRecoveredNumber.Text = totalrecoveries.ToString("#,0", nfi);
                    txtGlobNewCasesNumber.Text = newcases.ToString("#,0", nfi);
                }

            });
        }
        private void DisplayRegionData(bool init) {
            //Display regional data based on selected Item
            IEnumerable<Items> regionItems = GetFilteredRegion(searchableSpinner.SelectedItem.ToString());
            Items dispItemRegion = regionItems.FirstOrDefault();
            txtTotalNumber.Text = dispItemRegion.confirmed.ToString("#,0", nfi);
            txtTotalDeathsNumber.Text = dispItemRegion.deaths.ToString("#,0", nfi);
            txtTotalRecoveredNumber.Text = dispItemRegion.recovered.ToString("#,0", nfi);
            txtNewCasesNumber.Text = dispItemRegion.confirmed_diff.ToString("#,0", nfi);
            txtBackground.Text = dispItemRegion.regionISO;

        }
        //Get the filtered region details via SQL query
        public IEnumerable<Items> GetFilteredRegion(string regionName) {
            lock (collisionLock) {
                var query = from Items in db.Table<Items>()
                            where Items.regionName == regionName
                            select Items;
                return query.AsEnumerable();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        //Classes
        public class Region {
            public string iso { get; set; }
            public string name { get; set; }
            public string province { get; set; }
            public string lat { get; set; }
            public string @long { get; set; }
            public List<object> cities { get; set; }
        }

        public class Datum {
            public string date { get; set; }
            public int confirmed { get; set; }
            public int deaths { get; set; }
            public int recovered { get; set; }
            public int confirmed_diff { get; set; }
            public int deaths_diff { get; set; }
            public int recovered_diff { get; set; }
            public string last_update { get; set; }
            public int active { get; set; }
            public int active_diff { get; set; }
            public double fatality_rate { get; set; }
            public Region region { get; set; }
        }

        public class Root {
            public List<Datum> data { get; set; }
        }

        //Table
        [Table("Items")]
        public class Items {
            [PrimaryKey, AutoIncrement, Column("_id")]
            public int Id { get; set; }
            [Column("Date"), MaxLength(255)]
            public string date { get; set; }
            [Column("confirmed")]
            public int confirmed { get; set; }
            [Column("deaths")]
            public int deaths { get; set; }
            [Column("recovered")]
            public int recovered { get; set; }
            [Column("confirmed_diff")]
            public int confirmed_diff { get; set; }
            [Column("last_update"), MaxLength(255)]
            public string last_update { get; set; }
            [Column("active")]
            public int active { get; set; }
            [Column("active_diff")]
            public int active_diff { get; set; }
            [Column("regionName"), MaxLength(255)]
            public string regionName { get; set; }
            [Column("regionISO"), MaxLength(255)]
            public string regionISO { get; set; }

        }
    }
}