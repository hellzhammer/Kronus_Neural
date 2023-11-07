using Kronus_Neural.Activations;
using Kronus_Neural.Interfaces;
using Kronus_Neural.MultiLayerPerceptron;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kronus_Neural.Reccurent
{
    /*public class RNN : INeuralNetwork
    {
        public List<List<string>> hiddenLayers { get; private set; }
        public List<Neuron> output { get; private set; }
        public Dictionary<string, Neuron> hidden_neurons;
        public int[] recurring_layer { get; protected set; } // which layers repeat

        private int min_weight_init_val { get; set; }
        private int max_weight_init_val { get; set; }

        public RNN(int input_length,
            int output_nodes,
            int[] layer_node_counts,
            int[] recurring_layers,
            IActivation<double>[] hiddenActivations,
            IActivation<double> outputActivation,
            int weight_init_min,
            int weight_init_max,
            Initializer weightInit = Initializer.random)
        {
            this.hidden_neurons = new Dictionary<string, Neuron>();
            this.hiddenLayers = new List<List<string>>();
            this.max_weight_init_val = weight_init_max;
            this.min_weight_init_val = weight_init_min;
            this.recurring_layer = recurring_layers;
        }

        *//*public double[] Process(double[] inputs)
        {
            if (inputs == null || inputs.Length == 0)
            {
                throw new System.Exception("Data cannot be null or empty");
            }
            try
            {
                
            }
            catch (System.Exception)
            {
                throw;
            }
        }*//*
    }*/
}
