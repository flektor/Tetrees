using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ23
{
    public class RootSpawner : MonoBehaviour
    {
        [SerializeField] private List<Root> _rootPrefabs = new();
        
        public Root SpawnRoot()
        {
            var nodePrefab = _rootPrefabs[Random.Range(0, _rootPrefabs.Count)];
            return Instantiate(nodePrefab, new Vector3(10000, 0, 0), Quaternion.identity);
        }
    }
}