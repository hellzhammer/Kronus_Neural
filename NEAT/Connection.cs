using System;

namespace Kronus_Neural.NEAT
{
    public class Connection : IGene 
    {
        public string gene_id { get; set; }

        public string Input { get; set; }
        public string Output { get; set; }
        public double Weight { get; set; }

        public bool is_recurrent { get; set; }

        public Connection(string gene_identity, int min, int max, string in_node, string out_node)
        {
            Random r = new Random();
            this.Weight = ((double)r.Next(min, max) * r.NextDouble()) * (1.0 - -1.0) + -1.0;
            this.gene_id = gene_identity;
            this.Input = in_node;
            this.Output = out_node;
        }

        public Connection(string gene_identity, double weight, string in_node, string out_node)
        {
            this.Weight = weight;
            this.gene_id = gene_identity;
            this.Input = in_node;
            this.Output = out_node;
        }

        public Connection clone()
        {
            return new Connection(this.gene_id, this.Weight, this.Input, this.Output) 
            {
                is_recurrent = this.is_recurrent 
            };
        }
    }
}
