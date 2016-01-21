from node import Node, RootNode
from math import pow

#SWITCH LIST
IP     = 0
SUBNET = 1
MAC    = 2
NAME   = 3
DPID   = 4


"""
" BitmapTree
" 
" description: 
"""
class BitmapTree:

    """
    " __init__()
    " 
    " input: @self, @switch_list, @stride
    " output:
    " changes: @self.switch_list, @self.stride, @self.switches_network, self.root_node
    "
    " description: Constructor of the class, calls method to build the tree
    """
    def __init__(self, switch_list, stride):
        self.switch_list = switch_list
        self.stride = stride
        self.switch_list_to_switches_network()
        self.build_tree()

    """
    " switch_list_to_switches_network()
    " 
    " input: @self
    " output:
    " changes: @self.switches_network
    "
    " description: Converts the ips from the switch_list to binary. Then applies the network mask to the ips to get the Prefix Table.
    """
    def switch_list_to_switches_network(self):
        print("-- Creating Switches Network Array (Prefix Table)")
        self.switches_network = {}
        i = 0
        for switch in self.switch_list:
            ip_network = "".join([bin(int(x)+256)[3:] for x in switch.split('.')])
            ip_network = ip_network[:int(self.switch_list[switch][SUBNET])]
            self.switches_network[ip_network] = switch
            print("--- ["+ self.switches_network[ip_network] +"] : "+ip_network)
        print("-- Finished Switches Network Array\n")

    """
    " switch_list_to_switches_network()
    " 
    " input: @self
    " output:
    " changes: @self.root_node
    "
    " description: Creates the bitmap tree. Starts by creating the root_node and then enters a recursive algorithm to build the complete tree
    """
    def build_tree(self):
        print("-- Building Bitmap Tree (stride = "+ str(self.stride) +") with "+ str(len(self.switch_list)) +" Switches")
        print("--- Creating Root Node")
        self.root_node = RootNode()
        self.create_node(self.root_node,"")
        print("-- Bitmap Tree Built with Success")


    """
    " create_node()
    " 
    " input: @self, @node, @ip
    " output:
    " changes: @self.root_node
    "
    " description: Creates the node, while creating children the method becomes recursive
    "   Before checking for internal results and children the idxs are set to the next position availabel in each array
    """
    def create_node(self, node, ip):
        print("--- Building Node")
        node.update_idx()
        self.check_for_internal_results(node, ip)
        self.check_for_children(node, ip)

    """
    " check_for_internal_results()
    " 
    " input: @self, @node, @ip
    " output:
    " changes: @self.root_node
    "
    " description: Looks in the @self.switches_network for a match with the ips represented by the @node.
    "   If there is a match it means that the this node is representing that prefix and so it is added to the internal bitmap of that node and to the list of results.
    """
    def check_for_internal_results(self, node, ip):
        print("---- Building Internal Bitmap")
        max_ip = bin(int(pow(2, self.stride))-1)[2:]
        actual_idx = 0
        actual_ip = ""
        while 1:
            complete_ip = ip+actual_ip
            if (complete_ip in self.switches_network):
                print("----- Adding Result")
                result = self.switches_network[ip+actual_ip]
                del self.switches_network[ip+actual_ip]
                print("------ ["+ str(actual_idx) +"] : "+ result)
                node.add_result(actual_idx, result)
            if actual_ip == max_ip:
                break
            else:
                actual_ip = self.next_ip(actual_ip)
                actual_idx +=1
        print("---- Finish Internal Bitmap")

    #WARNING: NOT SAFE FOR STRIDES != 2
    """
    " check_for_children()
    " 
    " input: @self, @node, @ip
    " output:
    " changes: @self.root_node
    "
    " description: Goes trough all the ips possible for children of this node and calls a method to check if there is in fact a child for that ip, creating an object if there is.
    "   Loops the list of children created and creates the node(fills the object previously created) for each one (in this fase the function becames recursive - Each child will create all it's children before returning )
    """
    def check_for_children(self, node, ip):
        print("---- Building Child Bitmap")
        i = "000"
        position = 0
        children_nodes = []
        children_ip = {}
        child_node = None
        while True:
            complete_ip = ip + i
            child_node = self.check_for_child(node,complete_ip, position)
            if child_node:
                children_nodes.append(child_node)
                children_ip[children_nodes[-1]] = complete_ip
                child_node = None
            if i == "111":
                break
            i= self.next_ip(i)
            position +=1
        print("---- Finish Child Bitmap")
        for child_node in children_nodes:
            self.create_node(child_node, children_ip[child_node])

    """
    " check_for_child()
    " 
    " input: @self, @node, @ip, @position
    " output:
    " changes: @self.root_node
    "
    " description: Looks in the @self.switches_network for a partial match with the ips represented by the @node.
    "   If there is a partial match it means that this node is a father of the node representing that prefix. A node is created and the algorithm continues in the new node (recursive)
    """
    def check_for_child(self,node,ip, position):
        for network in self.switches_network:
            if network.startswith(ip):
                print("----- Adding Child")
                print("------ ["+ str(position) +"] : "+ ip)
                child_node = node.add_child (position)
                return child_node
        
    """
    " next_ip()
    " 
    " input: @self, @actual_ip
    " output: @next_ip
    " changes:
    "
    " description: Small method to get the next ip to go to trough the tree in the bitmap style (like a snake).
    "   If there is no change in the number of bits represented it works as a simple sum of 1.
    """          
    def next_ip(self, actual_ip):
        if (actual_ip == ""):
            return "0"
        last_idx = len(actual_ip) - 1
        last_char = actual_ip[last_idx]
        if (last_char == "0"):
            return actual_ip[:last_idx] + "1"
        elif (last_char == "1"):
            for i in range (last_idx - 1, -1, -1):
                if(actual_ip[i] == "0"):
                    return actual_ip[:i] + "1" + "".join("0" for x in range (i, last_idx))
            return "".join("0" for x in range (last_idx + 2))

    """
    " print_tree()
    " 
    " input: @self
    " output: 
    " changes:
    "
    " description: Prints the tree
    """    
    def print_tree(self):
        print("\n\nPRINTING TREE\n--ROOT NODE")
        self.root_node.print_node()
        i=0
        for node in self.root_node.children:
            print("\n--NODE "+str(i))
            node.print_node()
            i+=1
        print("\n\n--RESULTS")
        i=0
        for result in self.root_node.results:
            print("["+str(i)+"] "+result)
            i+=1  
        
        
