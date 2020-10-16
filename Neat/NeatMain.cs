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
        List<Node> inputNodes;
        List<Node> outputNodes;
        List<Species> speciesCollection;
        int currentSpeciesIndex = -1;
        int currentGenomeIndex = -1;
        static NeatMain instance;

        private NeatMain(NeatConfig config, int inputNodeCount, int outputNodeCount,
                            int initPopulationCount)
        {
            configSanity(config);
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

        public static void initInstance(NeatConfig config, int inputNodeCount,
            int outputNodeCount, int initPopulationCount) {
            if (instance == null)
            {
                instance = new NeatMain(config, inputNodeCount, outputNodeCount, initPopulationCount);
            }
        }

        public static NeatMain getInstance() => instance;

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
            // find a better way to find best score in generation
            double overallMaxScore = speciesCollection[0].getMaxScoreInSpeices();
            for (int i = 1; i < speciesCollection.Count; i++)
            {
                double score = speciesCollection[i].getMaxScoreInSpeices();
                overallMaxScore = overallMaxScore < score ? score : overallMaxScore;
            }
            generationNo++;
            int genomeVacancy = 0;
            for (int i = 0; i < speciesCollection.Count; i++)
            {
                Species s = speciesCollection[i];
                // selection
                genomeVacancy += s.evaluateSpecies(generationNo, overallMaxScore);
                if (s.populationCount() == 0) {
                    // remove species
                    speciesCollection.RemoveAt(i);
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
                Genome mutatingGenome = (Genome)RandomGenerator
                    .getRandomElementFromList<Genome>(allGenomes).Clone();
                Species speciesBeforeMutation = mutatingGenome.species;
                mutatingGenome.mutateGenome();
                // if current genome did not have a previous species assigned or is not compatible with
                // it's previous species, find and add the genome to another species
                addToFittingSpeices(mutatingGenome);
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
        private void configSanity(NeatConfig config)
        {
            if (config.minRequiredScoreToMaxScore > 1.0
                || config.minRequiredScoreToMaxScore < 0.0)
                throw new NeatConfigException("minRequiredScoreToMaxScore needs to be between 0 <-> 1");
            if (config.populationSruvivalPercentagePerSpecies > 100
                || config.populationSruvivalPercentagePerSpecies < 0)
                throw new NeatConfigException("populationSruvivalPercentagePerSpecies needs to be between 0 <-> 100");
            if (config.percentageOfGenomesToMutate > 100
                || config.percentageOfGenomesToMutate < 0)
                throw new NeatConfigException("percentageOfGenomesToMutate needs to be between 0 <-> 100");
            int totalPercentage = config.geneWeightChangePercentage + config.geneMutationPercentage
                + config.nodeMutationPercentage + config.disableGenePercentage;
            if (totalPercentage != 100)
                throw new NeatConfigException("Total percentage sum of " +
                " geneWeightChangePercentage, geneMutationPercentage, nodeMutationPercentage," +
                " disableGenePercentage needs to be between 0 <-> 100");
            if (config.weightDeltaOnMutation > 1.0
                || config.weightDeltaOnMutation < 0.0)
                throw new NeatConfigException("weightDeltaOnMutation needs to be between 0 <-> 1");
        }
    }
    public class NeatConfigException : Exception
    {
        public NeatConfigException(String err) : base(err) { }
    }
    #endregion

    public class NeatBox
    {
        private Genome genome;
        public int generation;
        public int specimen;
        public int species;

        public double getFitness() => genome.fitnessScore;

        public int getGenomeId() => genome.genomeId;

        internal NeatBox(Genome genome, int gen, int species, int specimen)
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
}
