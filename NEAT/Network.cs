using Kronus_Neural.Interfaces;
using System.Collections.Generic;

namespace Kronus_Neural.NEAT
{
    public class Network : INeuralNetwork
    {
        public Dictionary<string, Connection> All_Connections { get; set; }
        public Dictionary<string, Neuron> Input_Neurons { get; set; }
        public Dictionary<string, Neuron> Hidden_Neurons { get; set; }
        public Dictionary<string, Neuron> Output_Neurons { get; set; }

        public List<List<string>> structure { get; set; }

        public void Zero_Network()
        {
            foreach (var n in Input_Neurons)
            {
                n.Value.Input = 0;
            }

            if (Hidden_Neurons.Count > 0)
            {
                foreach (var n in Hidden_Neurons)
                {
                    n.Value.Input = 0;
                }
            }

            foreach (var n in Output_Neurons)
            {
                n.Value.Input = 0;
            }
        }

        #region to remove
        /*public double[] Feed_Forward(double[] input)
        {
            this.Zero_Network();

            if (this.structure == null)
                structure = this.Get_Structure();
            
            Dictionary<string, Neuron> All_Nodes = new Dictionary<string, Neuron>(); // build a list of all nodes
            int index = 0;
            foreach (var item in Input_Neurons)
            {
                item.Value.Input = input[index];
                All_Nodes.Add(item.Key, item.Value);
                index++;
            }
            foreach (var item in Hidden_Neurons)
            {
                All_Nodes.Add(item.Key, item.Value);
            }
            foreach (var item in Output_Neurons)
            {
                All_Nodes.Add(item.Key, item.Value);
            }

            // FEED FORWARD
            // start at one as input values are entered already
            for (int x = 1; x < structure.Count; x++)
            {
                for (int y = 0; y < structure[x].Count; y++)
                {
                    foreach (var connection in All_Connections)
                    {
                        if (structure[x][y] == connection.Value.Output) // && !connection.Value.is_recurrent -- removed until i can readd recurrent net
                        {
                            double activation_output = All_Nodes[connection.Value.Input].Activate();
                            All_Nodes[connection.Value.Output].Input += activation_output * connection.Value.Weight;
                        }
                    }
                }
            }

            // ensure all nodes are equal. JUST INCASE
            // I might need the inputs before this is all done
            foreach (var item in All_Nodes)
            {
                if (Input_Neurons.ContainsKey(item.Key))
                {
                    Input_Neurons[item.Key] = item.Value.clone();
                }
                else if (Hidden_Neurons.ContainsKey(item.Key))
                {
                    Hidden_Neurons[item.Key] = item.Value.clone();
                }
                else if (Output_Neurons.ContainsKey(item.Key))
                {
                    Output_Neurons[item.Key] = item.Value.clone();
                }
            }

            double[] outs = new double[Output_Neurons.Count];
            index = 0;
            foreach (var item in Output_Neurons)
            {
                outs[index] = item.Value.Activate();
                index++;
            }

            return outs;
        }*/
        #endregion

        public int get_node_structure_position(string neuron_id)
        {
            if (this.structure == null)
                this.structure = this.Get_Structure();

            if (string.IsNullOrWhiteSpace(neuron_id))
                throw new System.Exception("neuron_id cannot be null!");

            int rtn = -1;
            for (int i = 0; i < structure.Count; i++)
            {
                if (structure[i].Contains(neuron_id))
                {
                    rtn = i;
                    break;
                }
            }

            if (rtn == -1)
                throw new System.Exception("Could not find a matching node with the id: " + neuron_id);

            return rtn;
        }

        private List<List<string>> recurring_path_finder(string from_neuron, int end_index)
        {
            List<List<string>> _structure = new List<List<string>>();
            List<string> neuron_ids = new List<string>();
            neuron_ids.Add(from_neuron);

            // only add if list is greater than 0
            if (neuron_ids.Count > 0)
                _structure.Add(neuron_ids);
           

            List<string> vistited = new List<string>();
            // get hidden neurons and add to structure if connections are present.
            for (int s = 0; s < _structure.Count; s++)
            {
                neuron_ids = new List<string>();
                for (int i = 0; i < _structure[s].Count; i++)
                {
                    foreach (var connection in All_Connections)
                    {
                        // if id matches..
                        if (!connection.Value.is_recurrent && connection.Value.Input == _structure[s][i])
                        {
                            // if id not already in array..
                            if (!neuron_ids.Contains(connection.Value.Output))
                            {
                                neuron_ids.Add(connection.Value.Output);
                            }
                        }
                    }
                }

                // as long as items > 0, add aray to array of arrays
                if (neuron_ids.Count > 0)
                {
                    // insert at 0 as algo is workin backwards
                    _structure.Add(neuron_ids);
                }

                if (s == end_index)
                {
                    break;
                }
            }

            return _structure;

        }

