using System.Collections.Generic;

namespace Kronus_Neural.NEAT
{
    public enum Gene_Match_Type
    {
        match,
        excess,
        disjoint
    }

    public class Gene_Sequence
    {
        public double total_weight_diff { get; protected set; }
        public double total_Excess { get; protected set; }
        public double total_Disjoint { get; protected set; }
        public List<Gene_Pair> Sequenced_Genes { get; protected set; }
        public Gene_Sequence()
        {
            this.total_weight_diff = 0;
            this.total_Excess = 0;
            this.total_Disjoint = 0;
            this.Sequenced_Genes = new List<Gene_Pair>();
        }

        public void add_set(Gene_Pair pair)
        {
            this.total_weight_diff += pair.weight_diff;
            if (pair.matchType == Gene_Match_Type.disjoint)
            {
                total_Disjoint++;
            }
            else if (pair.matchType == Gene_Match_Type.excess)
            {
                total_Excess++;
            }
            this.Sequenced_Genes.Add(pair);
        }
    }
}
