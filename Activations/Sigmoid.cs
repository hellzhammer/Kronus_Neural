using System;

namespace Kronus_Neural.Activations
{
    public partial class Sigmoid : IActivation<double>
    {
        public double Activate(double input, bool deriv)
        {
            if (deriv)
            {
                return Sigmoid_Deriv(input);
            }
            else
            {
                return _Sigmoid(input);
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

        /// <summary>
        /// returns the derivitive of sigmoid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private double Sigmoid_Deriv(double input)
        {
            return input * (1.0 - input);
            //return 1 / (1 + (1 / System.Math.Pow(System.Math.E, -input)));
        }
    }
}
