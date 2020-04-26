using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Database;
using Firebase.Database.Query;
using Google.Android.Material.Snackbar;

namespace ObsTallyLight
{
    [Activity(Label = "@string/app_name")]
    public class MainActivity : AppCompatActivity
    {

        FirebaseClient FirebaseClient = new FirebaseClient("https://obstally.firebaseio.com/");

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            FindViewById(Resource.Id.rootview).SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.LayoutStable | SystemUiFlags.LayoutFullscreen | SystemUiFlags.LayoutHideNavigation);
            Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            FindViewById<ImageButton>(Resource.Id.settings).Click += OpenSettingsClick;
            FindViewById<Button>(Resource.Id.save).Click += SaveClick;
        }

        private void SaveClick(object sender, EventArgs e)
        {
            //save settings
            
            FindViewById(Resource.Id.settingspage).Visibility = ViewStates.Gone;

            //Intent intent = Intent;
            //Finish();
            //StartActivity(intent);
        }

        private void OpenSettingsClick(object sender, EventArgs e)
        {
            //open settings
            FindViewById(Resource.Id.settingspage).Visibility = ViewStates.Visible;
        }

        //private string connection = "ws://10.0.2.2:4444";

        bool firstrun = true;

        class ObsInstance
        {
            //public string MachineId { get; set; }
            public string Name { get; set; }
            public bool Online { get; set; }
            public string Pwd { get; set; }
            public bool Recording { get; set; }
            public bool Streaming { get; set; }
            public List<string> Tally { get; set; }
            public List<string> PreviewTally { get; set; }
            public List<string> Sources { get; set; }
        }

        protected async override void OnResume()
        {
            base.OnResume();
            //_obs.SceneItemVisibilityChanged += _obs_SceneItemVisibilityChanged;
            //_obs.TransitionBegin += _obs_TransitionBegin;

            //FindViewById(Resource.Id.live).Visibility = ViewStates.Gone;
            FindViewById(Resource.Id.rec).Visibility = ViewStates.Gone;
            FindViewById(Resource.Id.settingspage).Visibility = ViewStates.Gone;

            CurrentInstance = Xamarin.Essentials.Preferences.Get("instance", null);
            CurrentSource = Xamarin.Essentials.Preferences.Get("source", 0);

            FindViewById<ListView>(Resource.Id.sources).ItemClick += MainActivity_ItemClick;



            var instances = await FirebaseClient.Child("instances").OnceAsync<ObsInstance>();

            var instance = instances.First();


            var labels = instance.Object.Sources;
            labels.Insert(0, "[PROGRAM OUTPUT]");
            RunOnUiThread(() =>
            {
                FindViewById<ListView>(Resource.Id.sources).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemSingleChoice, labels);
                    FindViewById<ListView>(Resource.Id.sources).SetSelection(CurrentSource);
                });


            var observeTally = FirebaseClient.Child("instances").Child(instance.Key).Child("Tally").AsObservable<List<string>>().Subscribe(tally =>
            {
                UpdateTally(tally.Object);
            });

            var observeStreaming = FirebaseClient.Child("instances").Child(instance.Key).Child("Streaming").AsObservable<bool>().Subscribe(streaming =>
            {
                UpdateStreaming(streaming.Object);
            });
        }

        void UpdateRecording(bool yes)
        {
            RECORDING = yes;
        }

        void UpdateStreaming(bool yes)
        {
            STREAMING = yes;
        }

        void UpdateTally(List<string> tally)
        {
            if (RECORDING)
            {
                //var scene = _obs.Api.GetCurrentScene();
                if (tally.Contains(sources[CurrentSource]))
                {
                    FindViewById(Resource.Id.rec).Visibility = ViewStates.Visible;
                }
                else
                {
                    FindViewById(Resource.Id.rec).Visibility = ViewStates.Gone;
                }
            }
        }

        private void MainActivity_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("source", e.Position);

            CurrentSource = e.Position;
        }

        //private void _obs_SceneChanged(ObsWebSocket sender, string newSceneName)
        //{
        //    if (RECORDING)
        //    {
        //        var scene = _obs.Api.GetCurrentScene();
        //        if (scene.Items.Exists(s => s.SourceName == sources[CurrentSource].Name))
        //        {
        //            FindViewById(Resource.Id.rec).Visibility = ViewStates.Visible;
        //        }
        //        else
        //        {
        //            FindViewById(Resource.Id.rec).Visibility = ViewStates.Gone;
        //        }                    
        //    }
        //}

        private void MainActivity_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //source changed:
            //FindViewById<ListView>(Resource.Id.sources).SetSelection(CurrentSource);
            //FindViewById<ListView>(Resource.Id.sources).SelectedView.Selected = true;

        }

        //private void _obs_Disconnected(object sender, EventArgs e)
        //{
        //    if (firstrun)
        //    {
        //        firstrun = false;
        //        return;
        //    }
        //    //obs disconnected
        //    RunOnUiThread(() =>
        //    {
        //        FindViewById<ImageView>(Resource.Id.connected).SetImageResource(Resource.Drawable.power_plug_off);
        //        FindViewById(Resource.Id.connectingprogres).Visibility = ViewStates.Visible;
        //    });
        //}


        List<string> sources;
        int CurrentSource = 0;
        private string CurrentInstance;
        bool RECORDING = false;
        bool STREAMING = false;
        

        //private void _obs_Connected(object sender, EventArgs e)
        //{
        //    //obs connected
        //    RunOnUiThread(() =>
        //    {
        //        FindViewById<ImageView>(Resource.Id.connected).SetImageResource(Resource.Drawable.power_plug);
        //        FindViewById(Resource.Id.connectingprogres).Visibility = ViewStates.Gone;
        //    });

        //    var streaming = _obs.Api.GetStreamingStatus();
        //    RunOnUiThread(() =>
        //    {
        //        if (CurrentSource == 0)
        //        {
        //            if (streaming.IsRecording)
        //                FindViewById(Resource.Id.rec).Visibility = ViewStates.Visible;
        //            if (streaming.IsStreaming)
        //                FindViewById(Resource.Id.live).Visibility = ViewStates.Visible;
        //        }
        //    });

        //    //list sources:
        //    sources = _obs.Api.GetSourcesList();
            
        //    var labels = (from s in sources select s.Name).ToList();
        //    labels.Insert(0,"[PROGRAM OUTPUT]");
        //    RunOnUiThread(() =>
        //    {
        //        FindViewById<ListView>(Resource.Id.sources).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemSingleChoice, labels);
        //        //FindViewById<ListView>(Resource.Id.sources).Post(() =>
        //        //{


        //            FindViewById<ListView>(Resource.Id.sources).SetSelection(CurrentSource);
        //            //FindViewById<ListView>(Resource.Id.sources).SelectedView.Selected = true;
        //        //});
        //    });



        //}

        //private void _obs_RecordingStateChanged(ObsWebSocket sender, OBS.WebSocket.NET.Types.OutputState type)
        //{
        //    if (type == OutputState.Started)
        //        RECORDING = true;
        //    else
        //        RECORDING = false;

        //    if (CurrentSource == 0)
        //    {
        //        RunOnUiThread(() =>
        //        {
        //            //recording started / stopped
        //            switch (type)
        //            {
        //                case OBS.WebSocket.NET.Types.OutputState.Started:
        //                case OBS.WebSocket.NET.Types.OutputState.Starting:
        //                    FindViewById(Resource.Id.rec).Visibility = ViewStates.Visible;

        //                    break;
        //                case OBS.WebSocket.NET.Types.OutputState.Stopped:
        //                    FindViewById(Resource.Id.rec).Visibility = ViewStates.Gone;
        //                    break;
        //            }
        //        });
        //    }
        //}

        //private void _obs_StreamingStateChanged(ObsWebSocket sender, OBS.WebSocket.NET.Types.OutputState type)
        //{
        //    if (type == OutputState.Started)
        //        STREAMING = true;
        //    else
        //        STREAMING = false;

        //    if (CurrentSource == 0)
        //    {
        //        RunOnUiThread(() =>
        //        {
        //        //streaming started / stopped
        //        switch (type)
        //            {
        //                case OBS.WebSocket.NET.Types.OutputState.Started:
        //                case OBS.WebSocket.NET.Types.OutputState.Starting:
        //                    FindViewById(Resource.Id.live).Visibility = ViewStates.Visible;

        //                    break;
        //                case OBS.WebSocket.NET.Types.OutputState.Stopped:
        //                    FindViewById(Resource.Id.live).Visibility = ViewStates.Gone;
        //                    break;
        //            }
        //        });
        //    }
        //}

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        
    }
}

