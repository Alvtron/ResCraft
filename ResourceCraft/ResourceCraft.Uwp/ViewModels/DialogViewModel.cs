using ResourceCraft.Model;

namespace ResourceCraft.Uwp.ViewModels
{
    public abstract class DialogViewModel : ObservableObject
    {
        private bool _canClose;
        public bool CanClose
        {
            get => _canClose;
            set => SetField(ref _canClose, value);
        }
    }
}
