using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ23
{
    public class RootSpawner : MonoBehaviour
    {
        [SerializeField] private List<Root> _rootPrefabs = new();
        
        private Root _holdingRoot;
        
        public Root SpawnRoot()
        {
            var nodePrefab = _rootPrefabs[Random.Range(0, _rootPrefabs.Count)];
            return Instantiate(nodePrefab, new Vector3(10000, 0, 0), Quaternion.identity);
        }

        public Root HoldRoot(Root currentRoot)
        {
            if (!_holdingRoot)
            {
                _holdingRoot = SpawnRoot();
                _holdingRoot.transform.position = new Vector3(20000, 0, 0);
                _holdingRoot.transform.rotation = Quaternion.identity;
            }
            (currentRoot, _holdingRoot) = (_holdingRoot, currentRoot);
            (currentRoot.transform.position, _holdingRoot.transform.position) = (_holdingRoot.transform.position, currentRoot.transform.position);
            currentRoot.GetOutOfHold();
            _holdingRoot.PutOnHold();
            return currentRoot;

        }
    }
}