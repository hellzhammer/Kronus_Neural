using Kronus_Neural.Activations;
using System;

namespace Kronus_Neural.MultiLayerPerceptron {

    public partial class Neuron : INeuron<double[]>
    {
        public IActivation<double> node_activation { get; set; }
        public double[] node_last_inputs { get; set; }
        public double[] node_weights { get; set; }
        public double error { get; set; }

        public double node_bias { get; set; }

        public Neuron(Neuron_Model saved_neuron)
        {
            this.node_weights = saved_neuron.weights;
            this.node_bias = saved_neuron.bias;

            switch (saved_neuron.activationMethod)
            {
                case "Sigmoid":
                    node_activation = new Sigmoid();
                    break;
                case "TanH":
                    node_activation = new TanH();
                    break;
                case "SoftPlus":
                    node_activation = new SoftPlus();
                    break;
                case "Gaussian":
                    node_activation = new Gaussian();
                    break;
                case "ReLU":
                    node_activation = new ReLU();
                    break;
                case "LeakyReLU":
                    node_activation = new LeakyReLU();
                    break;
                case "Swish":
                    node_activation = new Swish();
                    break;
            }
        }

        /// <summary>
        /// user select init
        /// </summary>
        /// <param name="weightCount"></param>
        /// <param name="activation"></param>
        /// <param name="weightInit"></param>
        public Neuron(int weightCount, IActivation<double> activation, Initializer weightInit, int weight_init_min, int weight_init_max)
        {
            this.node_activation = activation;
            this.node_weights = new double[weightCount];
            this.node_last_inputs = new double[weightCount];

            switch (weightInit)
            {
                case Initializer.ones:
                    initWeights_Value(1);
                    break;
                case Initializer.zeros:
                    initWeights_Value(0);
                    break;
                case Initializer.binary:
                    initWeights_Binary();
                    break;
                case Initializer.random:
                    initWeights_Random(weight_init_min, weight_init_max);
                    break;
            }
        }

        /// <summary>
        /// uses binary init
        /// </summary>
        /// <param name="weightCount"></param>
        /// <param name="activation"></param>
        public Neuron(int weightCount, IActivation<double> activation, int weight_init_min, int weight_init_max)
        {
            this.node_activation = activation;
            this.node_weights = new double[weightCount];
            this.node_last_inputs = new double[weightCount];
            node_bias = 1.0;
            initWeights_Random(weight_init_min, weight_init_max);
        }

        public Neuron_Model save_node()
        {
            return new Neuron_Model()
            {
                activationMethod = this.node_activation.GetType().Name,
                weights = this.node_weights,
                bias = this.node_bias
            };
        }

        /// <summary>
        /// activate, with deriv option
        /// </summary>
        /// <param name="deriv"></param>
        /// <returns></returns>
        public double Node_Activate(bool deriv)
        {
            double rtn = 0;
            for (int i = 0; i < node_weights.Length; i++)
            {
                rtn += node_weights[i] * node_last_inputs[i];
            }
            return this.node_activation.Activate(rtn + node_bias, deriv);
        }

        /// <summary>
        /// init weights using xavier init
        /// </summary>
        private void initWeights_Random(int min, int max)
        {
            Random r = new Random();
            for (int i = 0; i < node_weights.Length; i++)
            {
                node_weights[i] = (double)r.Next(min, max) * r.NextDouble();
            }
        }

        private void initWeights_Binary()
        {
            Random r = new Random();
            for (int i = 0; i < node_weights.Length; i++)
            {
                node_weights[i] = (double)r.Next(0, 1);
            }
        }

        private void initWeights_Value(double val)
        {
            for (int i = 0; i < node_weights.Length; i++)
            {
                node_weights[i] = val;
            }
        }

        /// <summary>
        /// use learning rate to ajust this neuron weight
        /// </summary>
        /// <param name="learnrate"></param>
        public void AdjustWeights(double learnrate, bool adjust_bias = true)
        {
            for (int i = 0; i < this.node_weights.Length; i++)
            {
                node_weights[i] += learnrate * error * node_last_inputs[i];
            }
            if (adjust_bias)
            {
                node_bias += error;
            }
        }
    }
}