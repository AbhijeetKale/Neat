# Neat
NEAT (NeuroEvolution of Augmenting Topologies) is an evolutionary algorithm that creates artificial neural networks. For a detailed description of the algorithm, you should probably [read this paper](http://nn.cs.utexas.edu/downloads/papers/stanley.ec02.pdf).

## c#
An implementation of NEAT in c# with .Net framework 4.7.2 (The code can be compiled with any .Net lib, just a few functions might need changing).

### Usage
```c#
  // set up config parameters for Neat algo
  // Reference the below section to know more about parameter settings in NeatConfig
  NeatConfig config = new NeatConfig();
  int noOfIntputNodes = 4;
  int noOfOutputNodes = 2;
  // Initial population (number of neural nets)
  int initPopulation = 50;
  
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
NOTE: Always make sure that the input you are giving does not change in order at any point in time. For ex: if you are giving
      the first param as time and second as distance, do not mix them up in any of the future calls to calculate output.
## JAVA
Current implementation of Neat in java is present in the [FlappyBIrd repo](https://github.com/AbhijeetKale/FlappyBird/tree/master/bin).
The code is old and very inefficient though. I will be porting the c# algo code to java once it is finished.
