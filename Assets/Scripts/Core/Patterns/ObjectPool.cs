using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarDealerSimulator.Core.Patterns
{
    /// <summary>
    /// Generic object pool that reuses instances instead of repeated Instantiate/Destroy calls.
    /// Prevents GC spikes and improves performance for frequently spawned objects.
    /// </summary>
    public class ObjectPool<T> where T : class
    {
        private readonly Stack<T> _pool;
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly int _maxSize;

        public int CountActive { get; private set; }
        public int CountInactive => _pool.Count;
        public int CountAll => CountActive + CountInactive;

        public ObjectPool(
            Func<T> createFunc,
            Action<T> onGet = null,
            Action<T> onRelease = null,
            int initialSize = 0,
            int maxSize = 1000)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _onGet = onGet;
            _onRelease = onRelease;
            _maxSize = maxSize;
            _pool = new Stack<T>(initialSize);

            for (int i = 0; i < initialSize; i++)
            {
                _pool.Push(_createFunc());
            }
        }

        public T Get()
        {
            T item = _pool.Count > 0 ? _pool.Pop() : _createFunc();
            _onGet?.Invoke(item);
            CountActive++;
            return item;
        }

        public void Release(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _onRelease?.Invoke(item);
            CountActive--;

            if (_pool.Count < _maxSize)
            {
                _pool.Push(item);
            }
        }

        public void Clear()
        {
            _pool.Clear();
            CountActive = 0;
        }
    }

    /// <summary>
    /// MonoBehaviour-based object pool for GameObjects with prefab instantiation.
    /// </summary>
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _initialSize = 10;
        [SerializeField] private int _maxSize = 100;

        private ObjectPool<GameObject> _pool;

        public void Initialize(GameObject prefab, int initialSize = 10, int maxSize = 100)
        {
            _prefab = prefab;
            _initialSize = initialSize;
            _maxSize = maxSize;
            CreatePool();
        }

        private void Awake()
        {
            if (_prefab != null)
            {
                CreatePool();
            }
        }

        private void CreatePool()
        {
            _pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var obj = Instantiate(_prefab, transform);
                    obj.SetActive(false);
                    return obj;
                },
                onGet: obj => obj.SetActive(true),
                onRelease: obj => obj.SetActive(false),
                initialSize: _initialSize,
                maxSize: _maxSize
            );
        }

        public GameObject Get()
        {
            return _pool.Get();
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            var obj = _pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }

        public void Release(GameObject obj)
        {
            obj.transform.SetParent(transform);
            _pool.Release(obj);
        }

        public void Clear()
        {
            _pool.Clear();
        }
    }
}
