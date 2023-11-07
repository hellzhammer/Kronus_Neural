using System;

namespace Kronus_Neural.Activations
{
    public class Softmax : IActivation<double[]>
    {
        public double[] Activate(double[] input, bool deriv)
        {
            if (!deriv)
            {
                double[] z_exp = new double[input.Length]; 

                for (int ze = 0; ze < z_exp.Length; ze++)
                {
                    z_exp[ze] = Math.Exp(input[ze]);
                }

                double sum_z_exp = 0.0;
                for (int i = 0; i < z_exp.Length; i++)
                {
                    sum_z_exp += z_exp[i];
                }

                var softmax = new double[z_exp.Length];
                for (int i = 0; i < softmax.Length; i++)
                {
                    softmax[i] = z_exp[i] / sum_z_exp;
                }

                return softmax;
            }
            else
            {
                double[] outp = new double[input.Length];
                for (int i = 0; i < outp.Length; i++)
                {
                    outp[i] = input[i] * (1d - input[i]);
                }
                return outp;
            }
        }
    }
}
