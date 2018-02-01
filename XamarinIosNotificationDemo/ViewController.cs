using System;
using Foundation;
using UIKit;
using UserNotifications;

namespace XamarinIosNotificationDemo
{
    public partial class ViewController : UIViewController
    {
        private const int Waitsecs = 30;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            button.TouchUpInside += (sender, e) =>
            {
                if (!CheckIsPermissionSet())
                {
                    ShowAskPermission();
                }

                SetupNotification();
            };
        }

        private void SetupNotification()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {

            }
            else
            {
                Console.WriteLine("iOS 9-, use old Notification API...");
                // create the notification
                var notification = new UILocalNotification
                {
                    // set the fire date (the date time in which it will fire)
                    FireDate = NSDate.FromTimeIntervalSinceNow(Waitsecs),

                    // configure the alert
                    AlertAction = "View Alert",
                    AlertBody = "Your 1 minute alert has fired!",

                    // modify the badge
                    ApplicationIconBadgeNumber = UIApplication.SharedApplication.ApplicationIconBadgeNumber + 1,

                    // set the sound to be the default sound
                    SoundName = UILocalNotification.DefaultSoundName
                };

                // schedule it
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                Console.WriteLine("Scheduled...");
            }
        }

        private bool CheckIsPermissionSet()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                throw new NotImplementedException();
            }
            else if(UIDevice.CurrentDevice.CheckSystemVersion(8,0))
            {
                Console.WriteLine("Since it is iOS 8 or later, we need to check if user agree to send notification...");

                var old = UIApplication.SharedApplication.CurrentUserNotificationSettings;
                if (old == null) { return false;}
                switch (old.Types)
                {
                    case UIUserNotificationType.None:
                        return false;
                    case UIUserNotificationType.Alert:
                    case UIUserNotificationType.Badge:
                    case UIUserNotificationType.Sound:
                        return true;
                    default:
                        break;
                }

                return false;
            }
            return true;
        }

        private void ShowAskPermission()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Ask the user for permission to get notifications on iOS 10.0+
                Console.WriteLine("Use new User Notification Framework API.");
                UNUserNotificationCenter.Current.RequestAuthorization(
                    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (approved, error) => { });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {                
                // Ask the user for permission to get notifications on iOS 8.0+
                Console.WriteLine("Use old register notification API.");
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}