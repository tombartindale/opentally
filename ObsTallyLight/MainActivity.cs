using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using OBS.WebSocket.NET;
using OBS.WebSocket.NET.Types;
using Xamarin.Essentials;

namespace ObsTallyLight
{
    [Activity(Label = "@string/app_name")]
    public class MainActivity : AppCompatActivity
    {
        private ObsWebSocket _obs;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            FindViewById(Resource.Id.rootview).SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.LayoutStable | SystemUiFlags.LayoutFullscreen | SystemUiFlags.LayoutHideNavigation);
            Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            FindViewById<ImageButton>(Resource.Id.settings).Click += OpenSettingsClick;
            FindViewById<Button>(Resource.Id.save).Click += SaveClick;

            _obs = new ObsWebSocket();
        }

        private void SaveClick(object sender, EventArgs e)
        {
            //save settings
            
            FindViewById(Resource.Id.settingspage).Visibility = ViewStates.Gone;


            Xamarin.Essentials.Preferences.Set("server", FindViewById<TextView>(Resource.Id.server).Text.ToString());
            Xamarin.Essentials.Preferences.Set("port", FindViewById<TextView>(Resource.Id.port).Text.ToString());




            var server = Xamarin.Essentials.Preferences.Get("server", "10.0.2.2");
            var port = Xamarin.Essentials.Preferences.Get("port", "4444");

            Intent intent = Intent;
            Finish();
            StartActivity(intent);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _obs.Disconnected -= _obs_Disconnected;
            _obs.Disconnect();
            _obs = null;
        }

        private void OpenSettingsClick(object sender, EventArgs e)
        {
            //open settings
            FindViewById(Resource.Id.settingspage).Visibility = ViewStates.Visible;
        }

        //private string connection = "ws://10.0.2.2:4444";

        bool firstrun = true;

        protected override void OnResume()
        {
            base.OnResume();
            _obs.Connected += _obs_Connected;
            _obs.Disconnected += _obs_Disconnected;
            _obs.OBSExit += _obs_OBSExit;
            _obs.StreamingStateChanged += _obs_StreamingStateChanged;
            _obs.RecordingStateChanged += _obs_RecordingStateChanged;
            _obs.SceneChanged += _obs_SceneChanged;
            //_obs.SceneItemVisibilityChanged += _obs_SceneItemVisibilityChanged;
            //_obs.TransitionBegin += _obs_TransitionBegin;

            //FindViewById(Resource.Id.live).Visibility = ViewStates.Gone;
            FindViewById(Resource.Id.rec).Visibility = ViewStates.Gone;
            FindViewById(Resource.Id.settingspage).Visibility = ViewStates.Gone;

            var server = Xamarin.Essentials.Preferences.Get("server", "10.0.2.2");
            FindViewById<TextView>(Resource.Id.server).Text = server;

            var port = Xamarin.Essentials.Preferences.Get("port", "4444");
            FindViewById<TextView>(Resource.Id.port).Text = port;

            CurrentSource = Xamarin.Essentials.Preferences.Get("source", 0);

            FindViewById<ListView>(Resource.Id.sources).ItemClick += MainActivity_ItemClick;


            


            (new Thread(() =>
            {
                try
                {
                    _obs.Connect($"ws://{server}:{port}", "");
                }
                catch { }
            })).Start();

        }

        private void _obs_TransitionBegin(object sender, EventArgs e)
        {
            
            
        }

        private void _obs_SceneItemVisibilityChanged(ObsWebSocket sender, string sceneName, string itemName)
        {
            
        }

        private void MainActivity_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("source", e.Position);

            CurrentSource = e.Position;
        }

        private void _obs_SceneChanged(ObsWebSocket sender, string newSceneName)
        {
            if (RECORDING)
            {
                var scene = _obs.Api.GetCurrentScene();
                if (scene.Items.Exists(s => s.SourceName == sources[CurrentSource].Name))
                {
                    FindViewById(Resource.Id.rec).Visibility = ViewStates.Visible;
                }
                else
                {
                    FindViewById(Resource.Id.rec).Visibility = ViewStates.Gone;
                }                    
            }
        }

        private void MainActivity_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //source changed:
            //FindViewById<ListView>(Resource.Id.sources).SetSelection(CurrentSource);
            //FindViewById<ListView>(Resource.Id.sources).SelectedView.Selected = true;

        }

        private void _obs_Disconnected(object sender, EventArgs e)
        {
            if (firstrun)
            {
                firstrun = false;
                return;
            }
            //obs disconnected
            RunOnUiThread(() =>
            {
                FindViewById<ImageView>(Resource.Id.connected).SetImageResource(Resource.Drawable.power_plug_off);
                FindViewById(Resource.Id.connectingprogres).Visibility = ViewStates.Visible;
            });

            (new Thread(() => {
                //while (!_obs.IsConnected)
                //{
                    //Thread.Sleep(5000);
                    var server = Xamarin.Essentials.Preferences.Get("server", "10.0.2.2");
                    var port = Xamarin.Essentials.Preferences.Get("port", "4444");
                    _obs.Connect($"ws://{server}:{port}", "");
                //}
            })).Start();
        }

        private void _obs_OBSExit(object sender, EventArgs e)
        {
            //obs not running
        }

        List<SourceInfo> sources;
        int CurrentSource = 0;
        bool RECORDING = false;
        bool STREAMING = false;
        

        private void _obs_Connected(object sender, EventArgs e)
        {
            //obs connected
            RunOnUiThread(() =>
            {
                FindViewById<ImageView>(Resource.Id.connected).SetImageResource(Resource.Drawable.power_plug);
                FindViewById(Resource.Id.connectingprogres).Visibility = ViewStates.Gone;
            });

            var streaming = _obs.Api.GetStreamingStatus();
            RunOnUiThread(() =>
            {
                if (CurrentSource == 0)
                {
                    if (streaming.IsRecording)
                        FindViewById(Resource.Id.rec).Visibility = ViewStates.Visible;
                    if (streaming.IsStreaming)
                        FindViewById(Resource.Id.live).Visibility = ViewStates.Visible;
                }
            });

            //list sources:
            sources = _obs.Api.GetSourcesList();
            
            var labels = (from s in sources select s.Name).ToList();
            labels.Insert(0,"[PROGRAM OUTPUT]");
            RunOnUiThread(() =>
            {
                FindViewById<ListView>(Resource.Id.sources).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemSingleChoice, labels);
                //FindViewById<ListView>(Resource.Id.sources).Post(() =>
                //{


                    FindViewById<ListView>(Resource.Id.sources).SetSelection(CurrentSource);
                    //FindViewById<ListView>(Resource.Id.sources).SelectedView.Selected = true;
                //});
            });



        }

        private void _obs_RecordingStateChanged(ObsWebSocket sender, OBS.WebSocket.NET.Types.OutputState type)
        {
            if (type == OutputState.Started)
                RECORDING = true;
            else
                RECORDING = false;

            if (CurrentSource == 0)
            {
                RunOnUiThread(() =>
                {
                    //recording started / stopped
                    switch (type)
                    {
                        case OBS.WebSocket.NET.Types.OutputState.Started:
                        case OBS.WebSocket.NET.Types.OutputState.Starting:
                            FindViewById(Resource.Id.rec).Visibility = ViewStates.Visible;

                            break;
                        case OBS.WebSocket.NET.Types.OutputState.Stopped:
                            FindViewById(Resource.Id.rec).Visibility = ViewStates.Gone;
                            break;
                    }
                });
            }
        }

        private void _obs_StreamingStateChanged(ObsWebSocket sender, OBS.WebSocket.NET.Types.OutputState type)
        {
            if (type == OutputState.Started)
                STREAMING = true;
            else
                STREAMING = false;

            if (CurrentSource == 0)
            {
                RunOnUiThread(() =>
                {
                //streaming started / stopped
                switch (type)
                    {
                        case OBS.WebSocket.NET.Types.OutputState.Started:
                        case OBS.WebSocket.NET.Types.OutputState.Starting:
                            FindViewById(Resource.Id.live).Visibility = ViewStates.Visible;

                            break;
                        case OBS.WebSocket.NET.Types.OutputState.Stopped:
                            FindViewById(Resource.Id.live).Visibility = ViewStates.Gone;
                            break;
                    }
                });
            }
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        
    }
}

