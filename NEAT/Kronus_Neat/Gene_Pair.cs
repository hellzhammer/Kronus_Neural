namespace Kronus_Neural.NEAT
{
    public class Gene_Pair
    {
        public string gene_id { get; set; }
        public double weight_diff { get; set; }
        public bool parent1_contains_gene { get; private set; }
        public bool parent2_contains_gene { get; private set; }
        public Gene_Match_Type matchType { get; private set; }
        public Gene_Pair(string geneID, bool fittest, bool other, Gene_Match_Type match_type)
        {
            this.gene_id = geneID;
            this.parent1_contains_gene = fittest;
            this.parent2_contains_gene = other;

            this.matchType = match_type;
        }
    }
}
