using System;
using System.Collections.Generic;

namespace Kronus_Neural.NEAT
{
    public class Reproduce
    {
        public static (Dictionary<string, NeatNetwork> nets, Species species) Reproduction(Dictionary<string, NeatNetwork> nets, Species species, bool eliteism)
        {
            Random r = new Random();
            Dictionary<string, NeatNetwork> offspring = new Dictionary<string, NeatNetwork>();
            List<string> new_members = new List<string>();            

            if (species.members.Count > 1) 
            {
                // this section handles sexual reproduction.
                // get instance of the 2 fittest members of a species
                string fittest = species.members[0];
                string second = species.members[1];
                for (int y = 2; y < species.members.Count; y++)
                {
                    if (nets[species.members[y]].current_fitness > nets[second].current_fitness)
                    {
                        second = species.members[y];
                    }
                }

                Gene_Sequence gs = Gene_Sequencer.get_sequence(nets[fittest], nets[second]);
                double offspring_count = nets[species.members[0]].current_adjusted_fitness;
                
                // if eliteism is in then we want to set fittest member back into the next gen.
                if (eliteism && offspring_count > 0)
                {
                    NeatNetwork nn = nets[species.members[0]].clone();
                    nn.set_net_id(nets[species.members[0]].network_id);

                    new_members.Add(nn.network_id);
                    offspring.Add(nn.network_id, nn);

                    offspring_count -= 1.0;
                }
                for (int i = 0; i < offspring_count; i++)
                {                                 
                    NeatNetwork new_net = new NeatNetwork();
                    new_net.species_id = "network - " + Guid.NewGuid().ToString();
                    //new_net.Input_Neurons = nets[fittest].Input_Neurons;
                    foreach (var item in nets[fittest].Input_Neurons)
                    {
                        new_net.Input_Neurons.Add(item.Key, item.Value.clone());
                    }
                    //new_net.Output_Neurons = nets[fittest].Output_Neurons;
                    foreach (var item in nets[fittest].Output_Neurons)
                    {
                        new_net.Output_Neurons.Add(item.Key, item.Value.clone());
                    }

                    // the following section may need some heavy debugging. expecting all kinds of errors
                    // there has been no testing, whatsoever

                    for (int s = 0; s < gs.Sequenced_Genes.Count; s++)
                    {
                        if (gs.Sequenced_Genes[s].matchType == Gene_Match_Type.match)
                        {
                            new_net.All_Connections.Add(gs.Sequenced_Genes[s].gene_id, nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].clone());

                            bool fit_contains = nets[fittest].Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input);
                            bool new_net_does_not_contain = new_net.Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input);
                            if (fit_contains && !new_net_does_not_contain)
                            {
                                new_net.Hidden_Neurons.Add(
                                    nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input,
                                    nets[fittest].Hidden_Neurons[nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input].clone()
                                    );
                            }
                            if (nets[fittest].Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output)
                                &&
                                nets[second].Hidden_Neurons.ContainsKey(nets[second].All_Connections[gs.Sequenced_Genes[s].gene_id].Output)
                                &&
                                !new_net.Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output))
                            {
                                double p_chance = new Random().NextDouble();
                                if (p_chance < 0.90)
                                {
                                    new_net.Hidden_Neurons.Add(
                                    nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output,
                                    nets[fittest].Hidden_Neurons[nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output].clone()
                                    );
                                }
                                else if (p_chance >= 0.90)
                                {
                                    new_net.Hidden_Neurons.Add(
                                    nets[second].All_Connections[gs.Sequenced_Genes[s].gene_id].Output,
                                    nets[second].Hidden_Neurons[nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output].clone()
                                    );
                                }
                            }
                        }

                        else if (gs.Sequenced_Genes[s].matchType == Gene_Match_Type.disjoint && gs.Sequenced_Genes[s].parent1_contains_gene)
                        {
                            new_net.All_Connections.Add(gs.Sequenced_Genes[s].gene_id, nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id]);
                            if (nets[fittest].Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input)
                                &&
                                !new_net.Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input))
                            {
                                new_net.Hidden_Neurons.Add(
                                    nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input,
                                    nets[fittest].Hidden_Neurons[nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input].clone()
                                    );
                            }
                            if (nets[fittest].Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output)
                                &&
                                !new_net.Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output))
                            {
                                new_net.Hidden_Neurons.Add(
                                    nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output,
                                    nets[fittest].Hidden_Neurons[nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output].clone()
                                    );
                            }
                        }
                        // if parent one contains add it, else 10% chance
                        else if (gs.Sequenced_Genes[s].matchType == Gene_Match_Type.excess && gs.Sequenced_Genes[s].parent1_contains_gene)
                        {
                            new_net.All_Connections.Add(gs.Sequenced_Genes[s].gene_id, nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].clone());
                            if (nets[fittest].Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input)
                                &&
                                !new_net.Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input))
                            {
                                new_net.Hidden_Neurons.Add(
                                    nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input,
                                    nets[fittest].Hidden_Neurons[nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Input].clone()
                                    );
                            }
                            if (nets[fittest].Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output)
                                &&
                                !new_net.Hidden_Neurons.ContainsKey(nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output))
                            {
                                new_net.Hidden_Neurons.Add(
                                    nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output,
                                    nets[fittest].Hidden_Neurons[nets[fittest].All_Connections[gs.Sequenced_Genes[s].gene_id].Output].clone()
                                    );
                            }
                        }
                    }
                    // add to new members
                    new_members.Add(new_net.network_id);
                    offspring.Add(new_net.network_id, new_net);
                }
            }
            species.set_all_members(new_members);
            return (offspring, species);
        }
    }
}
