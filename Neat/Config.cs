﻿using System;
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
		//speciation

		//crossover
		public double enableDisableFlagProbablity = 75;
		public double matingCrossoverProportion = 50;
		//crossover

		//Selection
		public double populationSruvivalPercentage = 50;
		public int minimumPopulation = 200;
		//Selection

		//mutation
		public int geneWeightChangeProbability = 50;
		public int geneMutationProbability = 25;
		public int nodeMutationProbability = 25;
		public bool randomWeightMutation = false;
		public double weightDeltaOnMutation = 0.1;
		//mutation

		//Gene Params
		public double minWeight = -1;
		public double maxWeight = 1;
		//Gene Params

	}
}
