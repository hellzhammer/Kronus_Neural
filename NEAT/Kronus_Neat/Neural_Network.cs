using System;
using System.Collections.Generic;

namespace Kronus_Neural.NEAT
{
    public class NeatNetwork : Network
    {
        public string network_id { get; protected set; }
        public int network_score { get; set; }
        public double current_fitness { get; set; }
        public double current_error { get; set; }
        public double current_adjusted_fitness { get; set; }
        public string species_id { get; set; }

        public NeatNetwork()
        {
            this.network_id = "network - " + Guid.NewGuid().ToString();
            this.current_fitness = 0;
            this.network_score = 0;
            this.current_error = 0;
            this.current_adjusted_fitness = 0;
            this.species_id = string.Empty;
            this.Input_Neurons = new Dictionary<string, Neuron>();
            this.Hidden_Neurons = new Dictionary<string, Neuron>();
            this.Output_Neurons = new Dictionary<string, Neuron>();
            this.All_Connections = new Dictionary<string, Connection>();
        }

        public void set_net_id(string id)
        {
            this.network_id = id;
        }

        public NeatNetwork clone()
        {
            Dictionary<string, Neuron> inputs = new Dictionary<string, Neuron>();
            foreach (var item in Input_Neurons)
            {
                inputs.Add(item.Key, item.Value.clone());
            }

            Dictionary<string, Neuron> hidden = new Dictionary<string, Neuron>();
            foreach (var item in Hidden_Neurons)
            {
                hidden.Add(item.Key, item.Value.clone());
            }

            Dictionary<string, Neuron> outputs = new Dictionary<string, Neuron>();
            foreach (var item in Output_Neurons)
            {
                outputs.Add(item.Key, item.Value.clone());
            }

            Dictionary<string, Connection> conns = new Dictionary<string, Connection>();
            foreach (var item in All_Connections)
            {
                conns.Add(item.Key, item.Value.clone());
            }

            return new NeatNetwork() 
            { 
                species_id = this.species_id,
                current_fitness = this.current_fitness,
                current_error = this.current_error,
                network_score = this.network_score, 
                current_adjusted_fitness = this.current_adjusted_fitness,
                Input_Neurons = inputs,
                Output_Neurons = outputs, 
                Hidden_Neurons = hidden,
                All_Connections = conns
            };
        }
    }
}
