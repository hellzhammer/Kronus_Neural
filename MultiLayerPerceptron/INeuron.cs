using Kronus_Neural.Activations;

namespace Kronus_Neural.MultiLayerPerceptron 
{

    public interface INeuron<T>
    {
        IActivation<double> node_activation { get; set; }
        T node_last_inputs { get; set; }
        //double node_bias { get; set; }
        T node_weights { get; set; }
        double Node_Activate(bool deriv);
        void AdjustWeights(double learnrate, bool adjust_bias = true);
    }
}