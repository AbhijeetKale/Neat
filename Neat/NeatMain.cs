using System.Collections.Generic;
using Neat.Config;
using Neat.Components;
using Neat.Util;
using System.Diagnostics.CodeAnalysis;

namespace Neat.Framework
{
    #region NeatMain
    public class NeatMain
    {
        public static NeatConfig config = new NeatConfig();

        protected int innovationNumber = 1;

        List<Node> inputNodes;
        List<Node> outputNodes;
        List<Species> speciesCollection;
        int currentSpeciesIndex = -1;
        int currentGenomeIndex = -1;

        public NeatMain(NeatConfig config, int inputNodeCount, int outputNodeCount, int initPopulationCount) {
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
            speciesCollection.Add(new Species());
            for(int count = 0; count < initPopulationCount; count++)
            {
                int minGenesCount = 1;
                int maxGenesCount = inputNodes.Count * outputNodes.Count;
                Genome genome = new Genome(inputNodes, outputNodes);
                int numOfGenes = max(minGenesCount, RandomGenerator.randomNumBefore(maxGenesCount));
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
        private int max(int a, int b) => a > b ? a : b;

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
        private void evaluateGeneration()
        {
            int genomeVacancy = 0;
            foreach(Species s in speciesCollection)
            {
                // selection
                genomeVacancy += s.selection();
            }
            List<Genome> allGenomes = new List<Genome>();
            foreach(Species s in speciesCollection)
            {
                for (int i = 0; i < s.populationCount(); i++)
                {
                    allGenomes.Add(s.getGenome(i));
                }
            }
            // fill in removed genome spaces with new ones

            //mutation
            int mutationCount = allGenomes.Count * config.percentageOfGenomesToMutate;
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
            for(int i = 0; i < genomeVacancy; i++)
            {
                Genome parent1 = RandomGenerator.getRandomElementFromList<Genome>(allGenomes);
                Genome parent2 = parent1;
                // looping until we get a different parent
                while(parent1 == parent2)
                {
                    parent2 = RandomGenerator.getRandomElementFromList(allGenomes);
                }
                Genome newGenome = Genome.crossover(parent1, parent2);
                addToFittingSpeices(newGenome);
            }
        }

        private void addToFittingSpeices(Genome genome)
        {
            bool foundSpecies = false;
            foreach (Species s in speciesCollection)
            {
                if (s.tryAddingGenome(genome))
                {
                    foundSpecies = true;
                    break;
                }
            }
            if (!foundSpecies)
            {
                Species s = new Species();
                s.tryAddingGenome(genome);
                speciesCollection.Add(s);
            }
        }

        public NeatBox getNextNeatBox()
        {
            currentSpeciesIndex = (currentSpeciesIndex + 1) % speciesCollection.Count;
            currentGenomeIndex = (currentGenomeIndex + 1) % speciesCollection[currentSpeciesIndex].populationCount();
            if (currentSpeciesIndex ==0 && currentGenomeIndex == 0)
            {
                evaluateGeneration();
            }
            Genome genome = speciesCollection[currentSpeciesIndex].getGenome(currentGenomeIndex);
            return new NeatBox(genome);
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

        public List<double> calculateOutput(List<double> input) =>
            genome.calculateOutput(input);
        public void setFitnessScore(double fitness) => genome.fitnessScore = fitness;

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

        // returns total genomes that have been removed as the selection process
        public int selection()
        {
            int tobeRemoved = speciesPopulation.Count * NeatMain.config.populationSruvivalPercentagePerSpecies / 100;
            int maxPossibleRemovals = speciesPopulation.Count - NeatMain.config.minimumPopulationPerSpecies;
            tobeRemoved = tobeRemoved > maxPossibleRemovals ? maxPossibleRemovals : tobeRemoved;
            speciesPopulation.Sort(new CompareGenomes());
            if (tobeRemoved > 0)
            {
                int idx = speciesPopulation.Count - tobeRemoved;
                removeGenomeFrom(idx);
            }
            return tobeRemoved > 0 ? tobeRemoved : 0;
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
                genome.species = this;
                setRandomRepresentative();
                return true;
            }
            return false;
        }

        public void removeGenome(Genome genome) {
            genome.species = null;
            speciesPopulation.Remove(genome);
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
            while (i++ < genome.getGenes().Count)
            {
                excessGeneCount++;
            }
            while (j++ < representativeGenome.getGenes().Count)
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
