using Frostline.Renderer;
using Frostline.World.Tiles;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private TileSettings tileSettings;

        private IServiceRegistry _serviceRegistry;
        private HashSet<IRequireServices> _initializers;

        private void Awake()
        {
            _serviceRegistry = new ServiceRegistry();
            _initializers = new();

            RegisterAll();
            FindSceneDependencies();

            InitializeAllDependencies(_initializers);
        }
        private void RegisterAll()
        {
            RegisterService(new TileManager(tileSettings));
            RegisterService(new VisibilityManager(20));
            RegisterDependency(new LogVisibility());
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
