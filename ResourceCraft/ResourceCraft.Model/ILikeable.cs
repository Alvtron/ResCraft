using System.Collections.ObjectModel;

namespace ResourceCraft.Model
{
    public interface ILikeable
    {
        ObservableCollection<Rating> Ratings { get; set; }
        int NumberOfLikes { get; }
        int NumberOfDislikes { get; }

        void Like(User user);
        void Dislike(User user);
        bool HasLiked(User user);
        bool HasDisliked(User user);
    }
}