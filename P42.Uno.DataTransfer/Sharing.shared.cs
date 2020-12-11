using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;

namespace P42.Uno.DataTransfer
{
    /// <summary>
    /// P42.Uno.DataTransfer.Clipboard class
    /// </summary>
    public static class Sharing
    {
        static INativeSharingService _service;
        static INativeSharingService Service
        {
            get
            {
#if __ANDROID__
                _service = _service ?? new Droid.SharingService();
#elif __IOS__
                _service = _service ?? new iOS.SharingService();
#elif NETFX_CORE
                _service = _service ?? new UWP.SharingService();
#endif
                return _service;
            }
        }

        /// <summary>
        /// Share the specified MimeItemCollection.  iPad: sharing popup points at target.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <param name="target">Target.</param>
        public static void Share(MimeItemCollection collection, FrameworkElement target) => Service?.Share(collection, target);

    }
}