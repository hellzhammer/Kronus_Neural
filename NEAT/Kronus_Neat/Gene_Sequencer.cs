using System;
using System.Collections.Generic;
using System.Text;

namespace Kronus_Neural.NEAT
{
    public class Gene_Sequencer
    {
        public static Gene_Sequence get_sequence(NeatNetwork fittest, NeatNetwork other)
        {
            Gene_Sequence gs = new Gene_Sequence();

            int index = 0;
            foreach (var c1 in fittest.All_Connections)
            {
                if (other.All_Connections.ContainsKey(c1.Key))
                {
                    gs.add_set(new Gene_Pair(c1.Key, true, true, Gene_Match_Type.match) { weight_diff = c1.Value.Weight - other.All_Connections[c1.Key].Weight });
                }
                else
                {
                    if (index < other.All_Connections.Count - 1 && index < fittest.All_Connections.Count - 1)
                    {
                        gs.add_set(new Gene_Pair(c1.Key, true, false, Gene_Match_Type.disjoint) { weight_diff = 0 });
                    }
                    else if (index > other.All_Connections.Count - 1 && index < fittest.All_Connections.Count - 1)
                    {
                        gs.add_set(new Gene_Pair(c1.Key,true, false, Gene_Match_Type.excess) { weight_diff = 0 });
                    }
                }
                index++;
            }

            index = 0;
            foreach (var c2 in other.All_Connections)
            {
                if (!fittest.All_Connections.ContainsKey(c2.Key))
                {
                    if (index < other.All_Connections.Count - 1 && index < fittest.All_Connections.Count - 1)
                    {
                        gs.add_set(new Gene_Pair(c2.Key, false, true, Gene_Match_Type.disjoint) { weight_diff = 0 });
                    }
                    else if (index < other.All_Connections.Count - 1 && index > fittest.All_Connections.Count - 1)
                    {
                        gs.add_set(new Gene_Pair(c2.Key, false, true, Gene_Match_Type.excess) { weight_diff = 0 });
                    }
                }
                index++;
            }

            return gs;
        }
    }
}
