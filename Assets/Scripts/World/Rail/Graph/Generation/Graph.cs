
using Frostline.Core;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.World.Generation
{
    public class Graph
    {
        private readonly List<Node> _nodes;
        public IReadOnlyList<Node> Nodes => _nodes;
        private readonly List<Edge> _edges;
        public IReadOnlyList<Edge> Edges => _edges;
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }
        public Vector2Int NegativeOffset { get; private set; }
        private readonly GraphSettings _settings;

        public Graph(GraphSettings settings)
        {
            _nodes = new List<Node>();
            _edges = new List<Edge>();
            _settings = settings;
        }

        private bool TryAddNode(Node node, Node parent)
        {
            if (IsTooClose(node.Polar))
            {
                return false;
            }

            _nodes.Add(node);

            if (parent != null)
            {
                Edge edge = new(parent, node);
                if (Edge.IsOverlapping(_edges, edge))
                {
                    return false;
                }
                _edges.Add(edge);
                parent.AddEdge(node);
                node.AddEdge(parent);
            }

            return true;
        }

        public void SetSize(int x, int y, Vector2Int negativeOffset)
        {
            SizeX = x;
            SizeY = y;
            NegativeOffset = negativeOffset;
        }

        public JunctionNode TryAddJunctionNode(Polar polar, Node parent, bool allowMakeConnections)
        {
            JunctionNode node = new(polar, _settings, allowMakeConnections);
            if (TryAddNode(node, parent))
            {
                return node;
            }
            return null;
        }

        public EndNode TryAddEndNode(Polar polar, Node parent)
        {
            EndNode node = new(polar);
            if (TryAddNode(node, parent))
            {
                return node;
            }

            return null;
        }
        public StartNode TryAddStartNode(Polar polar)
        {
            StartNode node = new(polar);
            if (TryAddNode(node, null))
            {
                return node;
            }

            return null;
        }
        public void ReplaceNode(Node prev, Node next)
        {
            int index = _nodes.IndexOf(prev);
            _nodes[index] = next;
            for (int i = 0; i < _edges.Count; i++)
            {
                Edge edge = _edges[i];
                if (edge.A == prev)
                {
                    edge.SetA(next);
                }
                if (edge.B == prev)
                {
                    edge.SetB(next);
                }
            }
            for (int i = 0; i < prev.Edges.Count; i++)
            {
                Node other = prev.Edges[i];
                next.AddEdge(other);
                other.RemoveEdge(prev);
                other.AddEdge(next);
            }
        }

        public void ExpandFromJunction(JunctionNode node, List<Node> toProcess)
        {
            Polar[] polars = node.GetNext();

            for (int i = 0; i < polars.Length; i++)
            {
                Polar polar = node.Polar + polars[i];
                if (IsTooClose(polar))
                {
                    continue;
                }

                if (Random.value < _settings.EndNodeSpawnRate)
                {
                    TryAddEndNode(polar, node);
                }
                else
                {
                    JunctionNode newNode = TryAddJunctionNode(polar, node, true);
                    if (newNode != null)
                    {
                        toProcess.Add(newNode);
                    }
                }
            }

            if (!node.AllowsJunctionConnections())
            {
                return;
            }
            Node[] closeNodes = GetNodesWithinDistance(node.Polar, _settings.JunctionConnectionSearchDistance, true);
            for (int i = 0; i < closeNodes.Length; i++)
            {
                Node other = closeNodes[i];
                if (!other.AllowsJunctionConnections())
                {
                    continue;
                }
                if (Random.value > _settings.JunctionConnectionChance)
                {
                    continue;
                }

                Edge edge = new(node, other);
                if (HasEdge(edge))
                {
                    continue;
                }
                if (Edge.IsOverlapping(_edges, edge))
                {
                    continue;
                }

                _edges.Add(edge);
                node.AddEdge(other);
                other.AddEdge(node);
            }
        }

        private bool HasEdge(Edge edge)
        {
            for (int i = 0; i < _edges.Count; i++)
            {
                Edge other = _edges[i];

                if (other.A == edge.A && other.B == edge.B)
                {
                    return true;
                }
                if (other.A == edge.B && other.B == edge.A)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsTooClose(Polar polar)
        {
            Vector2 test = polar.ToCartesian();
            for (int i = 0; i < _nodes.Count; i++)
            {
                Vector2 pos = _nodes[i].Polar.ToCartesian();

                float dist = Vector2.Distance(pos, test);
                if (dist < _settings.MinDistanceBetweenNodes)
                {
                    return true;
                }
            }

            return false;
        }

        private Node[] GetNodesWithinDistance(Polar polar, float testDist, bool excludeSelf = false)
        {
            List<Node> closeNodes = new();

            Vector2 test = polar.ToCartesian();
            for (int i = 0; i < _nodes.Count; i++)
            {
                Vector2 pos = _nodes[i].Polar.ToCartesian();
                float dist = Vector2.Distance(pos, test);
                if (excludeSelf && dist == 0)
                {
                    continue;
                }
                if (dist < testDist)
                {
                    closeNodes.Add(_nodes[i]);
                }
            }

            return closeNodes.ToArray();
        }
    }
}