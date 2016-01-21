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

        public static Dictionary<string, List<Node>> Prefixes = new Dictionary<string, List<Node>>();
        public static BinaryTreeImp tree = new BinaryTreeImp();
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

        //converting ip to bin and breaking address into strides//
        public static void IpLookUp(string IP, int maskLen, int stride)
        {
            string binIP = ConvertDecToBin(IP);
            int size = (maskLen/stride);
            string[] strides = new string[size];
            int index = 0;
            //braking IP to the actual strides//
            for (int i = 0, p = 0; i < size; i++, p++)
            {
                if ((index >= binIP.Length) || ((index + stride) > binIP.Length)) break;
                strides[p] = binIP.Substring(index, stride);
                index += stride;
            }
            int offset = 0;
            for (int i = 0; i < strides.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                if (i > strides.Length) 
                    break;
                for (int j = 0; j <= i; j++) 
                {
                    sb.Append(strides[j]);
                }
                CreateIndexArray(sb.ToString(), tree.root);
            }
            foreach (var s in Prefixes)
            {
                Console.WriteLine(s.Key);
                foreach (var t in s.Value) 
                {
                    Console.WriteLine("     "+t.data.IpAddres);
                }
            }
        }
        //checks starting sequence of IP address and combines them into dictionary coresponding to their begining sequence//
        public static void CreateIndexArray(string stride,Node root) 
        {
            Node temp;
            temp = root;
            if (temp == null)
                return;
            CreateIndexArray(stride, temp.left);
            if (!Prefixes.ContainsKey(stride))
            {
                Prefixes.Add(stride, new List<Node>());
            }
            else
            {
                if (temp.data.IpAddres.StartsWith(stride))
                {
                    Prefixes[stride].Add(temp);
                } 
            }
            CreateIndexArray(stride, temp.right);
        }
        //loading IP addresses into the tree formation from json with Newtonsoft.json lib//
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
                        data.MacAddres = item["MacAddress"].ToString();
                        data.Prefix = item["Prefix"];
                        Node node = new Node(data);
                        if (tree.root == null)
                        {
                            Node iniRoot = tree.AddNode(data);
                            tree.InsertNode(tree.root, iniRoot);
                        }
                        else
                        {
                            tree.InsertNode(tree.root, tree.AddNode(data));
                        }
                    }

                    //tree.DisplayTree(tree.root);
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
            IpLookUp("192.168.10.4", 24, 4);            
        }
    }
}

