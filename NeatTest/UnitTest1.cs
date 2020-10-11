using NUnit.Framework;
using Neat.Components;
using Neat.Framework;
using Neat.Config;
using System;
using System.Collections.Generic;
using Neat.Util;

namespace NeatTest
{
    public class Tests
    {
        Genome genome1;
        Genome genome2;
        List<Node> inputNodes;
        List<Node> outputNodes;

        [SetUp]
        public void Setup()
        {
            inputNodes = getInputNodeSets();
            outputNodes = getOutputNodeSets();
            genome1 = new Genome(inputNodes, outputNodes);
            genome2 = new Genome(inputNodes, outputNodes);
        }
        [TearDown]
        public void tearDown()
        {
            Genome.resetInovations();
        }
        private List<Node> getInputNodeSets()
        {
            Node[] nodes = new Node[5];
            nodes[0] = new Node(0, NodeType.INPUT);
            nodes[1] = new Node(1, NodeType.INPUT);
            nodes[2] = new Node(2, NodeType.INPUT);
            nodes[3] = new Node(3, NodeType.INPUT);
            nodes[4] = new Node(4, NodeType.INPUT);
            return new List<Node>(nodes);
        }

        private List<Node> getOutputNodeSets()
        {
            Node[] nodes = new Node[2];
            nodes[0] = new Node(5, NodeType.OUTPUT);
            nodes[1] = new Node(6, NodeType.OUTPUT);
            return new List<Node>(nodes);
        }


        private void checkGenomeResult(Genome genome, int[] result)
        {
            int idx = 0;
            foreach (Gene g in genome.getGenes())
            {
                Assert.AreEqual(result[idx++], g.inovationNumber);
            }
        }
        [Test]
        public void CreateGenomeTest()
        {
            genome1.fitnessScore = 4;
            genome1.addNewGene(inputNodes[0], outputNodes[0], 1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 2);
            genome1.addNewGene(inputNodes[1], outputNodes[0], 3);
            genome1.addNewGene(inputNodes[2], outputNodes[1], 4);
            genome1.addNewGene(inputNodes[2], outputNodes[0], 5);
            genome2.fitnessScore = 10;
            genome2.addNewGene(inputNodes[0], outputNodes[1], -1);
            genome2.addNewGene(inputNodes[1], outputNodes[0], -2);
            genome2.addNewGene(inputNodes[1], outputNodes[1], -3);
            genome2.addNewGene(inputNodes[2], outputNodes[1], -4);
            genome2.addNewGene(inputNodes[2], outputNodes[0], -5);
            int[] result1 = { 1, 2, 3, 4, 5 };
            int[] result2 = { 2, 3, 4, 5, 6 };
            checkGenomeResult(genome1, result1);
            checkGenomeResult(genome2, result2);
            Assert.Pass();
        }
        [Test]
        public void CrossoverTest2()
        {
            genome1.addNewGene(inputNodes[0], outputNodes[0], 1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 2);
            genome1.addNewGene(inputNodes[1], outputNodes[0], 3);
            genome1.addNewGene(inputNodes[2], outputNodes[1], 4);
            genome1.addNewGene(inputNodes[2], outputNodes[0], 5);
            genome2.addNewGene(inputNodes[0], outputNodes[1], -1);
            genome2.addNewGene(inputNodes[1], outputNodes[0], -2);
            genome2.addNewGene(inputNodes[1], outputNodes[1], -3);
            genome2.addNewGene(inputNodes[2], outputNodes[1], -4);
            genome2.addNewGene(inputNodes[2], outputNodes[0], -5);

            // equal fitness score
            genome1.fitnessScore = 10;
            genome2.fitnessScore = 10;
            Genome child = Genome.crossover(genome1, genome2);
            int[] result = { 1, 2, 3, 4, 5, 6 };
            checkGenomeResult(child, result);

            // genome 1 lower fitness score
            genome1.fitnessScore = 4;
            genome2.fitnessScore = 10;
            child = Genome.crossover(genome1, genome2);
            int[] result2 = { 2, 3, 4, 5, 6};
            checkGenomeResult(child, result2);

            // genome 2 lower fitness score
            genome1.fitnessScore = 10;
            genome2.fitnessScore = 4;
            child = Genome.crossover(genome1, genome2);
            int[] result3 = { 1, 2, 3, 4, 5 };
            checkGenomeResult(child, result3);

            Assert.Pass();
        }
        [Test]
        public void CrossoverTest3()
        {
            genome1.addNewGene(inputNodes[0], outputNodes[0], 1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 2);
            genome2.addNewGene(inputNodes[1], outputNodes[0], -2);
            genome2.addNewGene(inputNodes[1], outputNodes[1], -3);

            // equal fitness score
            genome1.fitnessScore = 10;
            genome2.fitnessScore = 10;
            Genome child = Genome.crossover(genome1, genome2);
            int[] result = { 1, 2, 3, 4};
            checkGenomeResult(child, result);

            // genome 1 lower fitness score
            genome1.fitnessScore = 4;
            genome2.fitnessScore = 10;
            child = Genome.crossover(genome1, genome2);
            int[] result2 = { 3, 4};
            checkGenomeResult(child, result2);

            // genome 2 lower fitness score
            genome1.fitnessScore = 10;
            genome2.fitnessScore = 4;
            child = Genome.crossover(genome1, genome2);
            int[] result3 = { 1, 2 };
            checkGenomeResult(child, result3);

            Assert.Pass();
        }

