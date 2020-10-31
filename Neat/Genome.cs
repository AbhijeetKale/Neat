using System;
using System.Collections.Generic;
using Neat.Util;
using Neat.Framework;
using System.Linq;
using Neat.Collection;

namespace Neat.Components
{
    public enum NodeType
    {
        INPUT,
        OUTPUT,
        HIDDEN
    }

    internal enum MutationType
    {
        MODIFY_WEIGHT,
        ADD_GENE,
        ADD_NODE,
        TOGGEL_GENE
    }

    internal class Node : ICloneable
    {
        public int nodeId { get; }
        public NodeType nodetype { get; }

        public Node(int nodeId, NodeType nodeType)
        {
            this.nodeId = nodeId;
            this.nodetype = nodeType;
        }

        public object Clone() => new Node(this.nodeId, this.nodetype);

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

    internal class Gene : ICloneable
    {
        public Node from { get; }
        public Node to { get; }
        public double weight { get; set; }
        public int inovationNumber { get; }
        // can be disabled
        public bool disabled { get; set; }

        public Gene(Node from, Node to, double weight, int inovationNumber)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
            this.disabled = false;
            this.inovationNumber = inovationNumber;
        }
        public object Clone() { 
            Gene g = new Gene(from, to, weight, inovationNumber);
            g.disabled = this.disabled;
            return g;
        }
    }
    internal class Genome : ICloneable
    {
        //from node -> to Node to inovationNumber
        static Dictionary<KeyValuePair<Node, Node>, int> allExistingGenes
            = new Dictionary<KeyValuePair<Node, Node>, int>();
        static int globalInnovationNumber = 1;
        static int genomeNo = 0;
        internal List<Node> inputNodes;

        internal List<Node> outputNodes;

        internal List<Node> hiddenNodes;

        private SortedList<int, Gene> genes = new SortedList<int, Gene>();

        public List<String> geneLog = new List<string>();

        internal Species species { get; set; }

        // map of Node x to all edges which end at node x
        private Dictionary<Node, List<Gene>> nodeDependencyGraph;

        private bool graphIsDirty = true;

        public double fitnessScore { get; set; }

        public int genomeId { get;  }

        internal Genome(List<Node> inputNodes, List<Node> outputNodes)
        {
            this.hiddenNodes = new List<Node>();
            this.inputNodes = inputNodes;
            this.outputNodes = outputNodes;
            fitnessScore = 0;
            graphIsDirty = true;
            species = null;
            genomeId = ++genomeNo;
            geneLog.Add("Added from public ctor");
        }

        // done just for testing
        public static void resetInovations()
        {
            globalInnovationNumber = 1;
            allExistingGenes.Clear();
            genomeNo = 0;
        }

        internal List<Gene> getGenes() => this.genes.Values.ToList<Gene>();

        private Genome(List<Gene> genesToClone, List<Node> inputNodes, List<Node> outputNodes)
        {
            this.inputNodes = inputNodes;
            this.outputNodes = outputNodes;
            this.genomeId = ++genomeNo;
            this.species = null;
            fitnessScore = 0;
            graphIsDirty = true;
            Dictionary<int, Node> hiddenNodesMap = new Dictionary<int, Node>();
            foreach (Gene g in genesToClone)
            {
                Gene gene = (Gene)g.Clone();
                this.genes.Add(gene.inovationNumber, gene);
                if (!hiddenNodesMap.ContainsKey(gene.from.nodeId)
                    && gene.from.nodetype == NodeType.HIDDEN)
                {
                    hiddenNodesMap.Add(gene.from.nodeId, gene.from);
                }
                if (!hiddenNodesMap.ContainsKey(gene.to.nodeId)
                    && gene.to.nodetype == NodeType.HIDDEN)
                {
                    hiddenNodesMap.Add(gene.to.nodeId, gene.to);
                }
            }
            this.hiddenNodes = hiddenNodesMap.Values.ToList<Node>();
        }

