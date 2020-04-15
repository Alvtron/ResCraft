using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ResourceCraft.Uwp.Xaml
{
    public class RichTextBox : BindableRichEditBox
    {
        public RichTextBox()
        {
            //IsEnabled = false;
            IsReadOnly = true;
            AllowFocusOnInteraction = false;
            AllowFocusWhenDisabled = false;
            FocusVisualPrimaryBrush = new SolidColorBrush(Colors.Transparent);
            FocusVisualSecondaryBrush = new SolidColorBrush(Colors.Transparent);
            Background = new SolidColorBrush(Colors.Transparent);
            BorderBrush = new SolidColorBrush(Colors.Transparent);
            BorderThickness = new Thickness(0);
            Padding = new Thickness(0);
        }

        private void ColorFormat()
        {
            IsReadOnly = false;
            Focus(FocusState.Pointer);
            Document.Selection.FindText("namespace", Rtf.Length, FindOptions.Word);
            Document.Selection.CharacterFormat.ForegroundColor = Colors.Red;
            IsReadOnly = true;
        }
    }
}