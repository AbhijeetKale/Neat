using System;
namespace Neat.Config
{
    public class NeatConfig
    {
		//speciation
		public double delta_Threshhold = 2.0;
		public double delta_C1 = 0.7;
		public double delta_C2 = 0.7;
		public double delta_C3 = 0.3;
		public int population_Normalization_Threshold = 20;
		//speciation

		//crossover
		public double enableDisableFlagProbablity = 75;
		public double matingCrossoverProportion = 50;
		//crossover

		//Selection
		public double populationSruvivalPercentage = 50;
		public int minimumPopulation = 5;
		//Selection

		//mutation
		public int weightMutationProbability = 60;
		public int geneMutationProbability = 20;
		public int nodeMutationProbability = 20;
		public int geneWeightChangeProbability = 10;
		public bool randomWeightMutation = false;
		public double weightDeltaOnMutation = 0.1;
		//mutation

		//Gene Params
		public double minWeight = -1;
		public double maxWeight = 1;
		//Gene Params

	}
}
