using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCraft.Minecraft
{
    public class Resource
    {
        public string Path { get; set; }
        public string Version { get; set; }
        [JsonIgnore]
        public long Size { get; set; }
        [JsonIgnore]
        public byte[] Hash { get; set; }
        [JsonIgnore]
        public string Name
        {
            get
            {
                var nameLowercase = System.IO.Path.GetFileNameWithoutExtension(Path).Replace('_', ' ');
                List<string> nameArray = new List<string>();

                bool nextIsUpper = false;

                for (int index = 0; index < nameLowercase.Length; index++)
                {
                    var letter = nameLowercase[index];

                    if (index == 0 || nextIsUpper) nameArray.Add(letter.ToString().ToUpper());
                    else nameArray.Add(letter.ToString().ToLower());

                    nextIsUpper = (letter == ' ');
                }

                return string.Join("", nameArray);
            }
        }
        [JsonIgnore]
        public string Filename => System.IO.Path.GetFileName(Path);
        [JsonIgnore]
        public string FileType => System.IO.Path.GetExtension(Path);
        [JsonIgnore]
        public bool Valid => !string.IsNullOrWhiteSpace(Path) && !string.IsNullOrWhiteSpace(Version);

        public Resource(string path, string version, long size, byte[] hash)
        {
            Path = path;
            Version = version;
            Size = size;
            Hash = hash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Resource);
        }

        public bool Equals(Resource resource)
        {
            return Path == resource.Path && Version == resource.Version;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
