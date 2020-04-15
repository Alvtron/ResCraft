using ResourceCraft.Model;
using ResourceCraft.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class PostListView : UserControl
    {
        public static readonly DependencyProperty PostsProperty = DependencyProperty.Register("Posts", typeof(ObservableCollection<Post>), typeof(PostListView), new PropertyMetadata(new ObservableCollection<Post>()));

        public ObservableCollection<Post> Posts
        {
            get => GetValue(PostsProperty) as ObservableCollection<Post>;
            set
            {
                if (value == null)
                {
                    Logger.WriteLine($"Failed to initialize '{nameof(Posts)}' in {nameof(ReplyListView)}. Value was null.");
                    return;
                }

                SetValue(PostsProperty, value);
                Logger.WriteLine($"Successfully initialized '{nameof(Posts)}' in {nameof(ReplyListView)}.");
            }
        }
    }
}
