using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Models.Types
{
    public class DetailedWorldMapStatistics
    {
        public string Selection { get; set; }
        public int Confirmed { get; set; }
        public int Active { get; set; }
        public int Recovered { get; set; }
        public int Deaths { get; set; }
    }
}
