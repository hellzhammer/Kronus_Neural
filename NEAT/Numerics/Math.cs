namespace Kronus_Neural.NEAT.Numerics
{
    public class NEAT_Math
    {
        public static double GetDelta(double n, double c1, double c2, double c3, double totalExcess, double totalDisjoint, double totalWeightDiff)
        {
            var x = (
                    ((c1 * totalExcess) / n)
                    +
                    ((c2 * totalDisjoint) / n)
                    +
                    (c3 * totalWeightDiff)
                    );
            if (x < 0)
            {
                x = x * x;
            }

            return x;
        }
    }
}