        /// <summary>
        /// This funciton loops through the connections dictionary in order to build a working neural network structure
        /// </summary>
        public List<List<string>> Get_Structure()
        {
            if (All_Connections == null || Input_Neurons == null || Hidden_Neurons == null || Output_Neurons == null)
                throw new System.Exception("Neuron Dictionaries cannot be null! Network.Get_Structure()");
            

            List<List<string>> structure = new List<List<string>>();
            List<string> neuron_ids = new List<string>();

            foreach (var neuron in Output_Neurons)
            {
                foreach (var connection in All_Connections)
                {
                    if (connection.Value.Output == neuron.Key)
                    {
                        // if id not already in array
                        if (!neuron_ids.Contains(connection.Value.Input))
                        {
                            // add id to array
                            if (!Input_Neurons.ContainsKey(connection.Value.Input))
                            {
                                neuron_ids.Add(connection.Value.Input);
                            }
                        }
                    }
                }
            }

            if (neuron_ids.Count > 0)
            {
                structure.Insert(0, neuron_ids);
            }

            for (int i = structure.Count - 1; i > -1; i--)
            {
                neuron_ids = new List<string>();
                for (int j = 0; j < structure[i].Count; j++)
                {
                    foreach (var connection in All_Connections)
                    {
                        // if id matches..
                        if (connection.Value.Output == structure[i][j])
                        {
                            // if id not already in array..
                            if (!neuron_ids.Contains(connection.Value.Input))
                            {
                                // if not an input node..
                                if (!Input_Neurons.ContainsKey(connection.Value.Input))
                                {
                                    // add id to array..
                                    neuron_ids.Add(connection.Value.Input);
                                }
                            }
                        }
                    }
                }
                // as long as items > 0, add aray to array of arrays
                if (neuron_ids.Count > 0)
                {
                    // insert at 0 as algo is workin backwards
                    structure.Insert(0, neuron_ids);
                }
            }

            neuron_ids = new List<string>();
            foreach (var item in Output_Neurons)
            {
                neuron_ids.Add(item.Key);
            }

            structure.Add(neuron_ids);

            // this adds in input nodes last
            neuron_ids = new List<string>();
            foreach (var item in Input_Neurons)
            {
                neuron_ids.Add(item.Value.gene_id);
            }
            structure.Insert(0, neuron_ids);

            return structure;
        }

        public double[] Process(double[] input)
        {
            this.Zero_Network();

            if (this.structure == null)
                structure = this.Get_Structure();
            
            Dictionary<string, Neuron> All_Nodes = new Dictionary<string, Neuron>(); // build a list of all nodes
            int index = 0;
            foreach (var item in Input_Neurons)
            {
                item.Value.Input = input[index];
                All_Nodes.Add(item.Key, item.Value);
                index++;
            }
            foreach (var item in Hidden_Neurons)
            {
                All_Nodes.Add(item.Key, item.Value);
            }
            foreach (var item in Output_Neurons)
            {
                All_Nodes.Add(item.Key, item.Value);
            }

            // FEED FORWARD
            // start at one as input values are entered already
            for (int x = 1; x < structure.Count; x++)
            {
                for (int y = 0; y < structure[x].Count; y++)
                {
                    foreach (var connection in All_Connections)
                    {
                        if (!connection.Value.is_recurrent)
                        {
                            if (structure[x][y] == connection.Value.Output) // && !connection.Value.is_recurrent -- removed until i can readd recurrent net
                            {
                                double activation_output = All_Nodes[connection.Value.Input].Activate();
                                All_Nodes[connection.Value.Output].Input += activation_output * connection.Value.Weight;
                            }
                        }
                        else if (connection.Value.is_recurrent)
                        {
                            if (structure[x][y] == connection.Value.Input)
                            {
                                double a_output = All_Nodes[connection.Value.Input].Activate();
                                All_Nodes[connection.Value.Output].Input += a_output * connection.Value.Weight;

                                var mini_struct = this.recurring_path_finder(connection.Value.Output, x);
                                for (int mx = 0; mx < mini_struct.Count; mx++)
                                {
                                    for (int my = 0; my < mini_struct[mx].Count; my++)
                                    {
                                        foreach (var cnn in All_Connections)
                                        {
                                            if (!cnn.Value.is_recurrent)
                                            {
                                                if (structure[x][y] == cnn.Value.Output) 
                                                {
                                                    double activation_output = All_Nodes[cnn.Value.Input].Activate();
                                                    All_Nodes[cnn.Value.Output].Input += activation_output * cnn.Value.Weight;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // here we want to create the ministructure and then fun a feed forward loop on that.
                        }
                    }
                }
            }

            // ensure all nodes are equal. JUST INCASE
            // I might need the inputs before this is all done
            foreach (var item in All_Nodes)
            {
                if (Input_Neurons.ContainsKey(item.Key))
                {
                    Input_Neurons[item.Key] = item.Value.clone();
                }
                else if (Hidden_Neurons.ContainsKey(item.Key))
                {
                    Hidden_Neurons[item.Key] = item.Value.clone();
                }
                else if (Output_Neurons.ContainsKey(item.Key))
                {
                    Output_Neurons[item.Key] = item.Value.clone();
                }
            }

            double[] outs = new double[Output_Neurons.Count];
            index = 0;
            foreach (var item in Output_Neurons)
            {
                outs[index] = item.Value.Activate();
                index++;
            }

            return outs;
        }
    }
}
