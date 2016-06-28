using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using NotificationsExtensions.Toasts;

namespace LubCycle.UWP.Helpers
{
    class NotificationHelper
    {
        public static void GetNotification()
        {
            // In a real app, these would be initialized with actual data
            string title = "Dodano stację startową";
            string content = "Check this out, Happy Canyon in Utah!";
            //string image = "http://blogs.msdn.com/cfs-filesystemfile.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-01-71-81-permanent/2727.happycanyon1_5B00_1_5D00_.jpg";
            //string logo = "ms-appdata:///local/Andrew.jpg";

            // Construct the visuals of the toast
            ToastVisual visual = new ToastVisual()
            {
                TitleText = new ToastText()
                {
                    Text = title
                },
                BodyTextLine1 = new ToastText()
                {
                    Text = content
                }
                //InlineImages =
                //{
                //    new ToastImage()
                //    {
                //        Source = new ToastImageSource(image)
                //    }
                //},
                //AppLogoOverride = new ToastAppLogo()
                //{
                //    Source = new ToastImageSource(logo),
                //    Crop = ToastImageCrop.Circle
                //}
            };
            // Now we can construct the final toast content
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                //Actions = actions,

                // Arguments when the user taps body of toast
    //            Launch = new QueryString()
    //{
    //    { "action", "viewConversation" },
    //    { "conversationId", conversationId.ToString() }

    //}.ToString()
            };

            // And create the toast notification
            var toast = new ToastNotification(toastContent.GetXml());

            toast.ExpirationTime = DateTime.Now.AddDays(2);

            toast.Tag = "1";
            toast.Group = "wallPosts";

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static ToastNotification GetTextOnlyNotification(string title, string content)
        {
            var visual = new ToastVisual()
            {
                TitleText = new ToastText()
                {
                    Text = title
                },
                BodyTextLine1 = new ToastText()
                {
                    Text = content
                }
            };
            var toastContent = new ToastContent()
            {
                Visual = visual
            };
            var toast = new ToastNotification(toastContent.GetXml());
            toast.ExpirationTime = DateTime.Now.AddDays(2);
            return toast;
        }

    }
}
