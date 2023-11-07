using System;
using System.Collections.Generic;
using System.Text;

namespace Kronus_Neural.NEAT
{
    public class Purge
    {
        /// <summary>
        /// purges all species 
        /// </summary>
        /// <returns></returns>
        public static (Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> species) Purge_Species(Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> current_species, double survival_chance, double drop_off_age)
        {
            try
            {
                // here i iterate through in order to find all of the fittest members.
                // iterator is the survivors parameter calculated by using the survival_chance var
                // only n number of members can survive the purge
                List<string> purged_species = new List<string>();
                var list_of_nets = nets;
                Dictionary<string, NeatNetwork> n_nets = new Dictionary<string, NeatNetwork>();
                foreach (var item in nets)
                {
                    n_nets.Add(item.Key, item.Value);
                }

                foreach (var species in current_species)
                {
                    if (species.Value.gens_since_improvement < drop_off_age)
                    {
                        List<string> survived = new List<string>();
                        int survivors = (int)Math.Round(species.Value.members.Count * survival_chance);

                        for (int i = 0; i < survivors; i++)
                        {
                            string survivor = species.Value.members[0];

                            for (int m = 1; m < species.Value.members.Count; m++)
                            {
                                if (nets[survivor].current_fitness < nets[species.Value.members[m]].current_fitness)
                                {
                                    survivor = species.Value.members[m];
                                }
                            }

                            // remove fittest from the rest of species population
                            species.Value.members.Remove(survivor);
                            // add member to fittest list
                            survived.Add(survivor);
                        }

                        // purge the unclean from the overall global population
                        for (int m = 0; m < species.Value.members.Count; m++)
                        {
                            nets.Remove(species.Value.members[m]);
                        }
                        // set the fittest list (survived) as the members list.
                        species.Value.set_all_members(survived);
                    }
                    else
                    {
                        purged_species.Add(species.Key);
                    }
                }

                for (int i = 0; i < purged_species.Count; i++)
                {
                    current_species.Remove(purged_species[i]);
                }

                return (nets, current_species);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
