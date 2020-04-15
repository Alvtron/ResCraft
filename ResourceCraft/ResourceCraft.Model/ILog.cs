namespace ResourceCraft.Model
{
    public interface ILog : IFirebaseEntity
    {
        string Action { get; set; }
        FirebaseKey Actor { get; set; }
        bool IsPublic { get; set; }
        FirebaseKey Subject { get; set; }
    }
}