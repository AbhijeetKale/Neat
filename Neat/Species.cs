using System;
using System.Collections.Generic;
using System.Text;
using Neat.Components;
using Neat.Framework;
using Neat.Util;

namespace Neat.Collection
{
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
    public class Species
    {
        private List<Genome> speciesPopulation;
        // update this whenever the species is evaluated
        private int lastGenOfSpeciesEval;
        // best score when last species eval happened
        private double bestFitnessScoreofLastEval;
        // a radndomly chosen specimen to compare other sample genomes with
        private Genome representativeGenome = null;

        public Species(int generationNo)
        {
            lastGenOfSpeciesEval = generationNo;
            speciesPopulation = new List<Genome>();
            representativeGenome = null;
        }

        public String toString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Genome genome in this.speciesPopulation)
            {
                List<Gene> genes = genome.getGenes();
                StringBuilder geneSb = new StringBuilder();
                foreach (Gene gene in genes)
                {
                    geneSb.Append(gene.inovationNumber);
                    geneSb.Append(", ");
                }
                sb.Append(geneSb.ToString());
                sb.Append("\n");
            }
            return sb.ToString();
        }
        // Species evaluation when one generation is completed
        // function returns number of genomes removed function
        // returns -1 if species is to be deleted
        public int evaluateSpecies(int currentGen, double maxScoreInCurrGen)
        {
            // if best score of species is less than pervious evaluation's score delete species
            if (lastGenOfSpeciesEval - currentGen > NeatMain.config.numOfGenForSpeciesEval)
            {
                double thresholdScore =
                    NeatMain.config.minimumPopulationPerSpecies * maxScoreInCurrGen;
                double currentBestScore = getMaxScoreInSpeices();
                if (currentBestScore < bestFitnessScoreofLastEval ||
                currentBestScore < thresholdScore)
                {
                    return -1;
                }
                bestFitnessScoreofLastEval = currentBestScore;
                this.lastGenOfSpeciesEval = currentGen;
            }
            // TODO (abhijeet): Figure out a way to increase population above min
            //  species population.
            return selection();
        }

        public double getMaxScoreInSpeices()
        {
            double max = 0;
            foreach (Genome g in speciesPopulation)
            {
                max = max < g.fitnessScore ? g.fitnessScore : max;
            }
            return max;
        }

        // returns total genomes that have been removed as the selection process
        private int selection()
        {
            int tobeRemoved = speciesPopulation.Count * (100 - NeatMain.config.populationSruvivalPercentagePerSpecies) / 100;
            int maxPossibleRemovals = speciesPopulation.Count - NeatMain.config.minimumPopulationPerSpecies;
            tobeRemoved = tobeRemoved > maxPossibleRemovals ? maxPossibleRemovals : tobeRemoved;
            if (tobeRemoved > 0)
            {
                speciesPopulation.Sort(new CompareGenomes());
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

        public void removeGenome(Genome genome)
        {
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
            List<Gene> genomeGenes = genome.getGenes();
            List<Gene> repGenes = representativeGenome.getGenes();
            while (i < genomeGenes.Count && j < repGenes.Count)
            {
                Gene gene1 = genomeGenes[i];
                Gene gene2 = repGenes[j];
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
            while (i++ < genomeGenes.Count)
            {
                excessGeneCount++;
            }
            while (j++ < repGenes.Count)
            {
                excessGeneCount++;
            }
            double delta = NeatMain.config.delta_C1 * (double)excessGeneCount
                            + NeatMain.config.delta_C2 * (double)disjointGenes;
            int maxGeneCount = repGenes.Count > genomeGenes.Count
                                ? repGenes.Count : genomeGenes.Count;
            if (maxGeneCount > NeatMain.config.gene_Normalization_Threshold)
            {
                delta = delta / maxGeneCount;
            }
            delta += NeatMain.config.delta_C3 * avgWeightDifference;
            return delta >= NeatMain.config.delta_Threshhold ? false : true;
        }
        private double abs(double a) => a < 0 ? -a : a;
    }
}
