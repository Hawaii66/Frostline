using Frostline.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.World.Generation
{
    public abstract class Node
    {
        private readonly Polar _polar;
        private readonly List<Node> _edges;
        public IReadOnlyList<Node> Edges => _edges;
        public Polar Polar => _polar;

        public Node(Polar polar)
        {
            _polar = polar;
            _edges = new List<Node>();
        }

        public void AddEdge(Node node)
        {
            if (!_edges.Contains(node))
            {
                _edges.Add(node);
            }
        }
        public void RemoveEdge(Node node)
        {
            _edges.Remove(node);
        }
        public int ConnectedNodes()
        {
            HashSet<string> seen = new();
            for (int i = 0; i < _edges.Count; ++i)
            {
                Node node = _edges[i];
                seen.Add(node._polar.ToCartesian().ToString());
            }
            return seen.Count;
        }

        public abstract Color GetColor();
        public abstract bool AllowsJunctionConnections();
    }
}