        [Test]
        public void addHiddenNodeTest()
        {
            genome1.fitnessScore = 4;
            genome1.addNewGene(inputNodes[0], outputNodes[0], 1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 2);
            genome1.addNewGene(inputNodes[1], outputNodes[0], 3);
            genome1.addNewGene(inputNodes[2], outputNodes[1], 4);
            genome1.addNewGene(inputNodes[2], outputNodes[0], 5);
            Gene g = genome1.getGene(4);
            Node newNode = genome1.addHiddenNodeBetween(g);
            int[] result = { 1, 2, 3, 5, 6, 7};
            checkGenomeResult(genome1, result);
            Gene newGene1 = genome1.getGene(6);
            Assert.AreEqual(newGene1.from, g.from);
            Assert.AreEqual(newGene1.to, newNode);
            Assert.AreEqual(newGene1.weight, 1.0);

            Gene newGene2 = genome1.getGene(7);
            Assert.AreEqual(newGene2.from, newNode);
            Assert.AreEqual(newGene2.to, g.to);
            Assert.AreEqual(newGene2.weight, g.weight);
        }
        [Test]
        public void weightMutationTest()
        {
            genome1.fitnessScore = 4;
            genome1.addNewGene(inputNodes[0], outputNodes[0], 0.1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 0.2);
            genome1.addNewGene(inputNodes[1], outputNodes[0], 0.3);
            genome1.addNewGene(inputNodes[2], outputNodes[1], 0.4);
            genome1.addNewGene(inputNodes[2], outputNodes[0], 0.5);

            // checking weight mutation
            NeatMain.config.geneWeightChangeProbability = 100;
            NeatMain.config.geneMutationProbability = 0;
            NeatMain.config.nodeMutationProbability = 0;
            NeatMain.config.disableGeneProbability = 0;
            NeatMain.config.randomWeightMutation = false;
            genome1.mutateGenome();
            // weight mutation of +- weightdelta in config should occur in one gene
            int weightMutatedCount = 0;
            double[] originalWeights = { 0.1, 0.2, 0.3, 0.4, 0.5 };
            int idx = 0;
            foreach(Gene gene in genome1.getGenes())
            {
                if (originalWeights[idx] != gene.weight)
                {
                    double delta = Math.Round(gene.weight - originalWeights[idx], 4);
                    if (!(delta == NeatMain.config.weightDeltaOnMutation
                        || delta == -NeatMain.config.weightDeltaOnMutation))
                    {
                       Assert.Fail();
                    }
                    weightMutatedCount++;
                }
                idx++;
            }
            Assert.AreEqual(weightMutatedCount, 1);
            Assert.Pass();
        }

        [Test]
        public void weightMutationTest2()
        {
            genome1.fitnessScore = 4;
            genome1.addNewGene(inputNodes[0], outputNodes[0], 0.1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 0.2);
            genome1.addNewGene(inputNodes[1], outputNodes[0], 0.3);
            genome1.addNewGene(inputNodes[2], outputNodes[1], 0.4);
            genome1.addNewGene(inputNodes[2], outputNodes[0], 0.5);

            // checking weight mutation
            NeatMain.config.geneWeightChangeProbability = 100;
            NeatMain.config.geneMutationProbability = 0;
            NeatMain.config.nodeMutationProbability = 0;
            NeatMain.config.disableGeneProbability = 0;
            NeatMain.config.randomWeightMutation = true;
            genome1.mutateGenome();
            // weight mutation of +- weightdelta in config should occur in only one gene
            int weightMutatedCount = 0;
            double[] originalWeights = { 0.1, 0.2, 0.3, 0.4, 0.5 };
            int idx = 0;
            foreach (Gene gene in genome1.getGenes())
            {
                if (originalWeights[idx++] != gene.weight)
                {
                    weightMutatedCount++;
                }
            }
            Console.WriteLine(weightMutatedCount);
            Assert.AreEqual(weightMutatedCount, 1);
            Assert.Pass();
        }

