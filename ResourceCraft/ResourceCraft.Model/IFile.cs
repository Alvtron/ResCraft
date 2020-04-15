using System;
using System.IO;
using System.Threading.Tasks;

namespace ResourceCraft.Model
{
    public interface IFile
    {
        string Description { get; set; }
        string Extension { get; set; }
        string FileName { get; }
    }
}