
using Frostline.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.World.Generation
{
    public class JunctionNode : Node
    {
        private readonly GraphSettings _settings;
        private readonly int _junctions;
        private readonly bool _allowMakeConnections;

        public JunctionNode(Polar polar, GraphSettings settings, bool allowMakeConnections) : base(polar)
        {
            _settings = settings;
            _allowMakeConnections = allowMakeConnections;
            _junctions = settings.WithVariation(settings.JunctionCount, settings.JunctionCountVariation);
        }

        public override bool AllowsJunctionConnections()
        {
            if (!_allowMakeConnections)
            {
                return false;
            }

            if (ConnectedNodes() < _junctions)
            {
                return true;
            }

            return false;
        }


        public override Color GetColor()
        {
            return Color.yellow;
        }

        public Polar[] GetNext()
        {
            if (_junctions == 1)
            {
                return new Polar[0];
            }

            float radSplit = Polar.CircleRad / _junctions;
            List<Polar> result = new();

            for (int i = 0; i < _junctions - 1; i++)
            {
                float radVariation = radSplit * _settings.WithVariation(0, _settings.JunctionRadVariation);
                float radCorrection = -(Polar.CircleRad / 2 - Polar.rad - radSplit);
                float rad = radSplit * i + radCorrection + radVariation;
                float dist = _settings.WithVariation(_settings.NodeDistance, _settings.NodeDistanceVariation);

                if (Random.value < _settings.NextNodeSurvivalChance)
                {
                    result.Add(new Polar(rad, dist));
                }
            }

            return result.ToArray();
        }
    }
}