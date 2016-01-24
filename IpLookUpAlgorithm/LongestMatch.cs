using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpLookUpAlgorithm
{
    class LongestMatch
    {
        public string headPointer {get; set;}
        
        public bool[] longestMatch { get; set; }
        
        public LongestMatch(string pointer, bool[] count) 
        {
            this.headPointer = pointer;
            this.longestMatch = count;
        }

        public LongestMatch()
        {
            this.headPointer = String.Empty;
            this.longestMatch = new bool[32];
        }
    }
}
