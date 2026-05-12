using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Renderer
{
    public interface IPoolable
    {
        void OnStartUse();
        void OnEndUse();
    }

    public class ObjectPool<T> where T : IPoolable
    {
        private readonly HashSet<T> _objectsInUse;
        private readonly Stack<T> _availableObjects;
        private Func<T> _generateObject;

        public ObjectPool(int preWarmSize, Func<T> generateObject)
        {
            _objectsInUse = new();
            _availableObjects = new();
            _generateObject = generateObject;

            for (int i = 0; i < preWarmSize; i++)
            {
                AddToAvailable();
            }
        }

        public T GetFromPool()
        {
            if (_availableObjects.Count == 0)
            {
                GrowPool();
            }

            T obj = _availableObjects.Pop();
            _objectsInUse.Add(obj);
            obj.OnStartUse();
            return obj;
        }

        public void ReturnToPool(T obj)
        {
            obj.OnEndUse();
            _objectsInUse.Remove(obj);
            _availableObjects.Push(obj);
        }

        private void GrowPool()
        {
            int newObjects = Math.Max(Mathf.RoundToInt(_objectsInUse.Count * 0.1f), 5);
            for (int i = 0; i < newObjects; i++)
            {
                AddToAvailable();
            }
        }

        private void AddToAvailable()
        {
            _availableObjects.Push(_generateObject());
        }
    }
}