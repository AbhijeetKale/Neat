using System.Collections.Generic;
using Neat.Config;
using Neat.Components;
using Neat.Util;
namespace Neat.Framework
{
    public class NeatMain
    {
        public static NeatConfig config = new NeatConfig();

        protected int innovationNumber = 1;

        public NeatMain() {
            
        }

        public NeatMain(NeatConfig config)
        {
            NeatMain.config = config;
        }
    }
    public class Species
    {
        private List<Genome> speciesPopulation;

        // a radndomly chosen specimen to compare other sample genomes with
        private Genome representativeGenome = null;

        public Species()
        {
            speciesPopulation = new List<Genome>();
            representativeGenome = null;
        }

        public void setRandomRepresentative()
        {
            if (speciesPopulation.Count == 0)
            {
                representativeGenome = null;
            }
            else
            {
                representativeGenome =
                    RandomGenerator.getRandomElementFromList<Genome>(speciesPopulation);
            }
        }

        public bool tryAddingGenome(Genome genome)
        {
            if (checkCompatibility(genome))
            {
                speciesPopulation.Add(genome);
                setRandomRepresentative();
                return true;
            }
            return false;
        }

        public void removeGenomeFrom(int fromIndex)
        {
            this.speciesPopulation.RemoveRange(fromIndex, speciesPopulation.Count - fromIndex);
            setRandomRepresentative();
        }

        // check if a particular genome belongs to this species
        public bool checkCompatibility(Genome genome)
        {
            if (representativeGenome == null)
            {
                return false;
            }
            int i = 0, j = 0;
            int matchingGenesCount = 0;
            int disjointGenes = 0;
            double avgWeightDifference = 0;
            while (i < genome.getGenes().Count && j < representativeGenome.getGenes().Count)
            {
                Gene gene1 = genome.getGenes()[i];
                Gene gene2 = representativeGenome.getGenes()[j];
                // matching gene case. Choose a gene at random
                if (gene1.inovationNumber == gene2.inovationNumber)
                {
                    matchingGenesCount++;
                    avgWeightDifference += abs(gene1.weight - gene2.weight);
                    i++;
                    j++;
                }
                // gene 2 isa disjoint gene
                else if (gene1.inovationNumber > gene2.inovationNumber)
                {
                    disjointGenes++;
                    j++;
                }
                // gene 1 is a disjoint gene
                else if (gene1.inovationNumber < gene2.inovationNumber)
                {
                    disjointGenes++;
                    i++;
                }
            }
            avgWeightDifference = avgWeightDifference / matchingGenesCount;
            int excessGeneCount = 0;
            while (i < genome.getGenes().Count)
            {
                excessGeneCount++;
            }
            while (j < representativeGenome.getGenes().Count)
            {
                excessGeneCount++;
            }
            double delta = NeatMain.config.delta_C1 * (double)excessGeneCount + NeatMain.config.delta_C2 * (double)disjointGenes;
            if (this.speciesPopulation.Count > NeatMain.config.population_Normalization_Threshold)
            {
                delta = delta / this.speciesPopulation.Count;
            }
            delta += NeatMain.config.delta_C3 * avgWeightDifference;
            return delta > NeatMain.config.delta_Threshhold ? false : true;
        }
        private double abs(double a) => a < 0 ? -a : a;
    }
}
