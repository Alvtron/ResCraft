using ResourceCraft.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceCraft.Model
{
    public class Report : FirebaseEntity
    {
        public FirebaseKey Target { get; set; }
        public string Message { get; set; }
        public bool Valid => string.IsNullOrWhiteSpace(Message);
        public ICollection<FirebaseImage> ImageAttachments { get; set; } = new List<FirebaseImage>();

        public Report()
        {
        }

        public Report(IFirebaseEntity target, string message)
        {
            Target = target?.Key;
            Message = message;
        }
    }
}
