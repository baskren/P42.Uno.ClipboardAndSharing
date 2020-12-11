using System;
using System.Collections.Generic;
using Foundation;
using MobileCoreServices;
using UIKit;
using System.Timers;
using System.Diagnostics;

namespace P42.Uno.DataTransfer.iOS
{

    class LazyMimeItem : INativeMimeItem
    {
        string _mimeType;
        public string MimeType
        {
            get => _mimeType;
            internal set
            {
                _mimeType = value?.ToLower();
            }
        }

        object _value;
        public object Value
        {
            get
            {
                if (_value != null)
                    return _value;
                return _value = GetValueAs(null);

            }
        }

        public object GetValueAs(Type type)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            NSObject nsObject = null;

            if (MimeType.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase) && _kvp.Value is UIImage uiImage)
            {
                switch (MimeType)
                {
                    case "image/jpeg":
                    case "image/jpg":
                        nsObject = uiImage.AsJPEG();
                        break;
                    case "image/png":
                        nsObject = uiImage.AsPNG();
                        break;
                    default:
                        break;
                }
            }

            //System.Diagnostics.Debug.WriteLine("\t\t\t GetValueAs 1 stopwatch.Elapsed: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();

            // IMPORTANT:  
            // Must try NSKeyedUnarchiver.UnarchiveObject() **before** NSPropertyListSerialization.PropertyListWithData()
            // The reverse will not work reliabily because NSKeyedArchive is a special form of NSPropertyListSerialization

            if (nsObject == null && _kvp.Value is NSData nsData)
                nsObject = NSKeyedUnarchiver.UnarchiveObject(nsData);
            else
                nsData = null;
            //System.Diagnostics.Debug.WriteLine("\t\t\t GetValueAs 2 stopwatch.Elapsed: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();


            if (nsObject == null && nsData != null)
            {
                var propertyListFormat = new NSPropertyListFormat();
                nsObject = NSPropertyListSerialization.PropertyListWithData(nsData, NSPropertyListReadOptions.Immutable, ref propertyListFormat, out NSError nsError);
                nsError?.Dispose();
            }
            //System.Diagnostics.Debug.WriteLine("\t\t\t GetValueAs 3 stopwatch.Elapsed: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();


            if (nsObject == null)
                nsObject = _kvp.Value;
            //System.Diagnostics.Debug.WriteLine("\t\t\t GetValueAs 4 stopwatch.Elapsed: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();


            var result = nsObject?.ToObject(type);
            //System.Diagnostics.Debug.WriteLine("\t\t\t GetValueAs 5 stopwatch.Elapsed: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();

            return result;
        }

        public Type Type { get; internal set; }

        readonly KeyValuePair<NSObject, NSObject> _kvp;
        readonly string _typeCodeString;

        internal LazyMimeItem() { }

        private LazyMimeItem(KeyValuePair<NSObject, NSObject> kvp, string typeCodeString = null)
        {
            if (kvp.Key == null)
                return;
            var uti = kvp.Key.ToString();
            if (uti == null)
                return;
            //var values = UIPasteboard.General.DataForPasteboardType(uti);
            MimeType = uti.ToMime(); // UTType.GetPreferredTag(uti, UTType.TagClassMIMEType);

            _kvp = kvp;
            _typeCodeString = typeCodeString;

        }

        public static LazyMimeItem Parse(KeyValuePair<NSObject, NSObject> kvp, string typeCodeString = null)
        {
            var result = new LazyMimeItem(kvp, typeCodeString);
            return result.MimeType != null ? result : null;
        }
    }
}