using System;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using P42.Utils;

namespace P42.Uno.DataTransfer
{
    /// <summary>
    /// Interface for a P42.Uno.DataTransfer.ClipboardEntry
    /// </summary>
    public interface IMimeItemCollection
    {
        /// <summary>
        /// Description of this ClipboardEntry  on clipboard (really only applies to Android)
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Plain text item in this ClipboardEntry
        /// </summary>
        string PlainText { get; }

        /// <summary>
        /// HtmlText item in this ClipboardEntry
        /// </summary>
        string HtmlText { get; }

        /// <summary>
        /// Gets the items in this ClipboardEntry
        /// </summary>
        /// <value>The items.</value>
        List<IMimeItem> Items { get; }
    }
}