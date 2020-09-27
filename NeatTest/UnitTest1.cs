using NUnit.Framework;
using Neat.Network;
using Neat.Util;
using System;
using System.Collections.Generic;
namespace NeatTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
        [TearDown]
        public void tearDown()
        {
            Genome.resetInovations();
        }
        private List<Node> getInputNodeSets()
        {
            Node[] nodes = new Node[7];
            nodes[0] = new Node(1, NodeType.INPUT);
            nodes[1] = new Node(2, NodeType.INPUT);
            nodes[2] = new Node(3, NodeType.INPUT);
            nodes[3] = new Node(4, NodeType.INPUT);
            nodes[4] = new Node(5, NodeType.INPUT);
            return new List<Node>(nodes);
        }

        private List<Node> getOutputNodeSets()
        {
            Node[] nodes = new Node[2];
            nodes[0] = new Node(6, NodeType.OUTPUT);
            nodes[1] = new Node(7, NodeType.OUTPUT);
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
            List<Node> inputNodes = getInputNodeSets();
            List<Node> outputNodes = getOutputNodeSets();
            Genome genome1 = new Genome(inputNodes, outputNodes);
            genome1.fitnessScore = 4;
            genome1.addNewGene(inputNodes[0], outputNodes[0], 1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 2);
            genome1.addNewGene(inputNodes[1], outputNodes[0], 3);
            genome1.addNewGene(inputNodes[2], outputNodes[1], 4);
            genome1.addNewGene(inputNodes[2], outputNodes[0], 5);
            Genome genome2 = new Genome(inputNodes, outputNodes);
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
            List<Node> inputNodes = getInputNodeSets();
            List<Node> outputNodes = getOutputNodeSets();
            Genome genome1 = new Genome(inputNodes, outputNodes);
            genome1.addNewGene(inputNodes[0], outputNodes[0], 1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 2);
            genome1.addNewGene(inputNodes[1], outputNodes[0], 3);
            genome1.addNewGene(inputNodes[2], outputNodes[1], 4);
            genome1.addNewGene(inputNodes[2], outputNodes[0], 5);
            Genome genome2 = new Genome(inputNodes, outputNodes);
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
            List<Node> inputNodes = getInputNodeSets();
            List<Node> outputNodes = getOutputNodeSets();
            Genome genome1 = new Genome(inputNodes, outputNodes);
            genome1.addNewGene(inputNodes[0], outputNodes[0], 1);
            genome1.addNewGene(inputNodes[0], outputNodes[1], 2);
            Genome genome2 = new Genome(inputNodes, outputNodes);
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
            List<Node> inputNodes = getInputNodeSets();
            List<Node> outputNodes = getOutputNodeSets();
            Genome genome1 = new Genome(inputNodes, outputNodes);
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