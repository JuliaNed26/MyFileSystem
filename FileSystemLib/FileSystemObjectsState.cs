using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemLib
{
    [Flags]
    public enum FileSystemObjectsState
    {
        Copied = 1,
        Cut = 2
    }
}
