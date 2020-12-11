using System;
using Android.Content;
using Android.Webkit;
using Android.Database;
using Android.Net;
using System.Collections.Generic;
using Java.Lang.Reflect;
using System.Runtime.InteropServices;
using Java.Nio.FileNio;
using System.Collections;
using System.IO;
using System.Linq;
using Android.OS;
using System.Reflection;

namespace P42.Uno.DataTransfer.Droid
{
    class MimeItemCollection : BaseMimeItemCollection
    {
        #region Static implementation
        readonly internal static Dictionary<Android.Net.Uri, P42.Uno.DataTransfer.IMimeItem> UriItems = new Dictionary<Android.Net.Uri, IMimeItem>();

        static ClipboardManager _clipboardManager;
        static ClipboardManager Clipboard
        {
            get
            {
                _clipboardManager = _clipboardManager ?? (ClipboardManager)Android.App.Application.Context.GetSystemService(Context.ClipboardService);
                return _clipboardManager;
            }
        }
        #endregion


        #region Properties
        public override string Description => Clipboard.PrimaryClip.Description.Label;

        public override List<IMimeItem> Items
        {
            get
            {
                if (_items != null)
                    return _items;
                _items = new List<IMimeItem>();
                if (Clipboard.HasPrimaryClip && Clipboard.PrimaryClip is ClipData clipData && clipData.ItemCount > 0)
                {
                    for (int i = 0; i < clipData.ItemCount; i++)
                    {
                        var item = clipData.GetItemAt(i);
                        var returnMimeItem = ReturnMimeItem.Parse(item);
                        if (returnMimeItem != null)
                            _items.Add(returnMimeItem);
                    }
                }
                return _items;
            }
        }



        #endregion


    }
}