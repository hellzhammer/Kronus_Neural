using Kronus_Neural.Activations;
using System;
using System.Collections.Generic;

namespace Kronus_Neural.NEAT
{
    public abstract class NEAT_Project : Kronus_NEAT
    {
        /// <summary>
        /// //basic usage
        /// 
        /// public override void Run(){
        /// 
        /// //*your code here*
        /// 
        /// //assess fitness
        /// FitnessTest();
        /// 
        /// //run genetic algorithm
        /// Train();
        /// 
        /// //update genetic dictionaries
        /// UpdateGeneDictionaries();
        /// 
        /// //*your code here*
        /// 
        /// }
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Run()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// init the project: load in your data, prepare networks, etc.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void init_project()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// fitness function to be used by the system. 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void FitnessTest()
        {
            throw new NotImplementedException();
        }

        public void UpdateGeneDictionaries()
        {
            foreach (var net in nets)
            {
                foreach (var connection in net.Value.All_Connections)
                {
                    if (!this.gene_tracker.Connection_Exists(connection.Key))
                    {
                        this.gene_tracker.Add_Connection(connection.Key, epoch);
                    }
                }
                foreach (var node in net.Value.Hidden_Neurons)
                {
                    if (!this.gene_tracker.Neuron_Exists(node.Key))
                    {
                        this.gene_tracker.Add_Node(node.Key, epoch);
                    }
                }
            }
        }
    }
    /// <summary>
    /// inherit from this class to set up neat projects. 
    /// </summary>
    public class Kronus_NEAT
    {
        public double globalAverageFitness { get; protected set; }
        public bool use_recurrent = false;
        public int epoch { get; set; }
        public int total_epochs { get; protected set; }

        public Dictionary<string, NeatNetwork> nets { get; protected set; }
        public Dictionary<string, Species> species { get; protected set; }
        public Genetic_Dictionary gene_tracker { get; protected set; }

        public IActivation<double> Hidden_Activation_Function { get; protected set; }
        public IActivation<double> Output_Activation_Function { get; protected set; }

        public readonly string[] Activation_Types = { "Gaussian", "ReLU", "LeakyReLU", "Sigmoid", "Swish", "SoftPlus", "TanH" };
        public List<string> Allowed_Activations { get; protected set; }
        public bool mutate_activation = false;

        /// <summary>
        /// r.Next(weight_init_min, weight_init_max)
        /// </summary>
        public int weight_init_min = -20;
        /// <summary>
        /// r.Next(weight_init_min, weight_init_max)
        /// </summary>
        public int weight_init_max = 20;

        public bool init_fully_connected = false;
        /// <summary>
        /// Allows the fittest of each species to be cloned into the next generation
        /// </summary>
        public bool Clone_Elite = false;
        /// <summary>
        /// this value is what Random().NextDouble() must score higher than in order to make initial connection
        /// </summary>
        public double chance_to_make_inital_connection = 0.50;
        public int input_neuron_count { get; protected set; }
        public int output_neuron_count { get; protected set; }
        public int max_hidden_nodes = 50;

        /// <summary>
        /// max == 0.65 --> otherwise starting centroids are not chosen in some runs.
        /// </summary>
        public double chance_to_choose_initial_centroid = 0.60;

        // not sure where to use mutation power?
        public double MutationPower = 1.0;

        public int PopulationMax = 200;
        public double survivalChance = 0.30;

        public double excessCoeff_c1 = 1.0;
        public double disjointCoeff_c2 = 1.0;
        public double weightDiffCoeff_c3 = 0.4;

        private double mutateFunctionChance = 0.90;
        public double mutateFunctionChanceMax = 0.90;
        public double MutateFunctionChance
        {
            get { return mutateFunctionChance; }
            set
            {
                if (value > mutateFunctionChanceMax)
                {
                    mutateFunctionChance = mutateFunctionChanceMax;
                }
                else
                {
                    mutateFunctionChance = value;
                }
            }
        }

        private double mutateWeightChance = 0.90;
        public double mutateWeightChanceMax = 0.90;
        public double MutateWeightChance
        {
            get { return mutateWeightChance; }
            set
            {
                if (value > mutateWeightChanceMax)
                {
                    mutateWeightChance = mutateWeightChanceMax;
                }
                else
                {
                    mutateWeightChance = value;
                }
            }
        }

        private double mutateNeuronChance = 0.90;
        public double mutateNeuronChanceMax = 0.90;
        public double MutateNeuronChance
        {
            get { return mutateNeuronChance; }
            set
            {
                if (value > mutateNeuronChanceMax)
                {
                    mutateNeuronChance = mutateNeuronChanceMax;
                }
                else
                {
                    mutateNeuronChance = value;
                }
            }
        }

        private double mutateDeleteNeuronChance = 0.95;
        public double mutateDeleteNeuronChanceMax = 1.0;
        public double MutateDeleteNeuronChance
        {
            get { return mutateDeleteNeuronChance; }
            set
            {
                if (value > mutateDeleteNeuronChanceMax)
                {
                    mutateDeleteNeuronChance = mutateDeleteNeuronChanceMax;
                }
                else
                {
                    mutateDeleteNeuronChance = value;
                }
            }
        }

