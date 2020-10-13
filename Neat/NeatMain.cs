using System.Collections.Generic;
using Neat.Config;
using Neat.Components;
using Neat.Util;
using System;
using Neat.Collection;
namespace Neat.Framework
{
    #region NeatMain
    public class NeatMain
    {
        public static NeatConfig config = new NeatConfig();
        static int generationNo = 0;
        protected int innovationNumber = 1;

        List<Node> inputNodes;
        List<Node> outputNodes;
        List<Species> speciesCollection;
        int currentSpeciesIndex = -1;
        int currentGenomeIndex = -1;

        public NeatMain(NeatConfig config, int inputNodeCount, int outputNodeCount, int initPopulationCount)
        {
            NeatMain.config = config;
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
            initPopulation(initPopulationCount);
        }

        private void initPopulation(int initPopulationCount)
        {
            // Initializing a species
            speciesCollection.Add(new Species(0));
            for (int count = 0; count < initPopulationCount; count++)
            {
                int minGenesCount = 1;
                int maxGenesCount = inputNodes.Count * outputNodes.Count;
                Genome genome = new Genome(inputNodes, outputNodes);
                int numOfGenes = max(minGenesCount, RandomGenerator.randomNumBefore(maxGenesCount));
                for (int i = 0; i < numOfGenes; i++)
                {
                    Node from = RandomGenerator.getRandomElementFromList(inputNodes);
                    Node to = RandomGenerator.getRandomElementFromList(outputNodes);
                    double weight = RandomGenerator.getRandomDouble();
                    // already existing genes would not be added to same genome
                    // even if it's not added, it would not be a problem since
                    // our purpose is to randomly initialize genomes
                    genome.addNewGene(from, to, weight);
                }
                addToFittingSpeices(genome);
            }
        }
        private int max(int a, int b) => a > b ? a : b;

        // reset all static things
        public void reset()
        {
            config = new NeatConfig();
            Genome.resetInovations();
        }

        // after the whole population is done, check it's evaluation
        private void evaluateGeneration()
        {
            generationNo++;
            int genomeVacancy = 0;
            for (int i = 0; i < speciesCollection.Count; i++)
            {
                Species s = speciesCollection[i];
                // selection
                int k = s.evaluateSpecies(generationNo);
                if (k == -1) {
                    // remove species
                    genomeVacancy += s.populationCount();
                    speciesCollection.RemoveAt(i);
                } else {
                    genomeVacancy += k;
                }
            }
            List<Genome> allGenomes = new List<Genome>();
            foreach (Species s in speciesCollection)
            {
                for (int i = 0; i < s.populationCount(); i++)
                {
                    allGenomes.Add(s.getGenome(i));
                }
            }
            // fill in removed genome spaces with new ones

            //mutation
            int mutationCount = genomeVacancy * config.percentageOfGenomesToMutate / 100;
            genomeVacancy -= mutationCount;
            for (int i = 0; i < mutationCount; i++)
            {
                Genome mutatingGenome = RandomGenerator.getRandomElementFromList<Genome>(allGenomes);
                Species speciesBeforeMutation = mutatingGenome.species;
                mutatingGenome.mutateGenome();
                // if current genome did not have a previous species assigned or is not compatible with
                // it's previous species, find and add the genome to another species
                if (speciesBeforeMutation == null ||
                    !speciesBeforeMutation.checkCompatibility(mutatingGenome))
                {
                    speciesBeforeMutation.removeGenome(mutatingGenome);
                    addToFittingSpeices(mutatingGenome);
                }
            }

            // crossover
            for (int i = 0; i < genomeVacancy; i++)
            {
                Genome parent1 = RandomGenerator.getRandomElementFromList<Genome>(allGenomes);
                Genome parent2 = parent1;
                // looping until we get a different parent
                while (parent1 == parent2)
                {
                    parent2 = RandomGenerator.getRandomElementFromList(allGenomes);
                }
                Genome newGenome = Genome.crossover(parent1, parent2);
                addToFittingSpeices(newGenome);
            }
        }
        // check which species the genome belongs to. Create a new species if does not belong to any
        private void addToFittingSpeices(Genome genome)
        {
            foreach (Species s in speciesCollection)
            {
                if (s.tryAddingGenome(genome))
                {
                    return;
                }
            }
            // if no fitting species found create new
            Species newSpecies = new Species(generationNo);
            newSpecies.tryAddingGenome(genome);
            speciesCollection.Add(newSpecies);
        }

        public NeatBox getNextNeatBox()
        {
            if (currentGenomeIndex == -1 && currentSpeciesIndex == -1)
            {
                currentSpeciesIndex = 0;
                currentGenomeIndex = 0;
            }
            else
            {
                currentGenomeIndex = (currentGenomeIndex + 1) % speciesCollection[currentSpeciesIndex].populationCount();
                if (currentGenomeIndex == 0)
                {
                    currentSpeciesIndex = (currentSpeciesIndex + 1) % speciesCollection.Count;
                }

                if (currentSpeciesIndex == 0 && currentGenomeIndex == 0)
                {
                    evaluateGeneration();
                }
            }
            Genome genome = speciesCollection[currentSpeciesIndex].getGenome(currentGenomeIndex);
            return new NeatBox(genome, generationNo, currentSpeciesIndex, currentGenomeIndex);
        }
    }
    #endregion

    public class NeatBox
    {
        private Genome genome;
        public int generation;
        public int specimen;
        public int species;
        public NeatBox(Genome genome, int gen, int species, int specimen)
        {
            this.genome = genome;
            this.generation = gen;
            this.specimen = specimen;
            this.species = species;
        }

        public List<double> calculateOutput(List<double> input) =>
            genome.calculateOutput(input);
        public void setFitnessScore(double fitness) => genome.fitnessScore = fitness;

        public List<String> getGenomeLog()
        {
            return this.genome.geneLog;
        }
    }

    public class CompareGenomes : Comparer<Genome>
    {
        public override int Compare(Genome x, Genome y)
        {
            if (x.fitnessScore > y.fitnessScore)
            {
                return -1;
            }
            else if (x.fitnessScore < y.fitnessScore)
            {
                return 1;
            }
            return 0;
        }
    }

    #region species
    
    #endregion
}
