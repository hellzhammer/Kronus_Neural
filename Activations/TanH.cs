using System;

namespace Kronus_Neural.Activations
{
    public class TanH : IActivation<double>
    {
        public double Activate(double input, bool deriv)
        {
            if (deriv)
            {
                return Tanh_Deriv(input);
            }
            else
            {
                return Tanh(input);
            }
        }

        private double Tanh(double value)
        {
            return
                (System.Math.Exp(value) - System.Math.Exp(-value))
                /
                (System.Math.Exp(value) + System.Math.Exp(-value));
        }

        private double Tanh_Deriv(double value)
        {
            return 1 - (
                (System.Math.Exp(value) - System.Math.Exp(-value))
                *
                (System.Math.Exp(value) - System.Math.Exp(-value)))
                /
                ((System.Math.Exp(value) + System.Math.Exp(-value))
                *
                (System.Math.Exp(value) + System.Math.Exp(-value)));
        }
    }
}
