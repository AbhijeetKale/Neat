using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading.Tasks;
using Neat.Components;
using Neat.Framework;
using Neat.Collection;

namespace NeatTest
{
    class SpeciesTest
    {
        Genome genome1;
        Genome genome2;
        List<Node> inputNodes;
        List<Node> outputNodes;
        int intputNodeCount = 5;
        int outputNodeCount = 2;

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
            List<Node> nodes = new List<Node>();
            for (int count = 0; count < intputNodeCount; count++) {
                nodes.Add(new Node(count, NodeType.INPUT));
            }
            return nodes;
        }

        private List<Node> getOutputNodeSets()
        {
            List<Node> nodes = new List<Node>();
            for (int count = intputNodeCount; count < outputNodeCount; count++)
            {
                nodes.Add(new Node(count, NodeType.OUTPUT));
            }
            return nodes;
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
            if (!species.tryAddingGenome(genome1))
            {
                Assert.Fail();
                return;
            }
            bool check = species.checkCompatibility(genome2);
            Assert.AreEqual(check, true);
            Assert.Pass();
        }
    }
}
