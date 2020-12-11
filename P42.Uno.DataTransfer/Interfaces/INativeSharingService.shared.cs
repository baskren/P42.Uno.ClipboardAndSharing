using System;
using Windows.UI.Xaml;

namespace P42.Uno.DataTransfer
{
    interface INativeSharingService
    {
        void Share(MimeItemCollection mimeItemCollection, FrameworkElement target);
    }
}