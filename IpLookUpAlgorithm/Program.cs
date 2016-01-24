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

        public static Dictionary<string[], List<Node>> Prefixes = new Dictionary<string[], List<Node>>();
        public static BinaryTreeImp Tree = new BinaryTreeImp();
        public static byte[] Network = new byte[32];
        public static byte[] NextHop = new byte[32]; 
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

        //converting ip to bin and breaking address into strides//
        public static void AddressSplitUp(string IP, int maskLen, int stride)
        {
            string binIP = ConvertDecToBin(IP);
            int size = (maskLen/stride);
            string[] strides = new string[size];
            int index = 0;
            //braking IP into separate parts/strides//
            for (int i = 0, p = 0; i < size; i++, p++)
            {
                if ((index >= binIP.Length) || ((index + stride) > binIP.Length)) break;
                strides[p] = binIP.Substring(index, stride);
                index += stride;
            }
        }
        //calculates network ip address from subnet mask by doing logical AND operation//
        //calculates and the range of given subnet//
        public static string FindTheIpRange(string ipAddr,string subNetMask) 
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
            
            while ( l < 32)
            {
                endIP.Append("1");
                startIP.Append("0");
                l++;
            }
            string range = startIP.ToString() + ":" + endIP.ToString();

            Console.WriteLine(startIP);
            Console.WriteLine(endIP);
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
                if(binMask[i])
                    binaryMask.Append("1");
                else
                    binaryMask.Append("0");
            }
            return binaryMask.ToString();
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
            ReadJson("IpConfig.json");
            AddressSplitUp("192.168.10.4", 24, 4);
            CreateRadixArray(Tree.Root);
            foreach (var pair in Prefixes) 
            {
                foreach (var k in pair.Key)
                {
                    Console.WriteLine(k);
                }
                foreach (var v in pair.Value)
                {
                    Console.WriteLine("     "+v.data.IpAddres);
                }
            }
        }
    }
}