        internal bool addNewGene(Node from, Node to, double weight)
        {
            // TODO(abhijeet): Check for cyclic dependency in neural net before adding a new gene
            geneLog.Add("Trying to add Gene to Genome");
            // Gene validations
            if (!((inputNodes.Contains(from) || hiddenNodes.Contains(from))
                && (hiddenNodes.Contains(to) || outputNodes.Contains(to))))
            {
                throw new ArgumentException("This Gene cannot be added to this network");
            }

            KeyValuePair<Node, Node> nodePair = new KeyValuePair<Node, Node>(from, to);

            int newInovationNumber;
            Gene newGene;
            if (allExistingGenes.ContainsKey(nodePair))
            {
                newInovationNumber = allExistingGenes[nodePair];
                newGene = new Gene(from, to, weight, newInovationNumber);
                // fetch index of gene where it should be inserted
                if (this.genes.ContainsKey(newInovationNumber))
                {
                    geneLog.Add("Failed to add gene. Gene already exists in Genome");
                    return false;
                }
                this.genes.Add(newInovationNumber, newGene);
                geneLog.Add("Added previously seen gene from " + from.nodeId + " to " + to.nodeId);
            }
            else
            {
                newInovationNumber = Genome.globalInnovationNumber++;
                allExistingGenes.Add(nodePair, newInovationNumber);
                newGene = new Gene(from, to, weight, newInovationNumber);
                this.genes.Add(newGene.inovationNumber, newGene);
                geneLog.Add("Added new gene from " + from.nodeId + " to " + to.nodeId);
            }
            graphIsDirty = true;
            return true;
        }

        internal Node addHiddenNodeBetween(Gene gene)
        {
            // create node id based on the number of nodes that have been created for this structure
            // If any duplicate mutation occurs within the same generation, this will help kekep track
            // and not create a new inovation number for the duplication
            if (!this.hasGene(gene))
            {
                throw new ArgumentException("Gene passed is not present in Genome");
            }
            int totalNodes = inputNodes.Count + hiddenNodes.Count + outputNodes.Count;
            // incremental node id
            Node newNode = new Node(totalNodes + 1, NodeType.HIDDEN);
            hiddenNodes.Add(newNode);
            this.removeGene(gene.inovationNumber);
            this.addNewGene(gene.from, newNode, 1.0);
            this.addNewGene(newNode, gene.to, gene.weight);
            geneLog.Add("Added new node: " + newNode.nodeId);
            graphIsDirty = true;
            return newNode;
        }

        void removeGene(int inovationNumber)
        {
            if (this.hasGene(inovationNumber))
            {
                this.genes.Remove(inovationNumber);
                geneLog.Add("Removed Gene " + inovationNumber + " from geneome");
                graphIsDirty = true;
            }
        }

        internal Gene getGene(int inovationNumber)
        {
            Gene output;
            if (this.genes.TryGetValue(inovationNumber, out output))
            {
                return output;
            }
            return null;
        }

        internal bool hasGene(Gene gene)
        {
            return hasGene(gene.inovationNumber);
        }

        public bool hasGene(int inovationNumber) => this.genes.ContainsKey(inovationNumber);

