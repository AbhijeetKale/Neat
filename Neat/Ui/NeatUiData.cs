using Neat.Components;
using Neat.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat.Ui
{
    public class NeatUiData
    {
        // adjacency list for UI data
        public List<UiEdgeData> allEdges { get; }
        public List<UiNodeData> inputLayer { get; }
        public List<UiNodeData> hiddenLayer { get; }
        public List<UiNodeData> outputLayer { get; }
        // map of internal node Id to external UiNodeDataId
        private Dictionary<int, UiNodeData> idMap;

        internal NeatUiData(Genome g) {
            allEdges = new List<UiEdgeData>();
            inputLayer = new List<UiNodeData>();
            hiddenLayer = new List<UiNodeData>();
            outputLayer = new List<UiNodeData>();
            idMap = new Dictionary<int, UiNodeData>();
            int initId = RandomGenerator.getRandomInt();
            foreach(Node n in g.inputNodes) {
                idMap.Add(n.nodeId, new UiNodeData(initId++));
                inputLayer.Add(idMap[n.nodeId]);
            }
            foreach(Node n in g.outputNodes) {
                idMap.Add(n.nodeId, new UiNodeData(initId++));
                outputLayer.Add(idMap[n.nodeId]);
            }
            foreach (Node n in g.hiddenNodes) {
                idMap.Add(n.nodeId, new UiNodeData(initId++));
                hiddenLayer.Add(idMap[n.nodeId]);
            }
            foreach(Gene gene in g.getGenes()) {
                UiNodeData fromUiNodeId = idMap[gene.from.nodeId];
                UiNodeData toUiNodeId = idMap[gene.to.nodeId];
                allEdges.Add(new UiEdgeData(fromUiNodeId, toUiNodeId, !gene.disabled));
            }
        }
    }
    // class containing ui data related to nodes
    public class UiNodeData {
        public int id { get; }
        public UiNodeData(int id) {
            this.id = id;
        }
    }
    public class UiEdgeData {
        public UiNodeData from { get; }
        public UiNodeData to { get; }
        public bool enabled { get; }

        public UiEdgeData (UiNodeData from, UiNodeData to, bool enabled) {
            this.from = from;
            this.to = to;
            this.enabled = enabled;
        }
    }
}
