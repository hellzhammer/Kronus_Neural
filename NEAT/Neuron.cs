using Kronus_Neural.Activations;

namespace Kronus_Neural.NEAT
{
    public class Neuron : IGene
    {
        public bool input_neuron { get; set; }
        public string gene_id { get; set; }
        public double Input { get; set; }

        public IActivation<double> activation_func { get; set; }

        public Neuron(bool is_input, string gene_identity, IActivation<double> activation)
        {
            this.activation_func = activation;
            this.gene_id = gene_identity;
            this.input_neuron = is_input;
        }

        public double Activate()
        {
            if (input_neuron)
            {
                return Input;
            }
            else
            {
                return activation_func.Activate(Input, false);
            }
        }

        public Neuron clone()
        {
            return new Neuron(this.input_neuron, this.gene_id, this.activation_func) { Input = this.Input };
        }
    }
}
