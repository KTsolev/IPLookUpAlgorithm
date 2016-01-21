using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpLookUpAlgorithm
{
    class BinaryTreeImp
    {
        public Node root;
     
        static int count = 0;
        
        public BinaryTreeImp()
        {
            root = null;
        }
        public Node AddNode(Data data)
        {
            Node newNode = new Node(data);

            if (root == null)
            {
                root = newNode;
            }
            count++;
            return newNode;
        }
        //Inserting new node into the tree based on IP address. IP addresses are compared and based on them is made desicions in which branch of the tree to be inserted(left or right)//
        public void InsertNode(Node root, Node newNode)
        {
            Node temp;
            temp = root;

            if (newNode.data.IpAddres.CompareTo(temp.data.IpAddres) < 0)
            {
                if (temp.left == null)
                {
                    temp.left = newNode;
                }
                else
                {
                    temp = temp.left;
                    InsertNode(temp, newNode);
                }
            }
            else if (newNode.data.IpAddres.CompareTo(temp.data.IpAddres) > 0)
            {
                if (temp.right == null)
                {
                    temp.right = newNode;
                }
                else
                {
                    temp = temp.right;
                    InsertNode(temp, newNode);
                }
            }
        }


        public void DisplayTree(Node root)
        {
            Node temp;
            temp = root;

            if (temp == null)
                return;
            DisplayTree(temp.left);
            System.Console.WriteLine(temp.data.IpAddres + " " + temp.data.MacAddres + " " + temp.data.Prefix);
            DisplayTree(temp.right);
        }
    }
}
