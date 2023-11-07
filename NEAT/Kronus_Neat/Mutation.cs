using Kronus_Neural.Activations;
using System;
using System.Collections.Generic;

namespace Kronus_Neural.NEAT
{
    public class Mutation
    {
        /// <summary>
        /// loops through connections and adjusts weights if random value is greater than thresh
        /// 
        /// chance for negative and positive adjustment
        /// </summary>
        public static NeatNetwork Adjust_Weights(NeatNetwork net, double chance, double mutationPower, int min, int max)
        {
            var r = new Random();
            foreach (var item in net.All_Connections)
            {
                // get a value to compare to mutation type
                double mutation_type_prob = r.NextDouble();
                // if type probability is less than 90% (0.90), perform adjustment 
                if (mutation_type_prob >= chance && mutation_type_prob <= 0.90)
                {
                    // probability of getting a positive adjustment vs a negative adjustment
                    double adjustment_type_prob = r.NextDouble();
                    // a random double is then generated
                    double rn = r.NextDouble();
                    // generated double multiplied with mutation power to get the adjustment value
                    double adjustment = rn * mutationPower;
                    if (adjustment_type_prob > 0.50)
                    {
                        // apply positive adjustment
                        item.Value.Weight += adjustment;
                    }
                    else
                    {
                        // apply a negative adjustment
                        item.Value.Weight -= adjustment;
                    }
                }
                // if the type probability value is higher than 90% (0.90), completely reset the weight value
                else if (mutation_type_prob >= 0.90) 
                {
                    item.Value.Weight = ((double)r.Next(-min, max) * r.NextDouble()) * (1.0 - -1.0) + -1.0;
                }
            }

            return net;
        }

        /// <summary>
        /// adds a new node
        /// </summary>
        /// <param name="net"></param>
        /// <param name="chance"></param>
        /// <param name="nodeMax"></param>
        /// <param name="hidden_act"></param>
        /// <param name="weight_init_min"></param>
        /// <param name="weight_init_max"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static NeatNetwork Add_Node(NeatNetwork net, double chance, int nodeMax, IActivation<double> hidden_act, int weight_init_min, int weight_init_max)
        {
            if (net.Hidden_Neurons.Count == nodeMax)
            {
                // if this block is entered: cannot run due to no neurons
                return net; 
            }

            // selects a random connection that is split to add new neurons
            Random r = new Random();
            string selected_connection_id = string.Empty;
            foreach (var _connection in net.All_Connections)
            {

                // for now to not use recurring connections for splitting.
                // need to work this out before actually doing something like this.
                if (r.NextDouble() >= chance && !_connection.Value.is_recurrent) // ********* CHANGE LATER, ALGO SHOULD BE ABLE TO SPLIT RECURRING NODES
                {
                    selected_connection_id = _connection.Key;
                    break;
                }
            }

            // if a connection was selected.
            if (selected_connection_id != String.Empty)
            {
                // find the next node id to use.
                // finds the first unused numeral
                string next_node = String.Empty;
                for (int i = net.Input_Neurons.Count + net.Output_Neurons.Count; i < nodeMax; i++)
                {
                    if (!net.Hidden_Neurons.ContainsKey(i.ToString()))
                    {
                        next_node = i.ToString();
                        break;
                    }
                }

                // create new neuron with the new id created above
                Neuron new_n = new Neuron(false, next_node, hidden_act);
                
                // create a new connection  between old start node and new created node
                Connection conn1 = new Connection(
                    net.All_Connections[selected_connection_id].Input + "-" + next_node, weight_init_min, 
                    weight_init_max, 
                    net.All_Connections[selected_connection_id].Input, 
                    next_node) { /*need to add is_reccurent*/ };

                // create connection from new node to old end node
                Connection conn2 = new Connection(
                    next_node + "-" + net.All_Connections[selected_connection_id].Output, 
                    weight_init_min, 
                    weight_init_max, 
                    next_node, 
                    net.All_Connections[selected_connection_id].Output) { /*need to add is_reccurent*/ };
                
                // just incase
                if (conn1.Input == conn1.Output || conn2.Input == conn2.Output)
                {
                    throw new Exception("New bug! --> Not sure what is causing this. Function: Add_Node() --> Mutation Class. Cannot have same input node as output node.");
                }

                // if the network does not already contain these connections, add them to existing network
                if (!net.All_Connections.ContainsKey(conn1.gene_id) && !net.All_Connections.ContainsKey(conn2.gene_id))
                {
                    net.All_Connections.Remove(selected_connection_id);
                    net.All_Connections.Add(conn1.gene_id, conn1);
                    net.All_Connections.Add(conn2.gene_id, conn2);
                    net.Hidden_Neurons.Add(next_node, new_n);
                }
            }

            return net;
        }

