namespace ResourceCraft.Model
{
    public static class Experience
    {
        public enum Action
        {
            AddResourcePack = 500,
            AddResource = 100,
            AddPost = 100,
            AddReply = 50,
            UploadFile = 10,
            UploadImage = 10,
            UploadVideo = 10,
            Befriend = 50,
            SignIn = 10,
            ChangedSettings = 5
        };

        public static readonly int LevelUpExp = 5000;

        public static int ExpToLevel(int exp)
        {
            return exp / LevelUpExp;
        }

        public static int LevelToExp(int level)
        {
            return level * LevelUpExp;
        }

        public static int ProgressExp(int exp)
        {
            return exp % LevelUpExp;
        }

        public static double ProgressExpInPercentage(int exp)
        {
            return (exp % LevelUpExp / (double)LevelUpExp) * 100.00;
        }
    }
}