        private double mutateConnectionChance = 0.90;
        public double mutateConnectionChanceMax = 0.90;
        public double MutateConnectionChance
        {
            get { return mutateConnectionChance; }
            set
            {
                if (value > mutateConnectionChanceMax)
                {
                    mutateConnectionChance = mutateConnectionChanceMax;
                }
                else
                {
                    mutateConnectionChance = value;
                }
            }
        }

        private double mutateDeleteConnectionChance = 0.95;
        public double mutateDeleteConnectionChanceMax = 1.0;
        public double MutateDeleteConnectionChance
        {
            get { return mutateDeleteConnectionChance; }
            set
            {
                if (value > mutateDeleteConnectionChanceMax)
                {
                    mutateDeleteConnectionChance = mutateDeleteConnectionChanceMax;
                }
                else
                {
                    mutateDeleteConnectionChance = value;
                }
            }
        }

        public int totalSpeciesCountTarget = 5;

        public int dropOffAge = 15;

        public bool AutoAdjustCompatThresh = true;
        public double compatabilityThresholdMin = 0.75;
        public double compatabilityThresholdMax = 50.0;
        private double _compatabilityThreshold = 5.0;
        public double compatabilityThreshold
        {
            get { return _compatabilityThreshold; }
            set
            {
                if (value < compatabilityThresholdMin)
                {
                    _compatabilityThreshold = compatabilityThresholdMin;
                }
                else if (value > compatabilityThresholdMax)
                {
                    _compatabilityThreshold = compatabilityThresholdMax;
                }
                else
                {
                    _compatabilityThreshold = value;
                }
            }
        }

        private double compatabilityModifier = 0.025;
        public double compatabilityModifierMax = 0.5;
        public double CompatabilityModifier
        {
            get { return compatabilityModifier; }
            set
            {
                if (value > compatabilityModifierMax)
                {
                    compatabilityModifier = compatabilityModifierMax;
                }
                else
                {
                    compatabilityModifier = value;
                }
            }
        }

        /// <summary>
        /// this function finds the fittest agent in the dictionary of agents.
        /// </summary>
        /// <returns></returns>
        protected virtual string Find_Fittest_Network()
        {
            string fittest = string.Empty;
            foreach (var net in this.nets)
            {
                if (string.IsNullOrEmpty(fittest))
                {
                    fittest = net.Key;
                }
                else
                {
                    if (nets[fittest].current_fitness < net.Value.current_fitness)
                    {
                        fittest = net.Key;
                    }
                }
            }

            return fittest;
        }

