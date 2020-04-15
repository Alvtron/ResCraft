using ResourceCraft.DataAccess;
using ResourceCraft.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class AsyncImage : UserControl
    {
        public static readonly DependencyProperty WebFileProperty = DependencyProperty.Register("Source", typeof(IFile), typeof(AsyncImage), new PropertyMetadata(default(IFile)));
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(AsyncImage), new PropertyMetadata(default(Stretch)));

        public IFile File
        {
            get => GetValue(WebFileProperty) as IFile;
            set
            {
                SetValue(WebFileProperty, value);
                DownloadImageAsync();
            }
        }

        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public AsyncImage()
        {
            this.InitializeComponent();
        }

        private async void DownloadImageAsync()
        {
            if (File == null)
            {
                ImageHolder.Source = null;
                return;
            }

            var url = await StorageSource.GetFileUrlAsync($"images/{File.FileName}");
            var image = new BitmapImage(url);

            ImageHolder.Source = image;
        }
    }
}
