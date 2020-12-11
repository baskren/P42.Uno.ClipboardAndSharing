using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace P42.Uno.DataTransfer.UWP
{
    public class SharingService : P42.Uno.DataTransfer.INativeSharingService
    {
        P42.Uno.DataTransfer.MimeItemCollection _mimeItemCollection;

        public SharingService()
        {
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (_mimeItemCollection != null && _mimeItemCollection.Items.Count > 0)
            {
                var request = args.Request;
                request.Data.Source(_mimeItemCollection);
                System.Diagnostics.Debug.WriteLine("DataTransferManager_DataRequested: complete");
            }
        }


        public void Share(P42.Uno.DataTransfer.MimeItemCollection mimeItemCollection, FrameworkElement target)
        {
            _mimeItemCollection = mimeItemCollection;
            if (_mimeItemCollection.Items.Count>0)
                DataTransferManager.ShowShareUI();
        }
    }
}