#Example used in the first meating
#switch_list = {}
#switch_list["195.0.0.254"  ] = ["195.0.0.254","8" ,"00:00:00:11:11:01","s1","1"] 
#switch_list["128.128.0.254"] = ["128.128.0.254","8","00:00:00:11:11:02","s2","2"] 
#switch_list["154.128.0.254"] = ["154.128.0.254","8","00:00:00:11:11:03","s3","3"] 
#switch_list["197.160.0.254"] = ["197.160.0.254","8","00:00:00:11:11:04","s4","4"]

#Example used in the slides
switch_list = {}
switch_list["0.0.0.254"  ] = ["0.0.0.254","0" ,"00:00:00:11:11:01","s1","1"] 
switch_list["128.128.0.254"] = ["128.128.0.254","1","00:00:00:11:11:02","s2","2"] 
switch_list["0.128.0.254"] = ["0.128.0.254","2","00:00:00:11:11:03","s3","3"] 
switch_list["160.160.0.254"] = ["160.160.0.254","3","00:00:00:11:11:04","s4","4"]
switch_list["224.0.0.254"  ] = ["224.0.0.254","3" ,"00:00:00:11:11:01","s1","1"] 
switch_list["128.128.0.253"] = ["128.128.0.254","4","00:00:00:11:11:02","s2","2"] 
switch_list["232.128.0.254"] = ["232.128.0.254","5","00:00:00:11:11:03","s3","3"] 
switch_list["228.160.0.254"] = ["228.160.0.254","6","00:00:00:11:11:04","s4","4"]
switch_list["134.160.0.254"] = ["134.160.0.254","7","00:00:00:11:11:04","s4","4"]


tree = BitmapTree(switch_list, 2)
tree.print_tree()
