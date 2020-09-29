using System.Collections.Generic;
using Neat.Config;
using Neat.Components;
using Neat.Util;
namespace Neat.Framework
{
    #region NeatMain
    public class NeatMain
    {
        public static NeatConfig config;

        protected int innovationNumber = 1;

        List<Node> inputNodes;
        List<Node> outputNodes;
        List<Species> speciesCollection;
        // index of species current blac box containing genome belongs to
        int currentBlackBoxSpeciesIdx;
        // index of genome in current blac box wihtin a species
        int currentBlackBoxIdx;

        public NeatMain(NeatConfig config, int inputNodeCount, int outputNodeCount, int initPopulationCount) {
            NeatMain.config = config ?? new NeatConfig();
            speciesCollection = new List<Species>();
            inputNodes = new List<Node>();
            int id = 1;
            for (int count = 0; count < inputNodeCount; count++)
            {
                inputNodes.Add(new Node(id++, NodeType.INPUT));
            }
            outputNodes = new List<Node>();
            for (int count = 0; count < outputNodeCount; count++)
            {
                outputNodes.Add(new Node(id++, NodeType.OUTPUT));
            }
            currentBlackBoxIdx = 0;
            currentBlackBoxSpeciesIdx = 0;
            initPopulation(initPopulationCount);
        }

        private void initPopulation(int initPopulationCount)
        {
            // Initializing a species
            speciesCollection.Add(new Species());
            for(int count = 0; count < initPopulationCount; count++)
            {
                int minGenesCount = 1;
                int maxGenesCount = inputNodes.Count * outputNodes.Count;
                Genome genome = new Genome(inputNodes, outputNodes);
                int numOfGenes = min(minGenesCount, RandomGenerator.randomNumBefore(maxGenesCount));
                for(int i = 0; i < numOfGenes; i++)
                {
                    Node from = RandomGenerator.getRandomElementFromList(inputNodes);
                    Node to = RandomGenerator.getRandomElementFromList(outputNodes);
                    double weight = RandomGenerator.getRandomDouble();
                    // already existing genes would not be added to same genome
                    // even if it's not added, it would not be a problem since
                    // our purpose is to randomly initialize genomes
                    genome.addNewGene(from, to, weight);
                }
                chooseSpecies(genome);
            }
        }
        private int min(int a, int b) => a < b ? a : b;

        // check which species the genome belongs to. Create a new species if does not belong to any
        private void chooseSpecies(Genome genome)
        {
            bool belongs = false;
            foreach (Species species in speciesCollection)
            {
                if (species.tryAddingGenome(genome)) {
                    belongs = true;
                    break;
                }
            }
            if (!belongs)
            {
                Species newSpecies = new Species();
                newSpecies.tryAddingGenome(genome);
                speciesCollection.Add(newSpecies);
            }
        }

        // reset all static things
        public void reset()
        {
            config = new NeatConfig();
            Genome.resetInovations();
        }

        // after the whole population is done, check it's evaluation
        public void evaluateGeneration()
        {

        }

    }
    #endregion

    public class NeatBox
    {
        private Genome genome;

        public NeatBox(Genome genome)
        {
            this.genome = genome;
        }
    }


    #region species
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

        public int populationCount() => speciesPopulation.Count;

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

        public Genome getGenome(int idx) =>
            idx >= 0 && idx < speciesPopulation.Count ? speciesPopulation[idx] : null;

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
                return true;
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
    #endregion
}
