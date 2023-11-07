using Kronus_Neural.Activations;
using System;

namespace Kronus_Neural.NEAT
{
    public class Network_Generator
    {
        /// <summary>
        /// creates the whole neural network with random connections.
        /// </summary>
        /// <param name="input_count"> number of input neurons </param>
        /// <param name="output_count"> number of output neurons </param>
        /// <param name="chance_for_connection"> what double r.NextDouble() has to score higher than </param>
        /// <returns></returns>
        public static NeatNetwork Init_Random_Network(int input_count, int output_count, double chance_for_connection, IActivation<double> hidden_act, IActivation<double> output_act, int weight_min, int weight_max)
        {
            var net = Generate_New_Network_Neurons(input_count, output_count, hidden_act, output_act);
            net = Init_Connections_random_connections(net, chance_for_connection, weight_min, weight_max);
            return net;
        }

        /// <summary>
        /// creates the whole fully connected neural network.
        /// </summary>
        /// <param name="input_count"> number of input neurons </param>
        /// <param name="output_count"> number of output neurons </param>
        /// <returns></returns>
        public static NeatNetwork Init_FullyConnected_Network(int input_count, int output_count, IActivation<double> hidden_act, IActivation<double> output_act, int weight_min, int weight_max)
        {
            var net = Generate_New_Network_Neurons(input_count, output_count, hidden_act, output_act);
            net = Init_Connections_fully_connected(net, weight_min, weight_max);
            return net;
        }

        /// <summary>
        /// creates the initial connections in a neural network;
        /// fully connected
        /// </summary>
        /// <param name="net"> the network to be connected </param>
        /// <returns></returns>
        public static NeatNetwork Init_Connections_fully_connected(NeatNetwork net, int min, int max)
        {
            foreach (var c1 in net.Input_Neurons)
            {
                foreach (var c2 in net.Output_Neurons)
                {
                    Connection c = new Connection(c1.Key + "-" + c2.Key, min, max, c1.Key, c2.Key) { is_recurrent = false };
                    net.All_Connections.Add(c.gene_id, c);
                }
            }

            return net;
        }

        /// <summary>
        /// creates randomly selected starting connections
        /// </summary>
        /// <param name="net"> neural network to connect </param>
        /// <param name="chance_to_connect"> the threshold needed to create a connection </param>
        /// <returns></returns>
        public static NeatNetwork Init_Connections_random_connections(NeatNetwork net, double chance_to_connect, int min, int max)
        {
            Random r = new Random();
            foreach (var c1 in net.Input_Neurons)
            {
                foreach (var c2 in net.Output_Neurons)
                {
                    if (r.NextDouble() >= chance_to_connect)
                    {
                        Connection c = new Connection(c1.Key + "-" + c2.Key, min, max, c1.Key, c2.Key) { is_recurrent = false };
                        net.All_Connections.Add(c.gene_id, c);
                    }
                }
            }
            return net;
        }

        /// <summary>
        /// generates a new series of input and output nodes.
        /// </summary>
        /// <param name="input_count"> input neuron count </param>
        /// <param name="output_count"> output neuron count </param>
        /// <returns></returns>
        public static NeatNetwork Generate_New_Network_Neurons(int input_count, int output_count, IActivation<double> hidden_act, IActivation<double> output_act)
        {
            NeatNetwork new_net = new NeatNetwork();
            for (int i = 0; i < input_count; i++)
            {
                new_net.Input_Neurons.Add(i.ToString(), new Neuron(true, i.ToString(), hidden_act));
            }

            for (int o = 0; o < output_count; o++)
            {
                new_net.Output_Neurons.Add((input_count + o).ToString(), new Neuron(false, (input_count + o).ToString(), output_act));
            }

            return new_net;
        }
    }
}
