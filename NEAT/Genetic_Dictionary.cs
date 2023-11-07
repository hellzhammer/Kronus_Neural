using System.Collections.Generic;

namespace Kronus_Neural.NEAT
{
    public class Genetic_Dictionary
    {
        public Dictionary<string, int> Known_Connection_IDs { get; protected set; }

        public Dictionary<string, int> Known_Neuron_IDs { get; protected set; }

        public Genetic_Dictionary()
        {
            this.Known_Connection_IDs = new Dictionary<string, int>();
            this.Known_Neuron_IDs = new Dictionary<string, int>();
        }

        /// <summary>
        /// adds a connection to the dictionary. 
        /// 
        /// requires the current generation
        /// </summary>
        public void Add_Connection(string new_ID, int generation)
        {
            if (!this.Known_Connection_IDs.ContainsKey(new_ID))
            {
                this.Known_Connection_IDs.Add(new_ID, generation);
            }
        }

        /// <summary>
        /// adds a node to the dictionary.
        /// 
        /// requires the current generation
        /// </summary>
        public void Add_Node(string node_ID, int generation)
        {
            if (!this.Known_Neuron_IDs.ContainsKey(node_ID))
            {
                this.Known_Neuron_IDs.Add(node_ID, generation);
            }
        }

        public bool Connection_Exists(string connection_ID)
        {
            return this.Known_Connection_IDs.ContainsKey(connection_ID);
        }

        public bool Neuron_Exists(string node_ID)
        {
            return this.Known_Neuron_IDs.ContainsKey(node_ID);
        }
    }
}
