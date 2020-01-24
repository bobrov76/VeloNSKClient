using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;
using Plugin.Media;
using ImageCircle.Forms.Plugin.Droid;
using FFImageLoading.Forms.Platform;

namespace VeloNSK.Droid
{
    [Activity(Label = "VeloNSK", Icon = "@mipmap/logo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private Bundle bundle;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            CrossCurrentActivity.Current.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            Xamarin.Forms.DataGrid.DataGridComponent.Init();
            base.OnCreate(savedInstanceState);
            CrossMedia.Current.Initialize();
            ImageCircleRenderer.Init();
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CachedImageRenderer.InitImageViewHandler();
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}