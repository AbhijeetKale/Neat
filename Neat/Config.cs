using System;
namespace Neat.Config
{
    public class NeatConfig
    {
		//speciation
		public double delta_Threshhold = 0.75;
		// eexcess genes weight
		public double delta_C1 = 1.0;
		// disjoint genes weight
		public double delta_C2 = 0.5;
		// avg weight diff weight
		public double delta_C3 = 0;
		public int gene_Normalization_Threshold = 1;
		public int numOfGenForSpeciesEval = 7;
		// min proportion of max score that species should have during evaluation
		public double minRequiredScoreToMaxScore = 0.3;
		//speciation

		//crossover
		public double enableDisableFlagProbablity = 75;
		//crossover

		//Selection
		public int populationSruvivalPercentagePerSpecies = 50;
		public int minimumPopulationPerSpecies = 5;
		//Selection

		//mutation
		public int percentageOfGenomesToMutate = 10;
		// the sum of percentages below should be excatly 100
		// otherwise the lib will throw an error
		public int geneWeightChangePercentage = 40;
		public int geneMutationPercentage= 25;
		public int nodeMutationPercentage = 25;
		public int disableGenePercentage = 10;
		// randomly assign any value for weight mutation of gene
		public bool randomWeightMutation = false;
		// Increase/Decrease of weight when randomWeightMutation is false
		// for weight mutation
		public double weightDeltaOnMutation = 0.1;
		//mutation

		//Gene Params
		public double minWeight = -1;
		public double maxWeight = 1;
		//Gene Params

	}
}