        /// <summary>
        /// first epoch: 
        /// get fitness - you provide this code
        /// inits species 
        /// speciates
        /// 
        /// every following epoch:
        /// get fitness - you provide this code
        /// auto adjust compatability threshold - can be turned off or on;
        /// find species average fitness
        /// find each networks adjusted fitness
        /// purge species of the weak and unworthy
        /// reproduce
        /// mutate
        /// update species and speciate
        /// </summary>
        public virtual void Train()
        {
            try
            {
                // setup for neat algo
                (Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> species) data = (null, null);
                if (epoch < 1)
                {
                    epoch = 1;
                }

                // start NEAT ALGO
                // if   epoch is only 1, then we only want to speciate and init
                // else we want to update the centroids of each species and then speciate
                if (epoch == 1)
                {
                    data = Speciation.Init_Species(nets, chance_to_choose_initial_centroid, totalSpeciesCountTarget);
                }
                else if (epoch > 1)
                {
                    // check at the start of the training loop. if null, error out.
                    if (nets.Count == 0)
                    {
                        throw new Exception("Something has gone really wrong.");
                    }

                    data.nets = nets;
                    data.species = species;
                    if (this.AutoAdjustCompatThresh)
                    {
                        if (species.Count < this.totalSpeciesCountTarget)
                        {
                            this.compatabilityThreshold -= this.compatabilityModifier;
                        }
                        else if (species.Count > this.totalSpeciesCountTarget)
                        {
                            this.compatabilityThreshold += this.compatabilityModifier;
                        }
                    }

                    Dictionary<string, Species> species_avg_fitness =
                        Adjust_Fitness.Get_Species_Avg_Fitness(
                            data.nets,
                            data.species
                            );

                    Dictionary<string, NeatNetwork> adjusted_fitness =
                        Adjust_Fitness.Get_Adjusted_Fitness(
                            data.nets,
                            species_avg_fitness,
                            PopulationMax
                            );

                    if (this.species.Count == this.nets.Count)
                    {
                        Console.WriteLine("Species count has exploded! Could result in complete extermination of population.");
                        //throw new Exception("Species Count has exploded! Could result in complete extermination of population.");
                    }

                    (Dictionary<string, NeatNetwork> nets, Dictionary<string, Species> species) purged =
                        Purge.Purge_Species(
                            adjusted_fitness,
                            species_avg_fitness,
                            survivalChance,
                            this.dropOffAge
                            );

                    if (nets.Count == 0)
                    {
                        Console.WriteLine("Nets cannot be null or empty.");
                        //throw new Exception("Nets cannot be null or empty.");
                    }
                    if (this.species.Count == this.nets.Count)
                    {
                        Console.WriteLine("Species count has exploded! Could result in complete extermination of population.");
                        //throw new Exception("Species Count has exploded! Could result in complete extermination of population.");
                    }

                    //quickly remove all empty/extinct species
                    List<string> thepurged = new List<string>();
                    foreach (var purge in purged.species)
                    {
                        if (purge.Value.members.Count == 0)
                        {
                            thepurged.Add(purge.Key);
                        }
                    }

                    for (int z = 0; z < thepurged.Count; z++)
                    {
                        purged.species.Remove(thepurged[z]);
                    }

                    Dictionary<string, Species> new_species = new Dictionary<string, Species>();
                    Dictionary<string, NeatNetwork> next_generation = new Dictionary<string, NeatNetwork>();

                    // list of names is to use for iterating over nextgeneration and making changes.
                    List<string> names = new List<string>();

                    foreach (var spec in purged.species)
                    {
                        var reproduction = Reproduce.Reproduction(
                            purged.nets,
                            spec.Value,
                            this.Clone_Elite
                            );

                        // check if species members exist
                        if (reproduction.species.members.Count > 0)
                        {
                            // if they do add them to the next generation
                            // add names for mutation
                            foreach (var i in reproduction.nets)
                            {
                                names.Add(i.Key);
                                next_generation.Add(i.Key, i.Value);
                            }
                            // add to new species
                            new_species.Add(reproduction.species.Species_ID, reproduction.species);
                        }
                    }

                    // mutate
                    for (int i = 0; i < names.Count; i++)
                    {
                        next_generation[names[i]] = Mutation.Remove_Node(
                            next_generation[names[i]],
                            this.MutateDeleteNeuronChance
                            );
                        next_generation[names[i]] = Mutation.Adjust_Weights(
                            next_generation[names[i]],
                            this.MutateWeightChance,
                            this.MutationPower,
                            this.weight_init_min,
                            this.weight_init_max
                            );
                        next_generation[names[i]] = Mutation.Add_Node(
                            next_generation[names[i]],
                            this.MutateNeuronChance,
                            this.max_hidden_nodes,
                            this.Hidden_Activation_Function,
                            this.weight_init_min,
                            this.weight_init_max
                            );
                        next_generation[names[i]] = Mutation.Remove_Connection(
                            next_generation[names[i]],
                            this.MutateDeleteConnectionChance
                            );
                        next_generation[names[i]] = Mutation.Add_Connection(
                            next_generation[names[i]],
                            this.MutateConnectionChance,
                            this.weight_init_min,
                            this.weight_init_max
                            );
                        if (use_recurrent)
                        {
                            next_generation[names[i]] = Mutation.Add_Recurrent_Connection(
                            next_generation[names[i]],
                            this.MutateConnectionChance,
                            this.weight_init_min,
                            this.weight_init_max
                        );
                        }

                        if (mutate_activation && this.Allowed_Activations != null)
                        {
                            next_generation[names[i]] = Mutation.Alter_Function(
                                next_generation[names[i]],
                                MutateFunctionChance,
                                this.Allowed_Activations
                                );
                        }
                    }

                    // if to much die off occurs, the population is reset with new un-mutated networks
                    if (next_generation.Count < PopulationMax)
                    {
                        int remainder = (int)PopulationMax - next_generation.Count;
                        for (int i = 0; i < remainder; i++)
                        {
                            NeatNetwork net =
                                Network_Generator.Generate_New_Network_Neurons(
                                    input_neuron_count,
                                    output_neuron_count,
                                    this.Hidden_Activation_Function,
                                    this.Output_Activation_Function
                                    );

                            if (!this.init_fully_connected)
                            {
                                net =
                                    Network_Generator.Init_Connections_random_connections(
                                        net,
                                        this.chance_to_make_inital_connection,
                                        this.weight_init_min,
                                        this.weight_init_max
                                        );
                            }
                            else
                            {
                                net =
                                    Network_Generator.Init_Connections_fully_connected(
                                        net,
                                        this.weight_init_min,
                                        this.weight_init_max
                                        );
                            }
                            next_generation.Add(
                                net.network_id,
                                net
                                );
                        }
                    }

                    data =
                        Speciation.Update_Species(
                        next_generation,
                        new_species
                        );
                }

                data =
                    Speciation.Speciate(
                        data.nets,
                        data.species,
                        excessCoeff_c1,
                        disjointCoeff_c2,
                        weightDiffCoeff_c3,
                        compatabilityThreshold
                        );

                this.nets = data.nets;
                this.species = data.species;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
