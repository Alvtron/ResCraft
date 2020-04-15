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
    public sealed partial class AsyncPersonPicture : UserControl
    {
        public static readonly DependencyProperty WebFileProperty = DependencyProperty.Register("File", typeof(IFile), typeof(AsyncPersonPicture), new PropertyMetadata(default(IFile)));
        public new static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(AsyncPersonPicture), new PropertyMetadata(default(double)));
        public new static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(AsyncPersonPicture), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(AsyncPersonPicture), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty InitialsProperty = DependencyProperty.Register("Initials", typeof(string), typeof(AsyncPersonPicture), new PropertyMetadata(default(string)));
        
        public IFile File
        {
            get => GetValue(WebFileProperty) as IFile;
            set
            {
                SetValue(WebFileProperty, value);
                DownloadImageAsync();
            }
        }

        public new double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public new double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public string DisplayName
        {
            get => GetValue(DisplayNameProperty) as string;
            set => SetValue(DisplayNameProperty, value);
        }

        public string Inititals
        {
            get => GetValue(InitialsProperty) as string;
            set => SetValue(InitialsProperty, value);
        }

        public AsyncPersonPicture()
        {
            this.InitializeComponent();
        }

        private async void DownloadImageAsync()
        {
            if (File == null)
            {
                ImageHolder.ProfilePicture = null;
                return;
            }

            var url = await StorageSource.GetFileUrlAsync($"images/{File.FileName}");
            var image = new BitmapImage(url);

            ImageHolder.ProfilePicture = image;
        }
    }
}
