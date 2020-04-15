using System.Collections.Generic;

namespace ResourceCraft.Minecraft
{
    public class VersionComparer : IEqualityComparer<string>, IComparer<string>, IComparer<Version>
    {
        public int Compare(string x, string y)
        {
            return MinecraftAPI.CompareVersions(x, y);
        }

        public int Compare(Version x, Version y)
        {
            return x.ReleaseTime.CompareTo(y.ReleaseTime);
        }

        public bool Equals(string x, string y)
        {
            return MinecraftAPI.CompareVersions(x, y) == 0;
        }

        public int GetHashCode(string version)
        {
            return version.GetHashCode();
        }
    }
}
