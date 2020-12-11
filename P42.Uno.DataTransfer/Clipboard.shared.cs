using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace P42.Uno.DataTransfer
{
    /// <summary>
    /// P42.Uno.DataTransfer.Clipboard class
    /// </summary>
    public static class Clipboard
    {
        static INativeClipboardService _service;
        static INativeClipboardService Service
        {
            get
            {
#if __ANDROID__
                _service = _service ?? new Droid.ClipboardService();
#elif __IOS__
                _service = _service ?? new iOS.ClipboardService();
#elif NETFX_CORE
                _service = _service ?? new UWP.ClipboardService();
#endif
                return _service;
            }
        }

        /// <summary>
        /// Is the Clipboard service available on this platform?
        /// </summary>
        public static bool IsAvailable => Service != null;

        /// <summary>
        /// Gets/Sets the current Entry on the clipboard
        /// </summary>
        public static IMimeItemCollection Entry
        {
            get => Service?.Entry ?? new MimeItemCollection();

            set
            {
                if (Service != null)
                    Service.Entry = value;
            }
        }

        /// <summary>
        /// Turns of caching latest clipboard entry (for speed, of course).  Mostly for test purposes.
        /// </summary>
        /// <value><c>true</c> if the latest clipboard entry is cached; otherwise, <c>false</c>.</value>
        public static bool EntryCaching
        {
            get => Service?.EntryCaching ?? false;
            set
            {
                if (Service != null)
                    Service.EntryCaching = value;
            }
        }

        /// <summary>
        /// Event fired when the content on the clipboard has changed
        /// </summary>
        public static event EventHandler ContentChanged;

        internal static void OnContentChanged(object sender, EventArgs args)
        {
            ContentChanged?.Invoke(null, args);
        }



    }
}