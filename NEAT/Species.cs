using System;
using System.Collections.Generic;

namespace Kronus_Neural.NEAT
{   
    public class Species
    {
        public string Species_ID { get; } // for identity

        public List<string> members { get; protected set; } // the creature ids in species
        public double fitness_avg { get; protected set; } // the species overall avg fitness
        public int gens_since_improvement { get; set; }

        public Species(string species_ID)
        {
            this.Species_ID = species_ID;
            this.fitness_avg = 0.0;
            members = new List<string>();
        }

        /// <summary>
        /// sets the avg fitness value
        /// </summary>
        /// <param name="new_avg"></param>
        public void set_avg_fitness(double new_avg)
        {
            this.fitness_avg = new_avg;
        }

        /// <summary>
        /// adds a new member if they dont already exist
        /// </summary>
        /// <param name="id"></param>
        public void add_member(string id)
        {
            if (!members.Contains(id))
            {
                this.members.Add(id);
            }
            else
            {
                Console.WriteLine("****  ERROR:  Cannot add new member as its id already exists! --  add_member(string id)  --  class: Species  ****");
            }
        }

        /// <summary>
        /// sets all members of the species at once
        /// </summary>
        /// <param name="new_members"></param>
        public void set_all_members(List<string> new_members)
        {
            this.members = new List<string>();
            for (int i = 0; i < new_members.Count; i++)
            {
                this.members.Add(new_members[i]);
            }
        }

        /// <summary>
        /// resets the species, leaves ID the same.
        /// </summary>
        public void Reset()
        {
            this.members = new List<string>();
            //this.fitness_avg = 0;
        }
    }
}
