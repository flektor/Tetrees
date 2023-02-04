using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ23
{
    public class RootsController : MonoBehaviour
    {
        [SerializeField] private List<Root> _rootPrefabs = new List<Root>();
        [SerializeField] private float _snapThreshold;

        private List<RootConnection> _openConnections = new List<RootConnection>();

        private Root _currentRoot;

        private void Start()
        {
            IEnumerable<RootConnection> initialConnections = FindObjectsOfType<RootConnection>();

            _currentRoot = Instantiate(_rootPrefabs.First());

            _openConnections = initialConnections.ToList();
        }

        private void Update()
        {
            if (_currentRoot)
            {
                _currentRoot.HandleUpdate(_openConnections, _snapThreshold, out var placementState);

                if (placementState == Root.PlacementState.Locked)
                {
                    //TODO: Remove open connection
                    _openConnections.AddRange(_currentRoot.outgoingConnections);
                    SpawnRoot();
                }
            }
        }
        
        public void SpawnRoot()
        {
            if (_rootPrefabs.Count > 0)
            {
                var nodePrefab = _rootPrefabs[Random.Range(0, _rootPrefabs.Count)];
                _currentRoot = GameObject.Instantiate(nodePrefab);
            }
        }

    }
}