        /// <summary>
        /// removes a random node
        /// </summary>
        /// <param name="net"></param>
        /// <param name="chance"></param>
        /// <returns></returns>
        public static NeatNetwork Remove_Node(NeatNetwork net, double chance)
        {
            // select a random node where random value is above a given thresh
            Random r = new Random();
            string id = string.Empty;
            foreach (var neuron in net.Hidden_Neurons)
            {

                if (r.NextDouble() >= chance)
                {
                    id = neuron.Key;
                    break;
                }
            }

            // if a node is selected
            if (id != String.Empty)
            {
                // find all connections associated with this node and remove those connections

                // create list of connections
                List<string> ids_to_purge = new List<string>();
                foreach (var conn in net.All_Connections)
                {
                    if (conn.Value.Input == id || conn.Value.Output == id)
                    {
                        ids_to_purge.Add(conn.Key);
                    }
                }

                // remove listed connections
                for (int i = 0; i < ids_to_purge.Count; i++)
                {
                    net.All_Connections.Remove(ids_to_purge[i]);
                }

                net.Hidden_Neurons.Remove(id);
            }

            return net;
        }

        /// <summary>
        /// creates a non recurrent connection
        /// </summary>
        public static NeatNetwork Add_Connection(NeatNetwork net, double chance, int weight_init_min, int weight_init_max)
        {
            // randomly selected 2 nodes to create a new connection.
            // connection 2 must be downstream from node 1
            Random r = new Random();
            string id = string.Empty;
            string id2 = string.Empty;
            // always get structure here as network could have mutated and structure not be updated
            var struc = net.Get_Structure();
            int last_index = 0;
            for (int x = 0; x < struc.Count; x++)
            {
                for (int y = 0; y < struc[x].Count; y++)
                {
                    if (r.NextDouble() >= chance && id == String.Empty)
                    {
                        id = struc[x][y];
                        break;
                    }
                }
                if (id != String.Empty)
                {
                    last_index = x;
                    break;
                }
            }

            // non recurrent
            for (int x = last_index + 1; x < struc.Count; x++)
            {
                for (int y = 0; y < struc[x].Count; y++)
                {
                    if (r.NextDouble() >= chance && id2 == String.Empty)
                    {
                        id2 = struc[x][y];
                        break;
                    }
                }
                if (id2 != String.Empty)
                {
                    break;
                }
            }

            if (id != String.Empty && id2 != String.Empty && id != id2)
            {
                Connection conn = new Connection(id + "-" + id2, weight_init_min, weight_init_max, id, id2) { is_recurrent = false };
                if (conn.Input == conn.Output)
                {
                    throw new Exception("New bug! --> Not sure what is causing this. Function: Add_Connection() --> Mutation Class. Cannot have same input node as output node.");
                }
                if (!net.All_Connections.ContainsKey(conn.gene_id))
                {
                    net.All_Connections.Add(conn.gene_id, conn);
                }
            }
            return net;
        }

