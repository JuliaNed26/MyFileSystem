using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSystem
{
    [Flags]
    internal enum FilesActions
    {
        Copied = 1,
        Cut = 2
    }
}
