using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpLookUpAlgorithm
{
    //Data for the Node class//
    struct Data
    {
        public string IpAddres { get; set; }

        public string MacAddres { get; set; }

        public string Prefix { get; set; }

    }
    class Node
    {
        public Data data { get; set; }
        public Node left { get; set; }

        public Node right { get; set; }

        public Node(Data data)
        {
            this.data = data;
            left = null;
            right = null;
        }
    }
}
