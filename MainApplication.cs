using Android.App;
using Xamarin.Facebook;
using Xamarin.Facebook.AppEvents;

namespace testFbLogin
{
    public class MainApplication : Application
    {
        public override void OnCreate()
        {
            base.OnCreate();
            FacebookSdk.SdkInitialize(ApplicationContext);
            AppEventsLogger.ActivateApp(this);
        }
    }
}
