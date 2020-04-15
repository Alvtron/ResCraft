using ResourceCraft.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ResourceCraft.Uwp.ViewModels
{
    public class UsersViewModel : ObservableObject
    {
        private ObservableCollection<User> _allUsers;
        public ObservableCollection<User> AllUsers
        {
            get => _allUsers;
            set => SetField(ref _allUsers, value);
        }

        private ObservableCollection<User> _filteredUsers;
        public ObservableCollection<User> FilteredUsers
        {
            get => _filteredUsers;
            set => SetField(ref _filteredUsers, value);
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SearchByQuery(value);
                SetField(ref _searchQuery, value);
            }
        }

        public UsersViewModel(IEnumerable<User> users)
        {
            if (users == null)
            {
                throw new ArgumentNullException("Users was null");
            }

            AllUsers = new ObservableCollection<User>(users);
            FilteredUsers = AllUsers;

            AllUsers.CollectionChanged += (s, e) =>
            {
                SearchByQuery(SearchQuery);
            };
        }

        private void SearchByQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
                FilteredUsers = AllUsers;

            query = query?.ToLower();

            FilteredUsers = new ObservableCollection<User>(AllUsers?.Where(u => u.UserName.ToLower().StartsWith(query)));
        }
    }
}
