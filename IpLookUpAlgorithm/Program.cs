using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IpLookUpAlgorithm
{
    class IpLookUpAlgorithm
    {

        static Dictionary<string[], List<Node>> Prefixes = new Dictionary<string[], List<Node>>();
        static BinaryTreeImp Tree = new BinaryTreeImp();
        static List<LongestMatch> ListOfLongestMatches = new List<LongestMatch>();
        static LongestMatch lm = new LongestMatch();


        //converting from dec to bin//
        public static string ConvertDecToBin(string address)
        {
            string[] digits = address.Split('.');
            StringBuilder sb = new StringBuilder();
            int fromBase = 10;
            int toBase = 2;

            foreach (var number in digits)
            {
                string zeros = string.Empty;
                string result = Convert.ToString(Convert.ToInt32(number, fromBase), toBase);
                //for making all parts of IP address to be in 8 symbols length//
                if (result.Length <= 7)
                {
                    for (int i = 0; i <= (7 - result.Length); i++)
                    {
                        zeros += "0";
                    }
                    result = zeros + result;
                }
                sb.Append(result);
            }
            return sb.ToString();
        }

        //calculates network ip address from subnet mask by doing logical AND operation//
        //calculates and the range of given subnet//
        public static string FindTheIpRange(string ipAddr, string subNetMask)
        {
            StringBuilder startIP = new StringBuilder();
            StringBuilder endIP = new StringBuilder();
            for (int i = 0; i < ipAddr.Length; i++)
            {
                if (ipAddr[i] == subNetMask[i])
                {
                    if (i < 24)
                    {
                        startIP.Append(subNetMask[i]);
                        endIP.Append(subNetMask[i]);
                    }
                }
                else
                {
                    if (i < 24)
                    {
                        startIP.Append(ipAddr[i]);
                        endIP.Append(ipAddr[i]);
                    }
                }
            }
            //making logical OR operation to find last ip address of the given subnet//
            int l = 24;

            while (l < 32)
            {
                endIP.Append("1");
                startIP.Append("0");
                l++;
            }
            string range = startIP.ToString() + ":" + endIP.ToString();
            return range;
        }

        //calculating the subnet mask and representing it into binary form//
        public static string ReturnMask(int mask)
        {
            bool[] binMask = new bool[32];
            StringBuilder binaryMask = new StringBuilder();
            for (int i = 0; i < mask; i++)
            {
                binMask[i] = true;
            }
            for (int i = 0; i < binMask.Length; i++)
            {
                if (binMask[i])
                    binaryMask.Append("1");
                else
                    binaryMask.Append("0");
            }
            return binaryMask.ToString();
        }
        //Creates index array by calculating subnets and combines nodes depending to the subnet in which they are//
        public static void CreateRadixArray(Node root) 
        {
            Node temp;
            temp = root;
            if(root == null) return;

            CreateRadixArray(root.left);
            string[] ranges =  FindTheIpRange(temp.data.IpAddres, temp.data.Prefix).Split(':');
            
            if (!Prefixes.ContainsKey(ranges)) 
            {
                Prefixes.Add(ranges, new List<Node>());
            }
            if (temp.data.IpAddres.CompareTo(ranges[0]) >= 0 && temp.data.IpAddres.CompareTo(ranges[1]) <= 0) 
            {
                Prefixes[ranges].Add(temp);
            }
            CreateRadixArray(root.right);
        }

        ////converting ip to bin and breaking address into strides//
        public void AddressSplitUp(string IP, int maskLen, int stride)
        {
            string binIP = ConvertDecToBin(IP);
            int size = (maskLen / stride);
            string[] strides = new string[size];
            int index = 0;
            //braking IP into separate parts/strides//
            for (int i = 0, p = 0; i < size; i++, p++)
            {
                if ((index >= binIP.Length) || ((index + stride) > binIP.Length)) break;
                strides[p] = binIP.Substring(index, stride);
                index += stride;
            }
            index = 0;
            foreach (var s in strides) 
            {
                FindNextHop(s, index, Tree.Root);
                index += s.Length;
            }
        }
        public static int CheckIfWeAreFoundLongestMatch(bool[] arr)
        {
            int count = 0;
            foreach(var a in arr)
            {
                if(a)
                count++;
            }
            return count;
        }
        //searches for the ip among ips stored in the dictionary//
        public void FindNextHop(string stride,int offset,Node root) 
        {
            Node temp;
            temp = root;

            if (temp == null) 
                return;
            if (stride.Length == 0 || stride == null)
                return;
            foreach (var key in Prefixes.Keys) 
            {
                for (int i = offset; i < offset + stride.Length; i++)
                {
                    if (key[0][i] == stride[i - offset])
                    {
                        lm.longestMatch[i] = true;
                    }
                }

                lm.headPointer = temp.data.IpAddres;
                ListOfLongestMatches.Add(lm);

                int count = CheckIfWeAreFoundLongestMatch(lm.longestMatch);

                if (count >= 2)
                {
                    return;
                }
            }             

        }
        //loading IP addresses into the tree formation from json file//
        public static void ReadJson(string name)
        {
            //Read the file      
            string path = @"../../";
            try
            {
                using (StreamReader r = new StreamReader(path + name))
                {
                    string json = r.ReadToEnd();
                    dynamic array = JsonConvert.DeserializeObject(json);
                    foreach (var item in array["Hosts"])
                    {
                        Data data = new Data();
                        //Console.WriteLine(item.ToString());
                        data.IpAddres = ConvertDecToBin(item["IpAddress"].ToString());
                        int temp = Int32.Parse(item["Prefix"].ToString());
                        data.MacAddres = item["MacAddress"].ToString();
                        data.Prefix = ReturnMask(temp);
                        Node node = new Node(data);
                        if (Tree.Root == null)
                        {
                            Node iniRoot = Tree.AddNode(data);
                            Tree.InsertNode(Tree.Root, iniRoot);
                        }
                        else
                        {
                            Tree.InsertNode(Tree.Root, Tree.AddNode(data));
                        }
                    }

                    Tree.DisplayTree(Tree.Root);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
            }
        }
        static void Main(string[] args)
        {
            IpLookUpAlgorithm iplp = new IpLookUpAlgorithm();
            ReadJson("IpConfig.json");
            CreateRadixArray(Tree.Root);
            iplp.AddressSplitUp("192.168.10.5", 24, 4);
            Console.WriteLine("###################################");
            foreach (var pair in Prefixes)
            {
                foreach (var k in pair.Key)
                {
                    Console.WriteLine(k);
                }
                foreach (var v in pair.Value)
                {
                    Console.WriteLine("     " + v.data.IpAddres);
                }
            }
            Console.WriteLine("----------------------------------");
            foreach (var lm2 in ListOfLongestMatches)
            {
                Console.WriteLine(lm2.headPointer + "|" + CheckIfWeAreFoundLongestMatch(lm2.longestMatch));
            }
        }
    }
}