        public static Genome crossover(Genome parent1, Genome parent2)
        {
            int i = 0, j = 0;
            List<Gene> crossoverResult = new List<Gene>();
            List<Gene> genes1 = parent1.getGenes();
            List<Gene> genes2 = parent2.getGenes();
            while (i < genes1.Count && j < genes2.Count)
            {
                Gene gene1 = genes1[i];
                Gene gene2 = genes2[j];
                // matching gene case. Choose a gene at random
                if (gene1.inovationNumber == gene2.inovationNumber)
                {
                    crossoverResult.Add((Gene)RandomGenerator.chooseAtRandom(gene1, gene2));
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
            while (i < genes1.Count && parent1.fitnessScore >= parent2.fitnessScore)
            {
                crossoverResult.Add(genes1[i++]);
            }
            while (j < genes2.Count && parent2.fitnessScore >= parent1.fitnessScore)
            {
                crossoverResult.Add(genes2[j++]);
            }
            Genome result = new Genome(crossoverResult, parent1.inputNodes, parent1.outputNodes);
            result.geneLog.Add("Generated from crossOver");
            return result;
        }
        public void mutateGenome()
        {
            int[] mutationProbabiliteis = { NeatMain.config.geneWeightChangePercentage,
                NeatMain.config.geneMutationPercentage, NeatMain.config.nodeMutationPercentage,
                NeatMain.config.disableGenePercentage};
            MutationType[] mutations = { MutationType.MODIFY_WEIGHT, MutationType.ADD_GENE
                    , MutationType.ADD_NODE, MutationType.TOGGEL_GENE};
            MutationType mutation = RandomGenerator.getElementBasedonProbablity(
                new List<MutationType>(mutations), mutationProbabiliteis);
            List<Gene> geneList = getGenes();
            geneLog.Add("Mutating Genome: " + mutation);
            switch (mutation)
            {
                case MutationType.ADD_GENE:
                    List<Node> possibleFromNodes = new List<Node>();
                    List<Node> possibleToNodes = new List<Node>();
                    foreach (Node node in inputNodes)
                    {
                        possibleFromNodes.Add(node);
                    }
                    foreach (Node node in hiddenNodes)
                    {
                        possibleFromNodes.Add(node);
                        possibleToNodes.Add(node);
                    }
                    foreach (Node node in outputNodes)
                    {
                        possibleToNodes.Add(node);
                    }
                    Node from = RandomGenerator.getRandomElementFromList<Node>(possibleFromNodes);
                    Node to = RandomGenerator.getRandomElementFromList<Node>(possibleToNodes);
                    while (from.nodeId == to.nodeId)
                    {
                        to = RandomGenerator.getRandomElementFromList<Node>(possibleToNodes);
                    }

                    double weight = RandomGenerator.getRandomDouble();
                    if (!this.addNewGene(from, to, weight))
                    {
                        // if gene to be added is already present, change weight
                        mutateRandomGeneWeight();
                    }
                    break;
                case MutationType.MODIFY_WEIGHT:
                    mutateRandomGeneWeight();
                    break;
                case MutationType.ADD_NODE:
                    Gene gene = RandomGenerator.getRandomElementFromList(geneList);
                    addHiddenNodeBetween(gene);
                    break;
                case MutationType.TOGGEL_GENE:
                    Gene r = RandomGenerator.getRandomElementFromList(geneList);
                    r.disabled = !r.disabled;
                    geneLog.Add("Toggled gene from node " + r.from.nodeId + " to node " + r.to.nodeId + " disabled = " + r.disabled);
                    break;
            }
            graphIsDirty = true;
        }
        private void mutateRandomGeneWeight()
        {
            List<Gene> geneList = getGenes();
            Gene g = RandomGenerator.getRandomElementFromList(geneList);
            if (NeatMain.config.randomWeightMutation)
            {
                g.weight = RandomGenerator.getRandomDouble();
            }
            else
            {
                g.weight += (double)RandomGenerator.chooseAtRandom(
                    NeatMain.config.weightDeltaOnMutation, -NeatMain.config.weightDeltaOnMutation);
            }
            g.weight = max(NeatMain.config.minWeight, min(NeatMain.config.maxWeight, g.weight));
            // sometimes double arithemetic screws up. So rounding off:
            g.weight = Math.Round(g.weight, 4);
            geneLog.Add("Mutating Gene" + g.inovationNumber + "Genome weight to " + g.weight);
        }
        private double min(double a, double b) => a < b ? a : b;

        private double max(double a, double b) => a > b ? a : b;

        private void calcNodeDependencyGraph()
        {
            geneLog.Add("Calculating Dependency Graph");
            this.nodeDependencyGraph = new Dictionary<Node, List<Gene>>();
            foreach (Node node in inputNodes)
            {
                nodeDependencyGraph[node] = new List<Gene>();
            }
            foreach (Node node in hiddenNodes)
            {
                nodeDependencyGraph[node] = new List<Gene>();
            }
            foreach (Node node in outputNodes)
            {
                nodeDependencyGraph[node] = new List<Gene>();
            }
            foreach (Gene gene in this.genes.Values)
            {
                if (!gene.disabled)
                {
                    nodeDependencyGraph[gene.to].Add(gene);
                }
            }
            graphIsDirty = false;
        }

        public List<double> calculateOutput(List<double> inputs)
        {
            if (inputs.Count != inputNodes.Count)
            {
                throw new ArgumentException("Number of inputs incorrect");
            }
            int totalCount = inputNodes.Count + outputNodes.Count + hiddenNodes.Count;
            // nodeId to node's activation value
            Dictionary<int, double> activationValues = new Dictionary<int, double>();
            for (int i = 0; i < inputs.Count; i++)
            {
                activationValues[inputNodes[i].nodeId] = inputs[i];
            }
            List<double> output = new List<double>();
            if (graphIsDirty)
            {
                calcNodeDependencyGraph();
            }
            foreach (Node node in outputNodes)
            {
                setActivationValue(activationValues, node);
                output.Add(Math.Round(activationValues[node.nodeId], 4));
            }
            return output;
        }

        private void setActivationValue(Dictionary<int, double> activationValues, Node node)
        {
            activationValues[node.nodeId] = 0;
            foreach (Gene gene in nodeDependencyGraph[node])
            {
                if (!activationValues.ContainsKey(gene.from.nodeId))
                {
                    setActivationValue(activationValues, gene.from);
                }
                activationValues[node.nodeId] +=
                    gene.weight * activationValues[gene.from.nodeId];
            }
        }

        public object Clone()
        {
            Genome genome = new Genome(this.getGenes(), inputNodes, outputNodes);
            genome.fitnessScore = this.fitnessScore;
            genome.geneLog.Add("Cloned Genome");
            genome.species = this.species;
            return genome;
        }
    }
}
