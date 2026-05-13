using Frostline.Renderer;
using Frostline.World;
using Frostline.World.Structures;
using Frostline.World.Tiles;
using Frostline.World.Tracks;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private WorldSettings _worldSettings;

        private IServiceRegistry _serviceRegistry;
        private HashSet<IRequireServices> _initializers;

        private void Awake()
        {
            _serviceRegistry = new ServiceRegistry();
            _initializers = new();

            RegisterAll();
            FindSceneDependencies();

            InitializeAllDependencies(_initializers);

            WorldGeneration worldGeneration = _serviceRegistry.GetService<WorldGeneration>();
            worldGeneration.GenerateWorld();
        }

        private void RegisterAll()
        {
            RegisterService(_worldSettings);
            RegisterServiceDependency(new WorldGeneration());
            RegisterServiceDependency(new StructureBlueprintManager());
            RegisterServiceDependency(new TrackSegmentManager());
            RegisterServiceDependency(new TrackSegmentPlacer());
            RegisterServiceDependency(new StructureManager());
            RegisterServiceDependency(new TileManager());
            RegisterServiceDependency(new VisibilityManager());
        }
        private void RegisterService<T>(T t) where T : class
        {
            _serviceRegistry.RegisterService(t);
        }
        private void RegisterDependency<T>(T t) where T : IRequireServices
        {
            _initializers.Add(t);
        }
        private void RegisterServiceDependency<T>(T t) where T : class, IRequireServices
        {
            RegisterService(t);
            RegisterDependency(t);
        }
        private void InitializeAllDependencies(HashSet<IRequireServices> requireServices)
        {
            foreach (IRequireServices requireService in requireServices)
            {
                requireService.Initialize(_serviceRegistry);
            }
        }
        private void FindSceneDependencies()
        {
            MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IRequireServices requireService)
                {
                    _initializers.Add(requireService);
                }
            }
        }
    }
}