        [Test]
        public void speciesTest()
        {
            genome1.addNewGene(inputNodes[0], outputNodes[0], 0.1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 0.2);
            genome1.addNewGene(inputNodes[1], outputNodes[0], 0.3);
            genome1.addNewGene(inputNodes[2], outputNodes[1], 0.4);
            genome1.addNewGene(inputNodes[2], outputNodes[0], 0.5);
            genome2.addNewGene(inputNodes[0], outputNodes[1], -0.1);
            genome2.addNewGene(inputNodes[1], outputNodes[0], -0.2);
            genome2.addNewGene(inputNodes[1], outputNodes[1], -0.3);
            genome2.addNewGene(inputNodes[2], outputNodes[1], -0.4);
            genome2.addNewGene(inputNodes[2], outputNodes[0], -0.5);
            Species species = new Species(0);
            if(!species.tryAddingGenome(genome1))
            {
                Assert.Fail();
                return;
            }
            bool check = species.checkCompatibility(genome2);
            Assert.AreEqual(check, true);
            Assert.Pass();
        }

        [Test]
        public void speciesSortingTest()
        {
            genome1.addNewGene(inputNodes[0], outputNodes[0], 0.1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 0.2);
            genome1.addNewGene(inputNodes[1], outputNodes[0], 0.3);
            genome1.addNewGene(inputNodes[2], outputNodes[1], 0.4);
            genome1.addNewGene(inputNodes[2], outputNodes[0], 0.5);
            genome2.addNewGene(inputNodes[0], outputNodes[1], -0.1);
            genome2.addNewGene(inputNodes[1], outputNodes[0], -0.2);
            genome2.addNewGene(inputNodes[1], outputNodes[1], -0.3);
            genome2.addNewGene(inputNodes[2], outputNodes[1], -0.4);
            genome2.addNewGene(inputNodes[2], outputNodes[0], -0.5);
            genome1.fitnessScore = 10;
            genome2.fitnessScore = 29;
            List<Genome> genomes = new List<Genome>();
            genomes.Add(genome1);
            genomes.Add(genome2);
            CompareGenomes compare = new CompareGenomes();
            genomes.Sort(compare);
            Assert.AreEqual(genomes[0], genome2);
            Assert.AreEqual(genomes[1], genome1);
            Assert.Pass();
        }

        [Test]
        public void calculateOutputTest()
        {
            genome1.fitnessScore = 4;
            genome1.addNewGene(inputNodes[0], outputNodes[0], 0.1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 0.2);
            genome1.addNewGene(inputNodes[1], outputNodes[0], 0.3);
            genome1.addNewGene(inputNodes[2], outputNodes[1], 0.4);
            genome1.addNewGene(inputNodes[2], outputNodes[0], 0.5);
            Gene g = genome1.getGene(4);
            Node newNode = genome1.addHiddenNodeBetween(g);
            double[] input = { 1, 2, 3, 4, 5};
            double[] result = { 2.2, 1.4};
            List<double> output = genome1.calculateOutput(new List<double>(input));
            for(int i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(result[i], output[i]);
            }
            Assert.Pass();
        }

        [Test]
        public void MainTest() {
            int no_generations = 15;
            int inputCount = 5;
            int initPopulation = 25;
            NeatMain neatAlgo = new NeatMain(new NeatConfig(), inputCount, 3, initPopulation);
            for(int count = 0; count < no_generations; count++) {
                for(int counter = 0; counter < initPopulation; counter++) {
                    NeatBox neatBox = neatAlgo.getNextNeatBox();
                    List<double> randList = new List<double>();
                    for (int i = 0; i < inputCount; i++) {
                        randList.Add(RandomGenerator.getRandomDouble());
                    }
                    neatBox.calculateOutput(randList);
                    neatBox.setFitnessScore(RandomGenerator.getRandomDouble());
                }
            }
            Assert.Pass();
        }

        void printGenome(Genome genome)
        {
            foreach(Gene gene in genome.getGenes())
            {
                Console.Write(gene.inovationNumber + " ");
            }
            Console.WriteLine();
        }
    }
}