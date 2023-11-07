namespace Kronus_Neural.Activations
{
    public class Swish : IActivation<double>
    {
        public double Activate(double input, bool deriv)
        {
            if (deriv)
            {
                //(b*swish_function(b, x)) + (sigmoid(b, x)*(1-(b*swish_function(b, x))))
                return (input * _Sigmoid(input)) + (_Sigmoid(input) * (1 - (input * _Sigmoid(input))));
            }
            else
            {
                return input * _Sigmoid(input);
            }
        }

        /// <summary>
        /// returns a sigmoid output
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private double _Sigmoid(double input)
        {
            return 1.0 / (1.0 + System.Math.Pow(System.Math.E, -input));
        }
    }
}