        /// <summary>
        /// adds a new recurrent connection
        /// </summary>
        /// <param name="net"></param>
        /// <param name="chance"></param>
        /// <param name="weight_init_min"></param>
        /// <param name="weight_init_max"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static NeatNetwork Add_Recurrent_Connection(NeatNetwork net, double chance, int weight_init_min, int weight_init_max)
        {
            // finds 2 randomly selected nodes to create a connection
            // node 2 must be upstream from node 1
            Random r = new Random();
            string id = string.Empty;
            string id2 = string.Empty;
            var struc = net.Get_Structure();
            int last_index = 0;

            // cannot start at 0 because there are no reverse connections that can be made.
            for (int x = 0; x < struc.Count; x++)
            {
                for (int y = 1; y < struc[x].Count; y++)
                {
                    if (r.NextDouble() >= chance && id == String.Empty)
                    {
                        id = struc[x][y];
                        break;
                    }
                }
                if (id != String.Empty)
                {
                    last_index = x;
                    break;
                }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                // recurrent
                for (int x = 0; x < last_index - 1; x++)
                {
                    for (int y = 0; y < struc[x].Count; y++)
                    {
                        if (r.NextDouble() >= chance && id2 == String.Empty)
                        {
                            id2 = struc[x][y];
                            break;
                        }
                    }
                    if (id2 != String.Empty)
                    {
                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(id2))
                {
                    Connection conn = new Connection(id + "-" + id2, weight_init_min, weight_init_max, id, id2) { is_recurrent = true };
                    if (conn.Input == conn.Output)
                    {
                        throw new Exception("New bug! --> Not sure what is causing this. Function: Add_Connection() --> Mutation Class. Cannot have same input node as output node.");
                    }
                    if (!net.All_Connections.ContainsKey(conn.gene_id))
                    {
                        net.All_Connections.Add(conn.gene_id, conn);
                    }
                }
            }
            return net;
        }

        /// <summary>
        /// removes a random connection
        /// </summary>
        public static NeatNetwork Remove_Connection(NeatNetwork net, double chance)
        {
            // finds a random connection to remove
            Random r = new Random();
            string id = string.Empty;
            foreach (var conn in net.All_Connections)
            {

                if (r.NextDouble() >= chance)
                {
                    id = conn.Key;
                    break;
                }
            }
            net.All_Connections.Remove(id);
            return net;
        }

        /// <summary>
        /// mutates the activation function of a neat network
        /// </summary>
        /// <param name="net"></param>
        /// <param name="chance"></param>
        /// <param name="allowed_activations"></param>
        /// <returns></returns>
        public static NeatNetwork Alter_Function(NeatNetwork net, double chance, List<string> allowed_activations)
        {
            // randomly alters the activation function of a neuron
            Random r = new Random();
            string neuron_id = string.Empty;
            foreach (var neuron in net.Hidden_Neurons)
            {
                if (r.NextDouble() >= chance)
                {
                    neuron_id = neuron.Key;
                    break;
                }
            }

            int act_id = r.Next(0, allowed_activations.Count - 1);

            if (string.IsNullOrEmpty(allowed_activations[act_id]))
                net.Hidden_Neurons[neuron_id].activation_func = Parse_Activation_Type(allowed_activations[act_id]);

            return net;
        }

        /// <summary>
        /// simple parse function for activation type
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static IActivation<double> Parse_Activation_Type(string _type)
        {
            if (_type == "Sigmoid")
            {
                return new Sigmoid();
            }
            else if (_type == "TanH")
            {
                return new TanH();
            }
            else if (_type == "Gaussian")
            {
                return new Gaussian();
            }
            else if (_type == "ReLU")
            {
                return new ReLU();
            }
            else if (_type == "LeakyReLU")
            {
                return new LeakyReLU();
            }
            else if (_type == "Swish")
            {
                return new Swish();
            }
            else if (_type == "SoftPlus")
            {
                return new SoftPlus();
            }

            throw new Exception("_type does not appear to exist. Please ensure input is correct. This method of mutating genes is going to change. Just a test.");
        }
    }
}
