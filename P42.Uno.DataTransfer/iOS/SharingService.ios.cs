using System;
using UIKit;
using Foundation;
using MobileCoreServices;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using ObjCRuntime;
using System.Threading.Tasks;
using CoreGraphics;
using Windows.UI.Xaml;

namespace P42.Uno.DataTransfer.iOS
{
    public class SharingService : P42.Uno.DataTransfer.INativeSharingService
    {

        public void Share(P42.Uno.DataTransfer.MimeItemCollection mimeItemCollection, FrameworkElement target)
        {

            var nsItemProviders = mimeItemCollection.AsNSItemProviders();
            var activityController = new UIActivityViewController(nsItemProviders.ToArray(), null);
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
                vc = vc.PresentedViewController;

            if (Xamarin.Essentials.DeviceInfo.Idiom == Xamarin.Essentials.DeviceIdiom.Tablet)
            {
                var targetUIView = target as UIView;
                activityController.PopoverPresentationController.SourceView = targetUIView;
                activityController.PopoverPresentationController.SourceRect = new CGRect(new CGPoint(0, 0), targetUIView.Frame.Size);
                activityController.PopoverPresentationController.PageOverlayView().BackgroundColor = UIColor.Black.ColorWithAlpha(0.4f);
            }
            vc.PresentViewController(activityController, true, null);
        }
    }

    public static class UIPopoverPresentationControllerExtensions
    {
        public static UIView PageOverlayView(this UIPopoverPresentationController controller)
        {
            return controller.ValueForKey((Foundation.NSString)"_dimmingView") as UIView;
        }
    }
}