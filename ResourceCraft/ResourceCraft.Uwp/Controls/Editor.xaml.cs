using ResourceCraft.Uwp.Utilities;
using System.Windows.Input;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class Editor : UserControl
    {
        public static readonly DependencyProperty Headerproperty = DependencyProperty.Register("Header", typeof(string), typeof(Editor), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(Editor), new PropertyMetadata(true, EditablePropertyChanged));

        public static readonly DependencyProperty RtfProperty = DependencyProperty.Register("Rtf", typeof(string), typeof(Editor), new PropertyMetadata(default(string), RtfPropertyChanged));

        private RelayCommand _boldCommand;
        public ICommand BoldCommand => _boldCommand = _boldCommand ?? new RelayCommand(parameter => OnBold());

        private RelayCommand _italicCommand;
        public ICommand ItalicCommand => _italicCommand = _italicCommand ?? new RelayCommand(parameter => OnItalic());

        private RelayCommand _underlineCommand;
        public ICommand UnderlineCommand => _underlineCommand = _underlineCommand ?? new RelayCommand(parameter => OnUnderline());

        private RelayCommand _clearCommand;
        public ICommand ClearCommand => _clearCommand = _clearCommand ?? new RelayCommand(parameter => Clear());

        public string Header
        {
            get => (string)GetValue(Headerproperty);
            set => SetValue(Headerproperty, value);
        }

        public bool Editable
        {
            get => (bool)GetValue(EditableProperty);
            set => SetValue(EditableProperty, value);
        }

        public string Rtf
        {
            get => (string)GetValue(RtfProperty);
            set => SetValue(RtfProperty, value);
        }

        public string Text
        {
            get
            {
                EditorBox.Document.GetText(TextGetOptions.None, out string text);
                return text;
            }
        }

        public string ObjectText
        {
            get
            {
                EditorBox.Document.GetText(TextGetOptions.UseObjectText, out string text);
                return text;
            }
        }

        private bool LockChangeExecution { get; set; }

        public Editor()
        {
            InitializeComponent();
        }

        private void EditorBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!LockChangeExecution)
            {
                LockChangeExecution = true;

                EditorBox.Document.GetText(TextGetOptions.FormatRtf, out var rtf);
                Rtf = rtf;
                
                LockChangeExecution = false;
            }
        }

        private void UpdateRtfText(string rtf)
        {
            if (!LockChangeExecution)
            {
                EditorBox.IsReadOnly = false;
                EditorBox.Document.SetText(TextSetOptions.FormatRtf, rtf);
                EditorBox.IsReadOnly = !Editable;
            }
        }

        public void Clear()
        {
            EditorBox.IsReadOnly = false;
            EditorBox.Document.SetText(TextSetOptions.None, "");
            EditorBox.IsReadOnly = true;
        }

        private static void RtfPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is Editor editor))
            {
                return;
            }

            editor.UpdateRtfText(dependencyPropertyChangedEventArgs.NewValue as string);
        }

        private static void EditablePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is Editor rtb)) return;
            if (!rtb.LockChangeExecution)
            {
                rtb.EditorBox.IsReadOnly = !rtb.Editable;
            }
        }

        private void OnBold()
        {
            EditorBox.IsReadOnly = false;
            var selectedText = EditorBox.Document.Selection;
            if (selectedText != null)
            {
                var charFormatting = selectedText.CharacterFormat;
                charFormatting.Bold = FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
            EditorBox.IsReadOnly = !Editable;
        }

        private void OnItalic()
        {
            EditorBox.IsReadOnly = false;
            var selectedText = EditorBox.Document.Selection;
            if (selectedText != null)
            {
                var charFormatting = selectedText.CharacterFormat;
                charFormatting.Italic = FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
            EditorBox.IsReadOnly = !Editable;
        }

        private void OnUnderline()
        {
            EditorBox.IsReadOnly = false;
            var selectedText = EditorBox.Document.Selection;
            if (selectedText != null)
            {
                var charFormatting = selectedText.CharacterFormat;
                if (charFormatting.Underline == UnderlineType.None)
                {
                    charFormatting.Underline = UnderlineType.Single;
                }
                else
                {
                    charFormatting.Underline = UnderlineType.None;
                }
                selectedText.CharacterFormat = charFormatting;
            }
            EditorBox.IsReadOnly = !Editable;
        }
    }
}
