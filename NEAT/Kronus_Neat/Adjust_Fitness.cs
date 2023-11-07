using System;
using System.Collections.Generic;

namespace Kronus_Neural.NEAT
{
    public class Adjust_Fitness
    {
        public static Dictionary<string, Species> Get_Species_Avg_Fitness(Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> species)
        {
            foreach (var spec in species)
            {
                double total = 0;
                for (int i = 0; i < spec.Value.members.Count; i++)
                {
                    total += nets[spec.Value.members[i]].current_fitness;
                }
                if (total <= spec.Value.fitness_avg)
                {
                    spec.Value.gens_since_improvement++;
                }
                else
                {
                    spec.Value.gens_since_improvement = 0;
                }
                spec.Value.set_avg_fitness(Math.Round(total / spec.Value.members.Count));
            }

            return species;
        }

        private static double Sum_Species_Avg_Fitness(Dictionary<string, Species> species)
        {
            double total = 0;
            foreach (var spec in species)
            {
                total += spec.Value.fitness_avg;
            }

            return total;
        }

        public static Dictionary<string, NeatNetwork> Get_Adjusted_Fitness(Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> species, double N)
        {
            double avgfitness = Sum_Species_Avg_Fitness(species);

            foreach (var item in nets)
            {
                if (species.ContainsKey(item.Value.species_id))
                {
                    item.Value.current_adjusted_fitness = species[item.Value.species_id].fitness_avg / avgfitness * N;
                }
            }

            return nets;
        }
    }
}
