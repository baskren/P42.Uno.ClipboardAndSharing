﻿using System;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using P42.Utils;


namespace P42.Uno.DataTransfer
{
    /// <summary>
    /// Generic Interface for ClipboardEntryITem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMimeItem<T> : IMimeItem
    {
        /// <summary>
        /// Gets the value of this IClipboardEntryItem
        /// </summary>
        /// <value>The value.</value>
        new T Value { get; }
    }


    /// <summary>
    /// Interface for a MimeItem
    /// </summary>
    public interface IMimeItem
    {
        /// <summary>
        /// MimeType for an item
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// Value of the item
        /// </summary>
        object Value { get; }

        /*
        /// <summary>
        /// Type of the item (to help you with type conversion)
        /// </summary>
        Type Type { get; }
        */
    }


    internal interface INativeMimeItem : IMimeItem
    {
        object GetValueAs(Type type);
    }
}