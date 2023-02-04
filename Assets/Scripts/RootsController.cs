using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ23
{
    public class RootsController : MonoBehaviour
    {
        [SerializeField] private List<Root> _rootPrefabs = new();
        [SerializeField] private float _snapThreshold;
        [SerializeField] private Material _barkMaterial;
        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private Material _floatingMaterial;

        private List<RootConnection> _openConnections = new();
        private readonly List<RootConnection> _newConnections = new();

        private Root _currentRoot;

        private void Start()
        {
            _openConnections = FindObjectsOfType<RootConnection>().ToList();
            _openConnections.ForEach(c => c.EnableConnection());

            CreateNewRoot();
        }

        private void Update()
        {
            if (_currentRoot)
            {
                _currentRoot.HandleUpdate(_openConnections, _snapThreshold, out var placementState);

                if (placementState == Root.PlacementState.Locked)
                {
                    PlaceRoot();
                }
            }
        }

        private void PlaceRoot()
        {
            _currentRoot.SetMaterial(_barkMaterial);
            _currentRoot.snappedConnection.DisableConnection();
            _openConnections.Remove(_currentRoot.snappedConnection);
            _newConnections.ForEach(c => c.EnableConnection());
            _openConnections.AddRange(_newConnections);
            _newConnections.Clear();
            _newConnections.AddRange(_currentRoot.outgoingConnections);
            SpawnRoot();
        }

        private void SpawnRoot()
        {
            if (_rootPrefabs.Count <= 0)
            {
                throw new Exception("Root prefabs not configured for this level");
            }

            CreateNewRoot();
        }

        private void CreateNewRoot()
        {
            var nodePrefab = _rootPrefabs[Random.Range(0, _rootPrefabs.Count)];
            _currentRoot = Instantiate(nodePrefab, new Vector3(10000, 0, 0), Quaternion.identity);
            _currentRoot.highlightMaterial = _highlightMaterial;
            _currentRoot.floatingMaterial = _floatingMaterial;
            _currentRoot.SetMaterial(_floatingMaterial);
            var flip = Random.Range(0f, 1f) > 0.5f;
            if (flip)
            {
                _currentRoot.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}