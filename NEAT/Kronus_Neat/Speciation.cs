using System;
using System.Collections.Generic;

namespace Kronus_Neural.NEAT
{
    public class Speciation
    {
        /// <summary>
        /// Inits the species counts and selects random starting networks 
        /// as the centroids for each species.
        /// 
        /// double chance MAX value is 0.65
        /// to prevent to few species from being made.
        /// </summary>
        public static (Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> species) Init_Species(Dictionary<string, NeatNetwork> nets, double chance, int species_target)
        {
            try
            {
                if (chance > 0.65)
                {
                    chance = 0.65;
                }

                Random r = new Random();
                Dictionary<string, Species> rtnval = new Dictionary<string, Species>();

                foreach (var net in nets)
                {
                    // if 
                    if (r.NextDouble() >= chance)
                    {
                        Species s = new Species("species - " + Guid.NewGuid());
                        s.add_member(net.Key);
                        net.Value.species_id = s.Species_ID;
                        rtnval.Add(s.Species_ID, s);
                    }

                    // stop adding new species if species target has been reached. 
                    if (rtnval.Count == species_target)
                    {
                        break;
                    }
                }

                return (nets, rtnval);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// like init_species, this function updates the species fittest member.
        /// use this after the first generation and before speciation.
        /// </summary>
        /// <param name="nets"></param>
        /// <param name="current_species"></param>
        /// <returns></returns>
        public static (Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> species) Update_Species(Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> current_species)
        {
            try
            {
                // essentially we just want to find the fitest member of each species 
                // once found they become the new centroid for speciation

                foreach (var species in current_species)
                {
                    if (species.Value.members.Count > 1)
                    {
                        string fittest = species.Value.members[0];
                        for (int i = 1; i < species.Value.members.Count; i++)
                        {
                            if (nets[fittest].current_fitness < nets[species.Value.members[i]].current_fitness)
                            {
                                fittest = species.Value.members[i];
                            }
                        }
                        species.Value.Reset();
                        species.Value.members.Add(fittest);
                        nets[fittest].species_id = species.Key;
                    }
                    // else we want to ignore and move on.
                    // we use the member at position 0 as the fittest member;
                }

                return (nets, current_species);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// simply clusters all networks into their species catagory.
        /// </summary>
        /// <param name="first_run"></param>
        public static (Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> species) Speciate(Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> species, double c1, double c2, double c3, double compat_thresh)
        {
            // use existing centroids to catagorize species
            foreach (var net in nets)
            {
                
                // check to make sure we dont re-class the fittest member
                bool exists = false;

                try
                {
                    foreach (var item in species)
                    {
                        if (item.Value.members[0] == net.Key)
                        {
                            exists = true;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                if (!exists)
                {
                    string fittest_id = string.Empty;
                    double fittest_delta = 0;
                    foreach (var spec in species)
                    {
                        // sequence the gene of both networks
                        Gene_Sequence seq = Gene_Sequencer.get_sequence(nets[spec.Value.members[0]], net.Value);
                        // get the current delta                        
                        double cur_del = Numerics.NEAT_Math.GetDelta(nets.Count, c1, c2, c3, seq.total_Excess, seq.total_Disjoint, seq.total_weight_diff);

                        // if the calculated delta is less than the compatability threshold
                        // we can compare with the fittest delta
                        if (cur_del < compat_thresh)
                        {
                            // this code will only fire, obviously,
                            // if the the delta is less than the thresh

                            // if first iter
                            // set the fittest to current
                            if (fittest_id == String.Empty)
                            {
                                fittest_delta = cur_del;
                                fittest_id = spec.Key;
                            }
                            // if delta is less than the fittest, it becomes the fittest
                            else if (fittest_id != String.Empty && cur_del < fittest_delta)
                            {
                                fittest_delta = cur_del;
                                fittest_id = spec.Key;
                            }
                        }
                    }

                    try
                    {
                        // if the fittest id is still empty then we can create a new species
                        // if not the we can add to existing species
                        if (!string.IsNullOrEmpty(fittest_id))
                        {
                            species[fittest_id].add_member(net.Key);
                            net.Value.species_id = species[fittest_id].Species_ID;
                        }
                        else
                        {
                            Species s = new Species("species - " + Guid.NewGuid());
                            s.add_member(net.Key);
                            net.Value.species_id = s.Species_ID;
                            species.Add(s.Species_ID, s);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }

            return (nets, species);
        }
    }
}
