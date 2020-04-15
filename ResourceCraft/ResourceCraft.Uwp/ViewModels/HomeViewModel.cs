using ResourceCraft.Model;
using ResourceCraft.DataAccess;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceCraft.Uwp.ViewModels
{
    public class HomeViewModel
    {
        public ObservableCollection<User> NewestUsers { get; private set; } = new ObservableCollection<User>();
        
        public async Task RefreshNewestUsers()
        {
            NewestUsers.Clear();
            var users = await RestApiService<User>.Get();

            if (users == null)
            {
                return;
            }

            users.OrderByDescending(x => x.Created).ToList().ForEach(NewestUsers.Add);
        }
    }
}
