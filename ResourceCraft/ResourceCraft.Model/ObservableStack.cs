using System.Collections.Generic;
using System.Collections.Specialized;

namespace ResourceCraft.Model
{
    public class ObservableStack<T> : Stack<T>, INotifyCollectionChanged
    {
        public ObservableStack()
        {
        }

        public ObservableStack(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                base.Push(item);
        }

        public ObservableStack(IReadOnlyCollection<T> list)
        {
            foreach (var item in list)
                base.Push(item);
        }

        public new virtual void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new virtual T Pop()
        {
            var item = base.Pop();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, Count));
            return item;
        }

        public new virtual void Push(T item)
        {
            base.Push(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            RaiseCollectionChanged(e);
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}
