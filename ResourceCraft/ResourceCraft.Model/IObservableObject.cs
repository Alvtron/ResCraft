using System.ComponentModel;

namespace ResourceCraft.Model
{
    public interface IObservableObject : INotifyPropertyChanged
    {
        void RefreshBindings();
    }
}