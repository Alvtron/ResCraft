using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ResourceCraft.Model;
using ResourceCraft.Utilities;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class ReplyListView : UserControl
    {
        public static readonly DependencyProperty RepliesProperty = DependencyProperty.Register("Replies", typeof(ObservableCollection<Reply>), typeof(ReplyListView), new PropertyMetadata(new ObservableCollection<Reply>()));

        private bool Initialized { get; set; } = false;

        public ObservableCollection<Reply> Replies
        {
            get => GetValue(RepliesProperty) as ObservableCollection<Reply>;
            set
            {
                if (value == null)
                {
                    Logger.WriteLine($"Failed to initialize '{nameof(Replies)}' in {nameof(ReplyListView)}. Value was null.");
                    return;
                }

                SetValue(RepliesProperty, value);
                Logger.WriteLine($"Successfully initialized '{nameof(Replies)}' in {nameof(ReplyListView)}.");

                if (!Initialized)
                {
                    InitializeComponent();
                    Initialized = true;
                }
            }
        }
    }
}
