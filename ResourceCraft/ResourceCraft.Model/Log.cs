using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceCraft.Model
{
    public class Log : FirebaseEntity, ILog
    {
        public bool IsPublic { get; set; }
        public string Action { get; set; }
        public FirebaseKey Actor { get; set; }
        public FirebaseKey Subject { get; set; }

        public Log()
        {
        }

        public Log(FirebaseKey actorKey, string action, FirebaseKey subjectKey = null, bool isPublic = true)
        {
            Action = action;
            Actor = actorKey;
            Subject = subjectKey;
            IsPublic = isPublic;
        }
    }
}
