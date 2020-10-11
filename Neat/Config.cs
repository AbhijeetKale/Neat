using System;
namespace Neat.Config
{
    public class NeatConfig
    {
		//speciation
		public double delta_Threshhold = 4.0;
		public double delta_C1 = 0.7;
		public double delta_C2 = 0.6;
		public double delta_C3 = 0.2;
		public int population_Normalization_Threshold = 20;
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
