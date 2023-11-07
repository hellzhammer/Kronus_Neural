using System;
using System.Collections.Generic;
using System.Text;

namespace Kronus_Neural.Activations
{
    public class Linear : IActivation<double>
    {
        public double Activate(double input, bool deriv)
        {
            if (deriv)
            {
                return 1.0;
            }
            else
            {
                return input * 1.0;
            }
        }
    }
}
