# Neat
NEAT (NeuroEvolution of Augmenting Topologies) is an evolutionary algorithm that creates artificial neural networks. For a detailed description of the algorithm, you should probably [read this paper](http://nn.cs.utexas.edu/downloads/papers/stanley.ec02.pdf).

# c#
An implementation of NEAT in c# with .Net framework 4.7.2 (The code can be compiled with any .Net lib, just a few functions might need changing).

### Usage
```c#
  using Neat.framework;
  using Neat.config;
  
  // set up config parameters for Neat algo
  // Reference the below section to know more about parameter settings in NeatConfig
  NeatConfig config = new NeatConfig();
  int noOfIntputNodes = 4;
  int noOfOutputNodes = 2;
  // Initial population (number of neural nets)
  int initPopulation = 50;
  

  // set config params accordingly
  config.populationSruvivalPercentagePerSpecies = 30;

// instantiate the NeatMain class before you begin
  NeatMain.initInstance(config, noOfIntputNodes, noOfOutputNodes);
  
  // your code
  
  // the below line will get you one of the Genomes (neural net) from the entire population
  while() {
    NeatBox box = NeatMain.getInstance().getNextNeatBox();
    playGame(box);
  }

  void playGame(NeatBox box) {
    // prepare input that is to be given to the genome (neural net) to get a list of outputs
    // choosing which one of these outputs to choose and use is left to the user's discretion
    List<double> intput = new List<double>();

    // your code

    List<double> outputs = box.calculateOutput(input);
    
    // apply output, measure and set how well the current NeatBox (Genome) has performed
    box.setFitnessScore(fitnessScore);
  }
```

NOTE: Always make sure that the input you are giving does not change in order at any point in time. For ex: if you are giving
      the first param as time and second as distance, do not mix them up in any of the future calls to calculate output.

### Config Parameters
  Config Param Variable name | Description | Accepted Values | Default Vlaues |
------------ | ------------- | ------------- | ------------- |
double delta_Threshhold  | maximum allowed variation in different genomes that belongs to the same species | Any | 0.75 |
double delta_C1 | Weight of variation in excess genes between two genomes. (Used to compute variation between genomes) | Any | 1
double delta_C2 | Weight of variation in disjoint genes between two genomes. (Used to compute variation between genomes) | Any | 0.5
double delta_C3 | Weight of variation in weights of matching genes between two genomes. (Used to compute variation between genomes) | Any | 0
int gene_Normalization_Threshold | min number of gnenes required in any one genome to normalize the variation between two genomes | Any | 1
int numOfGenForSpeciesEval | number of generations before a species is evaluated to other species. If found weak, the species will be removed from the population | Any | 7
double minRequiredScoreToMaxScore | This parameter helps decide whether the species is weak or strong by comparing it's highest fitness score with the product of minRequiredScoreToMaxScore and overall maximum fitness score. | 0.0 to 1.0 | 0.3
int populationSruvivalPercentagePerSpecies | The percentage of top genomes (with best fitness score) within a species that sruvive and go into the next generation.
This is per species |  0 to 100 | 50
int minimumPopulationPerSpecies | The minnimum population of a species. No genomes will be removed if the population of a species go below the value of this parameter/ | > 1 | 5
int percentageOfGenomesToMutate | The percentage of surviving genomes to mutate and add to the next gen after a whole generation is run. | 0 to 100 | 10
int geneWeightChangePercentage  | Chance that a mutation will result in a change in weight changes within a gene. | 0 to 100 | 40
int geneMutationPercentage | Chance that a mutation will result in a new gene (connection) being added to the genome (neural net) | 0 - 100 | 25
int nodeMutationPercentage | Chance that a mutation will result in a new node being added to the genome (neural net) | 0 - 100 | 25
int disableGenePercentage  | Chance that a mutation will result in an existing gene (connection) is disabled in the genome (neural net) | 0 - 100 | 10
bool randomWeightMutation  | When a weight is mutatted for a gene assign it a weight randomly. | true, false | false
double weightDeltaOnMutation | If randomWeightMutation is not set, a weight mutation will occur with the addition/subtraction of this value | Any | 0.1
double minWeight | Minimum weight that can be assigned to a gene | Any | -1
double maxWeight Maximum weight that can be assigned to a gene | Any | 1

NOTE: The sum of geneWeightChangePercentage, geneMutationPercentage, nodeMutationPercentage, disableGenePercentage should be exactly 100.

NOTE:  The current version still has a few bugs. I will try to correct it as soon as possible. 
   


# JAVA
Current implementation of Neat in java is present in the [FlappyBIrd repo](https://github.com/AbhijeetKale/FlappyBird/tree/master/bin).
The code is old and very inefficient though. I will be porting the c# algo code to java once it is finished.
