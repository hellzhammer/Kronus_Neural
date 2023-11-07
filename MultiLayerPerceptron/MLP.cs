using Kronus_Neural.Activations;
using Kronus_Neural.Interfaces;
using Kronus_Neural.Loss_Functions;
using System.Collections.Generic;

namespace Kronus_Neural.MultiLayerPerceptron 
{   
    public class MLP : INeuralNetwork
    {
        public Neuron[][] hiddenLayers { get; private set; }
        public Neuron[] output { get; private set; }

        private int min_weight_init_val { get; set; }
        private int max_weight_init_val { get; set; }

        /// <summary>
        /// create a new neural network 
        /// </summary>
        /// <param name="input_length"></param>
        /// <param name="output_nodes"></param>
        /// <param name="layer_node_counts"></param>
        /// <param name="hiddenActivations"></param>
        /// <param name="outputActivation"></param>
        /// <param name="weightInit"></param>
        public MLP(
            int input_length,
            int output_nodes,
            int[] layer_node_counts,
            IActivation<double>[] hiddenActivations,
            IActivation<double> outputActivation,
            int weight_init_min,
            int weight_init_max,
            Initializer weightInit = Initializer.random
            )
        {
            this.hiddenLayers = new Neuron[hiddenActivations.Length][];
            this.max_weight_init_val = weight_init_max;
            this.min_weight_init_val = weight_init_min;

            if (this.hiddenLayers.Length > 0)
            {
                // set up the first hidden layer
                this.hiddenLayers[0] = new Neuron[layer_node_counts[0]];
                for (int x = 0; x < this.hiddenLayers[0].Length; x++)
                {
                    this.hiddenLayers[0][x] = new Neuron(input_length, hiddenActivations[0], weightInit, weight_init_min, weight_init_max);
                }

                // now handle the other layers in hidden
                for (int i = 1; i < hiddenActivations.Length; i++)
                {
                    Neuron[] n = new Neuron[layer_node_counts[i]];
                    for (int x = 0; x < n.Length; x++)
                    {
                        n[x] = new Neuron(this.hiddenLayers[i - 1].Length, hiddenActivations[i], weightInit, weight_init_min, weight_init_max);
                    }
                    this.hiddenLayers[i] = n;
                }
            }

            this.output = new Neuron[output_nodes];
            for (int i = 0; i < this.output.Length; i++)
            {
                int input_count = input_length;

                if (this.hiddenLayers.Length > 0)
                    input_count = this.hiddenLayers[this.hiddenLayers.Length - 1].Length;

                this.output[i] = new Neuron(input_count, outputActivation, weightInit, weight_init_min, weight_init_max);
            }
        }

        /// <summary>
        /// recreate a model from a saved neural network
        /// </summary>
        /// <param name="saved_network"></param>
        public MLP(MLP_Model saved_network)
        {
            List<Neuron[]> hiddens = new List<Neuron[]>();
            for (int x = 0; x < saved_network.hidden_layers.Length; x++)
            {
                List<Neuron> h_neurons = new List<Neuron>();
                for (int y = 0; y < saved_network.hidden_layers[x].Length; y++)
                {
                    h_neurons.Add(new Neuron(saved_network.hidden_layers[x][y]));
                }
                hiddens.Add(h_neurons.ToArray());
            }

            List<Neuron> outs = new List<Neuron>();
            for (int o = 0; o < saved_network.output.Length; o++)
            {
                outs.Add(new Neuron(saved_network.output[o]));
            }

            this.hiddenLayers = hiddens.ToArray();
            this.output = outs.ToArray();
        }

        public MLP_Model save_network()
        {
            try
            {
                List<Neuron_Model[]> hiddens = new List<Neuron_Model[]>();
                for (int x = 0; x < this.hiddenLayers.Length; x++)
                {
                    List<Neuron_Model> neurons = new List<Neuron_Model>();
                    for (int y = 0; y < this.hiddenLayers[x].Length; y++)
                    {
                        neurons.Add(this.hiddenLayers[x][y].save_node());
                    }
                    hiddens.Add(neurons.ToArray());
                }

                List<Neuron_Model> outs = new List<Neuron_Model>();
                for (int x = 0; x < this.output.Length; x++)
                {
                    outs.Add(this.output[x].save_node());
                }

                return new MLP_Model()
                {
                    hidden_layers = hiddens.ToArray(),
                    output = outs.ToArray()
                };
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public double[] GetNetworkOutputs()
        {
            double[] d_outs = new double[this.output.Length];

            for (int d = 0; d < d_outs.Length; d++)
            {
                d_outs[d] = output[d].Node_Activate(false);
            }

            return d_outs;
        }

        public double[] Process(double[] inputs)
        {
            if (inputs == null || inputs.Length == 0)
            {
                throw new System.Exception("Data cannot be null or empty");
            }
            try
            {
                for (int j = 0; j < this.hiddenLayers.Length; j++)
                {
                    var nOuts = new double[this.hiddenLayers[j].Length];
                    for (int n = 0; n < this.hiddenLayers[j].Length; n++)
                    {
                        this.hiddenLayers[j][n].node_last_inputs = inputs;
                        nOuts[n] = this.hiddenLayers[j][n].Node_Activate(false);
                    }
                    inputs = nOuts;
                }

                //input the last layers outs
                for (int i = 0; i < this.output.Length; i++)
                {
                    // if the output node activation == softmax, 
                    // we want to perform the exponent functions and sum.
                    // then apply the softmax. 

                    // else not == softmax, continue on as is. 
                    this.output[i].node_last_inputs = inputs;
                }

                double[] d_outs = new double[this.output.Length];

                for (int d = 0; d < d_outs.Length; d++)
                {
                    d_outs[d] = output[d].Node_Activate(false);
                }

                return d_outs;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void Learn(double learnrate, int iterations, double[][] TrainingInput, double[][] ExpectedOutput, Loss_Function loss, bool adjust_bias = true)
        {
            if (TrainingInput == null || TrainingInput.Length == 0)
            {
                throw new System.Exception("data cannot be null or empty");
            }
            if (TrainingInput.Length != ExpectedOutput.Length)
            {
                throw new System.Exception("Matrices incompatible");
            }
            try
            {
                int epoch = 0;

            Retry: // instance of repeater. cleaner than a another loop


                for (int i = 0; i < TrainingInput.Length; i++)  // iterate through inputs
                {
                    this.Process(TrainingInput[i]);
                    this.output = loss.Invoke(ExpectedOutput[i], learnrate, this.output);                    

                    // ***** BACK PROP ********** //
                    for (int o = 0; o < this.output.Length; o++)
                    {
                        // back prop through hidden layers
                        // x
                        for (int x = this.hiddenLayers.Length - 1; x > -1; x--)
                        {
                            // y
                            for (int y = 0; y < this.hiddenLayers[x].Length; y++)
                            {
                                // iterate through the output weights to get error
                                // and adjust hidden layer as needed.
                                for (int on = 0; on < output[o].node_weights.Length; on++)
                                {
                                    // calc hidden error.
                                    this.hiddenLayers[x][y].error =
                                        this.hiddenLayers[x][y].node_activation.Activate(this.hiddenLayers[x][y].Node_Activate(false), true)
                                        *
                                        output[o].error
                                        *
                                        output[o].node_weights[on];

                                    // adjust hidden node weights
                                    this.hiddenLayers[x][y].AdjustWeights(learnrate, true);
                                }
                            }
                        }
                    }
                }

                if (epoch != iterations)
                {
                    epoch++;
                    goto Retry;
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}