namespace Kronus_Neural.Activations
{
    public class Gaussian : IActivation<double>
    {
        public double Activate(double input, bool deriv)
        {
            if (!deriv)
            {
                return System.Math.Pow(System.Math.E, -input * -input);
            }
            else
            {
                var one = System.Math.Pow(System.Math.E, -input * -input);
                var two = input * one;
                return -2 * two;
            }
        }
    }
}
