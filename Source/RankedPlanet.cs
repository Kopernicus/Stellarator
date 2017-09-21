using Accrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellarator
{


    static class RankedPlanet
    {
        /// <summary>
        /// Distance from home planet - higher is more distant
        /// </summary>
        public static int Rank(this Planet planet, List<Planet> allPlanets, Planet kerbin)
        {
            int rank = RankDownwards(planet, kerbin);

            // Found kerbin
            if (rank > -1)
            {
                return rank;
            }

            rank = 1;
            Planet parent = planet.parent_body(allPlanets);
            bool found = false;
            while (!found)
            {
                // This planet orbits kerbin
                if (parent == kerbin)
                {
                    return rank;
                }

                // Look at following siblings
                int siblingrank = 0;
                Planet sibling = planet.next_planet;
                while (sibling != null)
                {
                    siblingrank++;

                    int downwardRank = RankDownwards(sibling, kerbin);
                    if (downwardRank > -1)
                    {
                        // Add sibling rank only if a different planet - parent is sun
                        return rank + (parent == null ? siblingrank * 3 : 0) + downwardRank;
                    }

                    sibling = sibling.next_planet;
                }

                // Look at preceding siblings
                siblingrank = 0;
                sibling = planet.previous_planet(allPlanets);
                while (sibling != null)
                {
                    siblingrank++;
                    int downwardRank = RankDownwards(sibling, kerbin);
                    if (downwardRank > -1)
                    {
                        // Add sibling rank only if a different planet - parent is sun
                        return rank + (parent == null ? siblingrank * 2 : 0) + downwardRank;
                    }

                    sibling = sibling.previous_planet(allPlanets);
                }

                if (parent == null)
                {
                    break;
                }

                // Continue upwards
                planet = parent;
                parent = parent.parent_body(allPlanets);
                rank++;
            }

            return -1;
        }

        private static int RankDownwards(Planet planet, Planet kerbin)
        {
            // This is kerbin, rank 0;
            if (planet == kerbin)
            {
                return 0;
            }

            // Reached leaf planet - no kerbin
            if (planet.BodiesOrbiting == null)
            {
                return -1;
            }

            // Look for kerbin downwards
            foreach (var orbiting in planet.BodiesOrbiting)
            {
                int rank = RankDownwards(orbiting, kerbin);
                if (rank > -1)
                {
                    // Found kerbin, return 1 more rank
                    return rank + 1;
                }
            }

            // Not found
            return -1;
        }

        public static Planet previous_planet(this Planet planet, List<Planet> allPlanets)
        {
            return allPlanets.Where(x => x.next_planet == planet).FirstOrDefault();
        }

        public static Planet parent_body(this Planet planet, List<Planet> allPlanets)
        {
            return allPlanets.Where(x => x.BodiesOrbiting != null && x.BodiesOrbiting.Contains(planet)).FirstOrDefault();
        }
    }
}
