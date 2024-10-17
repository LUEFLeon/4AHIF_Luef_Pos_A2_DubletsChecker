using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4AHIF_Luef_Dateidubletten_Filefinder
{
    public class Duplicate : IDuplicate
    {
        public List<string> FilePaths { get; } = new List<string>();

        IEnumerable<string> IDuplicate.FilePaths => new List<string>();
    }
}
