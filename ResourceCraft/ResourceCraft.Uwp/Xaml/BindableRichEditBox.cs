using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ResourceCraft.Uwp.Xaml
{
    public class BindableRichEditBox : RichEditBox
    {
        public static readonly DependencyProperty RtfProperty = DependencyProperty.Register("Rtf", typeof(string), typeof(BindableRichEditBox), new PropertyMetadata(default(string), RtfPropertyChanged));

        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(BindableRichEditBox), new PropertyMetadata(default(bool), EditablePropertyChanged));

        private bool _lockChangeExecution;

        public string Rtf
        {
            get => (string)GetValue(RtfProperty);
            set => SetValue(RtfProperty, value);
        }

        public string Text
        {
            get
            {
                Document.GetText(TextGetOptions.None, out string text);
                return text;
            }
        }

        public bool Editable
        {
            get => (bool)GetValue(EditableProperty);
            set => SetValue(EditableProperty, value);
        }

        public BindableRichEditBox()
        {
            TextChanged += BindableRichEditBox_TextChanged;
        }

        private void BindableRichEditBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!_lockChangeExecution)
            {
                _lockChangeExecution = true;
                Document.GetText(TextGetOptions.None, out var text);
                if (string.IsNullOrWhiteSpace(text))
                {
                    Rtf = "";
                }
                else
                {
                    Document.GetText(TextGetOptions.FormatRtf, out text);
                    Rtf = text;
                }
                _lockChangeExecution = false;
            }
        }

        public void Clear()
        {
            IsReadOnly = false;
            Document.SetText(TextSetOptions.None, "");
            IsReadOnly = true;
        }

        private static void RtfPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is BindableRichEditBox rtb)) return;
            if (!rtb._lockChangeExecution)
            {
                rtb.IsReadOnly = false;
                rtb.Document.SetText(TextSetOptions.FormatRtf, dependencyPropertyChangedEventArgs.NewValue as string);
                rtb.IsReadOnly = !rtb.Editable;
            }
        }

        private static void EditablePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is BindableRichEditBox rtb)) return;
            if (!rtb._lockChangeExecution)
            {
                rtb.IsReadOnly = !rtb.Editable;
            }
        }
    }
}
