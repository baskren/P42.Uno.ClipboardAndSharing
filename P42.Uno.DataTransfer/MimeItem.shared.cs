﻿using System;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using P42.Utils;
using System.Diagnostics;
using System.IO;

namespace P42.Uno.DataTransfer
{

    /// <summary>
    /// MimeItem class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MimeItem<T> : MimeItem, IMimeItem<T>
    {
        /// <summary>
        /// Gets or sets the value of this MimeItemn.
        /// </summary>
        /// <value>The value.</value>
        new public T Value
        {
            get
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                if (_mimeItem != null)
                {
                    if (_mimeItem is INativeMimeItem nativeMimeItem)
                    {
                        var result = nativeMimeItem.GetValueAs(typeof(T));
                        try
                        {
                            stopwatch.Stop();
                            //System.Diagnostics.Debug.WriteLine("\t\t MimeItem<T> get_Value A elapsed: " + stopwatch.ElapsedMilliseconds);
                            return (T)result;
                        }
                        catch (Exception)
                        {
                            stopwatch.Stop();
                            //System.Diagnostics.Debug.WriteLine("\t\t MimeItem<T> get_Value B elapsed: " + stopwatch.ElapsedMilliseconds);
                            return default;
                        }
                    }
                    stopwatch.Stop();
                    //System.Diagnostics.Debug.WriteLine("\t\t MimeItem<T> get_Value C elapsed: " + stopwatch.ElapsedMilliseconds);
                    if (_mimeItem.Value is byte[] byteArray && typeof(T) == typeof(string))
                    {
                        object result = System.Text.Encoding.UTF8.GetString(byteArray);
                        return (T)result;
                    }
                    return (T)_mimeItem.Value;
                }
                stopwatch.Stop();
                //System.Diagnostics.Debug.WriteLine("\t\t MimeItem<T> get_Value D elapsed: " + stopwatch.ElapsedMilliseconds);
                return (T)base.Value;
            }
            set => base.Value = value;
        }

        readonly IMimeItem _mimeItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:P42.Uno.DataTransfer.MimeItem`1"/> class.
        /// </summary>
        /// <param name="mimeType">MIME type.</param>
        /// <param name="value">Value.</param>
        public MimeItem(string mimeType, T value) : base(mimeType, value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:P42.Uno.DataTransfer.MimeItem`1"/> class.
        /// </summary>
        /// <param name="mimeItem">MIME item.</param>
        public MimeItem(IMimeItem mimeItem) : base(mimeItem.MimeType, null)
        {
            _mimeItem = mimeItem;
        }

        /// <summary>
        /// Create a new MimeItem with the specified mimeType and value.
        /// </summary>
        /// <returns>The create.</returns>
        /// <param name="mimeType">MIME type.</param>
        /// <param name="value">Value.</param>
        public static MimeItem<T> Create(string mimeType, object value)
        {
            if (value != null)
            {
                var valueType = value.GetType();
                if (typeof(T).IsAssignableFrom(valueType))
                    return new MimeItem<T>(mimeType, (T)value);
                if (value is T[] array)
                    return new MimeItem<T>(mimeType, array[0]);
            }
            return null;
        }

        internal static MimeItem<T> Create(IMimeItem mimeItem)
        {
            if (!(mimeItem is INativeMimeItem) && mimeItem?.Value == null)
                return null;
            return new MimeItem<T>(mimeItem);
        }
    }


    /// <summary>
    /// Base class for a MimeItem
    /// </summary>
    public class MimeItem : IMimeItem //, IPlatformKey
    {
        #region object validity check
        /// <summary>
        /// Checks if an object can be used as a valid MimeItem.Value
        /// </summary>
        /// <returns><c>true</c>, if value was valided, <c>false</c> otherwise.</returns>
        /// <param name="value">Value.</param>
        public static bool ValidValue(object value) => ValidValueType(value.GetType());


        /// <summary>
        /// Test to determine if type can be safely put on clipboard across platforms (without crazy schema gymnastics)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ValidValueType(Type type)
        {
            if (type == typeof(byte[]))
                return true;
            if (type == typeof(byte))
                return true;
            if (type == typeof(char))
                return true;
            if (type == typeof(ushort))
                return true;
            if (type == typeof(short))
                return true;
            if (type == typeof(uint))
                return true;
            if (type == typeof(int))
                return true;
            if (type == typeof(ulong))
                return true;
            if (type == typeof(long))
                return true;
            if (type == typeof(float))
                return true;
            if (type == typeof(double))
                return true;
            if (type == typeof(decimal))
                return true;
            if (type == typeof(string))
                return true;
            if (type == typeof(FileInfo))
                return true;

            var typeInfo = type.GetTypeInfo();
            //if (item.Value is IList ilist && typeInfo.IsGenericType)
            if (typeInfo.ImplementedInterfaces.Contains(typeof(IList)) && typeInfo.IsGenericType)
            {
                var elementType = typeInfo.GenericTypeArguments[0];
                if (elementType == typeof(string))  // need to do this because string is an IEnumerable
                    return true;
                var elementTypeInfo = elementType.GetTypeInfo();
                if (elementTypeInfo.ImplementedInterfaces.Contains(typeof(IDictionary)) && typeInfo.IsGenericType)
                    return ValidDictionary(elementTypeInfo);
                if (elementTypeInfo.ImplementedInterfaces.Contains(typeof(IEnumerable)))
                    return false;
                return ValidValueType(elementType);
            }
            if (typeInfo.ImplementedInterfaces.Contains(typeof(IDictionary)) && typeInfo.IsGenericType)
                return ValidDictionary(typeInfo);// && ValidItemType(valueType);

            return false;
        }

        static bool ValidDictionary(TypeInfo typeInfo)
        {
            var keyType = typeInfo.GenericTypeArguments[0];
            var valueType = typeInfo.GenericTypeArguments[1];
            if (keyType != typeof(string))
                return false;
            if (valueType == typeof(string))
                return true;
            if (valueType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerable)) || valueType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDictionary)))
                return false;
            return ValidValueType(keyType);// && ValidItemType(valueType);
        }
        #endregion





        string _mimeType;
        /// <summary>
        /// Get the MimeType of this MimeItem
        /// </summary>
        public string MimeType
        {
            get => _mimeType;
            protected set
            {
                _mimeType = value?.ToLower();
            }
        }

        object _item;
        /// <summary>
        /// Get the Value of thie MimeItem
        /// </summary>
        public object Value
        {
            get => _item;
            protected set => _item = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:P42.Uno.DataTransfer.MimeItem"/> class.
        /// </summary>
        /// <param name="mimeType">MIME type.</param>
        /// <param name="value">Value.</param>
        public MimeItem(string mimeType, object value)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
                throw new Exception("Empty or null mime type is not allowed.");
            //if (value == null)
            //    throw new Exception("null value is not allowed.");
            /*
            Type = value?.GetType();
            if (!ClipboardEntry.ValidItemType(Type))
                throw new ArgumentException("Item type [" + Type + "] is not a valid for use with MimeType.");
                */
            MimeType = mimeType;
            Value = value;
        }



    }

}

