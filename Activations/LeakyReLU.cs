namespace Kronus_Neural.Activations
{
    public class LeakyReLU : IActivation<double>
    {
        public double Activate(double input, bool deriv)
        {
            if (input > 0)
            {
                if (deriv)
                {
                    return 1.0;
                }
                else
                {
                    return input;
                }
            }
            else
            {
                return 0.001;
            }
        }
    }
}
