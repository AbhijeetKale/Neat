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
		public int geneWeightChangeProbability = 40;
		public int geneMutationProbability = 25;
		public int nodeMutationProbability = 25;
		public int disableGeneProbability = 10;
		public bool randomWeightMutation = false;
		public double weightDeltaOnMutation = 0.1;
		//mutation

		//Gene Params
		public double minWeight = -1;
		public double maxWeight = 1;
		//Gene Params

	}
}
