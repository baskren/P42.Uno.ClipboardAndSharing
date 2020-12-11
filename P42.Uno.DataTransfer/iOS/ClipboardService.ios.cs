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

namespace P42.Uno.DataTransfer.iOS
{
    public class ClipboardService : P42.Uno.DataTransfer.INativeClipboardService
    {
        static internal NSString TypeListUti = new NSString(UTType.CreatePreferredIdentifier(UTType.TagClassMIMEType, "application/f9p-clipboard-typelist", null));

        static ClipboardService()
        {
            UIPasteboard.Notifications.ObserveChanged(OnPasteboardChanged);
            UIPasteboard.Notifications.ObserveRemoved(OnPasteboardChanged);
        }

        static void OnPasteboardChanged(object sender, UIPasteboardChangeEventArgs e)
        {
            Clipboard.OnContentChanged(null, EventArgs.Empty);
        }

        #region Entry property
        nint _lastEntryChangeCount = int.MinValue;
        IMimeItemCollection _lastEntry;
        public IMimeItemCollection Entry
        {
            get
            {
                if (_lastEntryChangeCount != UIPasteboard.General.ChangeCount)
                {
                    _lastEntry = new MimeItemCollection();
                }
                _lastEntryChangeCount = UIPasteboard.General.ChangeCount;
                return _lastEntry;
            }
            set
            {
                try
                {
                    if (value is DataTransfer.MimeItemCollection entry)
                    {
                        if (Xamarin.Essentials.DeviceInfo.Version >= new Version(11, 0))
                        {
                            var itemProviders = value.AsNSItemProviders();
                            if (EntryCaching)
                            {
                                _lastEntry = value;
                                _lastEntryChangeCount = UIPasteboard.General.ChangeCount + 1;
                            }
                            UIPasteboard.General.ItemProviders = itemProviders.ToArray();
                        }
                        else
                        {
                            var items = new List<NSMutableDictionary>();
                            NSMutableDictionary<NSString, NSObject> itemRenditions = null;
                            foreach (var mimeItem in entry.Items)
                            {
                                if (mimeItem.ToUiPasteboardItem() is KeyValuePair<NSString, NSObject> itemKvp && itemKvp.Key != null)
                                {

                                    // if no renditions, add one.
                                    // if the current rendition already contains this mimeType, create a new rendition
                                    if (itemRenditions == null || itemRenditions.ContainsKey(itemKvp.Key))
                                    {
                                        itemRenditions = new NSMutableDictionary<NSString, NSObject>();
                                        items.Add(itemRenditions);
                                    }


                                    // some notes here:
                                    // when trying to copy a string to iOS Notes app:
                                    // - itemRenditions.Add(itemKvp.Key, itemKvp.Value) means the ReturnMimeItem.Value is a byte array BUT it **does** paste correctly into Notes and Mail app
                                    // - itemRenditions.Add(itemKvp.Key, plist) pastes the bplist contents into Notes;
                                    // - itemRenditions.Add(itemKvp.Key, archiver) pastes the archiver + bplist contents into Notes;

                                    /*
                                    if (mimeItem.MimeType.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase))
                                        itemRenditions.Add(itemKvp.Key, itemKvp.Value);
                                    else
                                    {
                                        var archiver = NSKeyedArchiver.ArchivedDataWithRootObject(itemKvp.Value);
                                        itemRenditions.Add(itemKvp.Key, archiver);
                                    }
                                    */

                                    itemRenditions.Add(itemKvp.Key, itemKvp.Value);

                                    /*
                                    if (mimeItem.MimeType.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase))
                                        itemRenditions.Add(itemKvp.Key, itemKvp.Value);
                                    else
                                    {
                                        var plist = NSPropertyListSerialization.DataWithPropertyList(itemKvp.Value, NSPropertyListFormat.Binary, NSPropertyListWriteOptions.Immutable, out NSError nSError);
                                        System.Diagnostics.Debug.WriteLine("\t\t ClipboardService set_Entry 1.4 elapsed: " + stopwatch.ElapsedMilliseconds);
                                        itemRenditions.Add(itemKvp.Key, plist);
                                        System.Diagnostics.Debug.WriteLine("\t\t ClipboardService set_Entry 1.5 elapsed: " + stopwatch.ElapsedMilliseconds);
                                    }
                                    */
                                }
                            }
                            var array = items.ToArray();
                            if (EntryCaching)
                            {
                                _lastEntry = value;
                                _lastEntryChangeCount = UIPasteboard.General.ChangeCount + 1;
                            }
                            UIPasteboard.General.Items = array;
                        }

                    }
                }
                catch (Exception e)
                {
                    var itemTypes = "ItemTypes: [" + string.Join(", ", value.Items) + "]";
                    P42.Utils.DebugExtensions.RequestUserHelp(e, itemTypes);
                }
            }
        }

        public bool EntryCaching { get; set; } = false;

        #endregion

    }

}