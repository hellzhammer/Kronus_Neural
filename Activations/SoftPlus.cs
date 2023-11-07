using System;

namespace Kronus_Neural.Activations
{
    public class SoftPlus : IActivation<double>
    {
        public double Activate(double input, bool deriv)
        {
            if (!deriv)
            {
                return System.Math.Log(System.Math.Pow(System.Math.E, input) + 1);
            }
            else
            {
                return 1 / (1 + System.Math.Pow(System.Math.E, -input));
            }
        }
    }
}
