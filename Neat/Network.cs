﻿using System;
using System.Collections.Generic;
using Neat.Util;
namespace Neat.Network
{
    public enum NodeType
    {
        INPUT,
        OUTPUT,
        HIDDEN
    }

    public class Node
    {
        int nodeId;
        NodeType nodetype;

        public Node(int nodeId, NodeType nodeType)
        {
            this.nodeId = nodeId;
            this.nodetype = nodeType;
        }

        public override bool Equals(object node)
        {
            Node comparator = (Node)node;
            return comparator.nodeId == this.nodeId;
        }

        public override int GetHashCode()
        {
            return nodeId.GetHashCode();
        }
    }

    public class Gene: ICloneable
    {
        public Node from;
        public Node to;
        public double weight;
        public int inovationNumber { get; set; }
        // can be disabled
        private bool disabled { get;  set; }

        public Gene(Node from, Node to, double weight, int inovationNumber)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
            this.disabled = false;
            this.inovationNumber = inovationNumber;
        }

        public object Clone() => new Gene(from, to, weight, inovationNumber);
    }

    public class Genome
    {
        //from node -> to Node to inovationNumber
        static Dictionary<KeyValuePair<Node, Node>, int> allExistingGenes
            = new Dictionary<KeyValuePair<Node, Node>, int>();
        static int globalInnovationNumber = 1;

        private List<Node> inputNodes;

        private List<Node> outputNodes;

        private List<Node> hiddenNodes;

        private List<Gene> genes;

        public float fitnessScore { get; set; }

        public Genome(List<Node> inputNodes, List<Node> outputNodes)
        {
            this.genes = new List<Gene>();
            this.inputNodes = inputNodes;
            this.outputNodes= outputNodes;
            this.hiddenNodes = new List<Node>();
        }

        // done just for testing
        public static void resetInovations()
        {
            globalInnovationNumber = 1;
            allExistingGenes.Clear();
        }

        public List<Gene> getGenes() => this.genes;

        private Genome(List<Gene> genes)
        {
            this.genes = genes;
        }

        public void addNewGene(Node from, Node to, double weight)
        {
            // Gene validations
            if (!((inputNodes.Contains(from) || hiddenNodes.Contains(from))
                && (hiddenNodes.Contains(to) || outputNodes.Contains(to))))
            {
                throw new ArgumentException("This Gene cannot be added to this network");
            }

            KeyValuePair<Node, Node> nodePair = new KeyValuePair<Node, Node>(from, to);

            int newInovationNumber;
            Gene newGene;
            if (allExistingGenes.ContainsKey(nodePair)) {
                newInovationNumber = allExistingGenes.GetValueOrDefault(nodePair);
                newGene = new Gene(from, to, weight, newInovationNumber);
                // fetch index of gene where it should be inserted
                int idx = searchForGenePosition(newGene);
                // check if gene is already present
                if (idx >= this.genes.Count)
                {
                    this.genes.Add(newGene);
                    return;
                }
                if (this.genes[idx].inovationNumber == newInovationNumber)
                {
                    return;
                }
                // largest inovation number in all genome
                else
                {
                    this.genes.Insert(idx, newGene);
                }
            }
            else
            {
                newInovationNumber = Genome.globalInnovationNumber++;
                allExistingGenes.Add(nodePair, newInovationNumber);
                newGene = new Gene(from, to, weight, newInovationNumber);
                this.genes.Add(newGene);
            }
        }

        public Node addHiddenNodeBetween(Gene gene) {
            // create node id based on the number of nodes that have been created for this structure
            // If any duplicate mutation occurs within the same generation, this will help kekep track
            // and not create a new inovation number for the duplication
            if(!this.hasGene(gene))
            {
                throw new ArgumentException("Gene passed is not present in Genome");
            }
            Node newNode = new Node(this.genes.Count, NodeType.HIDDEN);
            hiddenNodes.Add(newNode);
            this.removeGene(gene.inovationNumber);
            this.addNewGene(gene.from, newNode, 1.0);
            this.addNewGene(newNode, gene.to, gene.weight);
            return newNode;
        }

        void removeGene(int inovationNumber)
        {
            if (this.hasGene(inovationNumber))
            {
                int idx = searchForGenePosition(inovationNumber);
                this.genes.RemoveAt(idx);
            }
        }

        public Gene getGene(int inovationNumber)
        {
            if (hasGene(inovationNumber))
            {
                return this.genes[searchForGenePosition(inovationNumber)];
            }
            return null;
        }

        public bool hasGene(Gene gene)
        {
            return hasGene(gene.inovationNumber);
        }

        public bool hasGene(int inovationNumber)
        {
            int idx = searchForGenePosition(inovationNumber);
            return genes[idx].inovationNumber == inovationNumber;
        }

        // binary search
        private int searchForGenePosition(Gene gene)
        {
            return searchForGenePosition(gene.inovationNumber);
        }

        private int searchForGenePosition(int inovationNumber)
        {
            int i = 0;
            int j = this.genes.Count - 1;
            int mid = j;
            while (i < j)
            {
                mid = i + (j - i) / 2;
                if (this.genes[mid].inovationNumber > inovationNumber)
                {
                    j = mid - 1;
                }
                else if (this.genes[mid].inovationNumber < inovationNumber)
                {
                    i = mid + 1;
                }
                else
                {
                    return mid;
                }
            }
            while (mid >= 0 && this.genes[mid].inovationNumber > inovationNumber)
            {
                mid--;
            }
            return mid + 1;
        }


        public static Genome crossover(Genome parent1, Genome parent2)
        {
            int i = 0, j = 0;
            List<Gene> crossoverResult = new List<Gene>();
            while(i < parent1.genes.Count && j < parent2.genes.Count)
            {
                Gene gene1 = parent1.genes[i];
                Gene gene2 = parent2.genes[j];
                // matching gene case. Choose a gene at random
                if (gene1.inovationNumber == gene2.inovationNumber)
                {
                    crossoverResult.Add((Gene) RandomGenerator.chooseAtRandom(gene1, gene2));
                    i++;
                    j++;
                }
                // gene 2 isa disjoint gene
                else if (gene1.inovationNumber > gene2.inovationNumber)
                {
                    // take disjoint gene if fitness of parent is better than other
                    if (parent2.fitnessScore >= parent1.fitnessScore)
                    {
                        crossoverResult.Add(gene2);
                    }
                    j++;
                }
                // gene 1 is a disjoint gene
                else if (gene1.inovationNumber < gene2.inovationNumber)
                {
                    // take disjoint gene if fitness of parent is better than other
                    if (parent2.fitnessScore <= parent1.fitnessScore)
                    {
                        crossoverResult.Add(gene1);
                    }
                    i++;
                }
            }
            while (i < parent1.genes.Count && parent1.fitnessScore >= parent2.fitnessScore)
            {
                crossoverResult.Add(parent1.genes[i++]);
            }
            while (j < parent2.genes.Count && parent2.fitnessScore >= parent1.fitnessScore)
            {
                crossoverResult.Add(parent2.genes[j++]);
            }
            return new Genome(crossoverResult);
        }

        public void mutateRandomGene()
        {

        }
    }
}
