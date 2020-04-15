using Newtonsoft.Json;
using ResourceCraft.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCraft.Model
{
    public class FirebaseFile : FirebaseEntity, IFile
    {
        [NotMapped, JsonIgnore]
        public string FileName => $"{Key}{Extension}";

        public string Extension { get; set; }

        public string Description { get; set; }

        public FirebaseFile()
        {

        }

        public FirebaseFile(string extension)
        {
            Extension = extension ?? throw new ArgumentNullException(nameof(extension));
        }

        public override string ToString() => FileName;
    }
}
