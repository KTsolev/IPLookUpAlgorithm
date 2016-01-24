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
        
        public int longestMatch { get; set; }
        
        public LongestMatch(string pointer, int count) 
        {
            this.headPointer = pointer;
            this.longestMatch = count;
        }

        public LongestMatch()
        {
            // TODO: Complete member initialization
        }
    }
}
