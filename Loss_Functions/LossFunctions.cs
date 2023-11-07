using Kronus_Neural.MultiLayerPerceptron;

namespace Kronus_Neural.Loss_Functions
{
    public delegate Neuron[] Loss_Function(double[] expected, double learnrate, Neuron[] output_nodes);

    public class LossFunctions
    {
        public static Neuron[] MLP_LOSS(double[] expected, double learnrate, Neuron[] output_nodes)
        {
            for (int o = 0; o < output_nodes.Length; o++)
            {
                // get nodes output
                double actual_node_outp = output_nodes[o].Node_Activate(false);

                // calculate error from prediction 
                output_nodes[o].error = output_nodes[o].node_activation.Activate(actual_node_outp, true) * (expected[o] - actual_node_outp);

                // adjust weights
                output_nodes[o].AdjustWeights(learnrate);
            }

            return output_nodes;
        }
    }
}
