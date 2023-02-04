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
        [SerializeField] private List<Root> _rootPrefabs = new();
        [SerializeField] private float _snapThreshold;

        private List<RootConnection> _openConnections = new();
        private readonly List<RootConnection> _newConnections = new();

        private Root _currentRoot;

        private void Start()
        {
            var startingRoots = FindObjectsOfType<Root>().ToList();
            startingRoots.ForEach(r => r.SetCollidersAsTrigger(true));
            _openConnections = FindObjectsOfType<RootConnection>().ToList();
            _openConnections.ForEach(c => c.EnableConnection());

            _currentRoot = Instantiate(_rootPrefabs.First());
        }

        private void Update()
        {
            if (_currentRoot)
            {
                _currentRoot.HandleUpdate(_openConnections, _snapThreshold, out var placementState);

                if (placementState == Root.PlacementState.Locked)
                {
                    _currentRoot.snappedConnection.DisableConnection();
                    _currentRoot.SetCollidersAsTrigger(true);
                    _openConnections.Remove(_currentRoot.snappedConnection);
                    _newConnections.ForEach(c => c.EnableConnection());
                    _openConnections.AddRange(_newConnections);
                    _newConnections.Clear();
                    _newConnections.AddRange(_currentRoot.outgoingConnections);
                    SpawnRoot();
                }
            }
        }

        private void SpawnRoot()
        {
            if (_rootPrefabs.Count <= 0)
            {
                throw new Exception("Root prefabs not configured for this level");
            }

            var nodePrefab = _rootPrefabs[Random.Range(0, _rootPrefabs.Count)];
            _currentRoot = Instantiate(nodePrefab);
            _currentRoot.SetCollidersAsTrigger(false);
        }
    }
}