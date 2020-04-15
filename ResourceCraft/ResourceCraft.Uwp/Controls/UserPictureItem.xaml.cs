using ResourceCraft.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class UserPictureItem : UserControl
    {
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register("User", typeof(User), typeof(UserPictureItem), new PropertyMetadata(new User()));
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(int), typeof(UserPictureItem), new PropertyMetadata(50));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UserPictureItem), new PropertyMetadata(Orientation.Vertical));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(int), typeof(UserPictureItem), new PropertyMetadata(4));

        public User User
        {
            get => GetValue(UserProperty) as User;
            set => SetValue(UserProperty, value);
        }
        public int Size
        {
            get => (int)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
        public int StrokeThickness
        {
            get => (int)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public UserPictureItem()
        {
            this.InitializeComponent();
        }
    }
}
