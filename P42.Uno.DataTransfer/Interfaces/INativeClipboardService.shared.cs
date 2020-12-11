using System;

namespace P42.Uno.DataTransfer
{
    interface INativeClipboardService
    {
        IMimeItemCollection Entry { get; set; }

        bool EntryCaching { get; set; }

        //bool EntryItemTypeCaching { get; set; }
    }
}