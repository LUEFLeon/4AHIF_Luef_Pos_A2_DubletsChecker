using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4AHIF_Luef_Dateidubletten_Filefinder
{
    public class Duplicate : IDuplicate
    {
        public IEnumerable<string> FilePath { get; }
        public Duplicate(IEnumerable<string> filePaths)
        {
            FilePath = filePaths;
        }
    }
}
