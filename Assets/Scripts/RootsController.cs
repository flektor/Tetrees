using System;
using System.Collections.Generic;
using System.Linq;
using GGJ23.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private Camera _camera;

        private List<RootConnection> _openConnections = new();
        private readonly List<RootConnection> _newConnections = new();
        private List<WaterPocket> _waterPockets;

        private Root _currentRoot;

        private void Start()
        {
            _openConnections = FindObjectsOfType<RootConnection>().ToList();
            _openConnections.ForEach(c => c.EnableConnection());

            _waterPockets = FindObjectsOfType<WaterPocket>().ToList();

            CreateNewRoot();
        }

        private void Update()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                Zoom(Input.mouseScrollDelta.y);
            }

            if (_currentRoot)
            {
                _currentRoot.HandleUpdate(_openConnections, _snapThreshold, out var placementState);

                if (placementState == Root.PlacementState.Locked)
                {
                    PlaceRoot();
                }
            }
        }

        private void Zoom(float zoom)
        {
            var mouseWorldPosStart = _camera.ScreenToWorldPoint(Input.mousePosition);
            _camera.orthographicSize -= zoom;

            var mouseWorldPosDiff = mouseWorldPosStart - _camera.ScreenToWorldPoint(Input.mousePosition);
            _camera.transform.position += mouseWorldPosDiff;
        }

        private void PlaceRoot()
        {
            _currentRoot.SetMaterial(_barkMaterial);
            _currentRoot.snappedConnection.DisableConnection();
            _openConnections.Remove(_currentRoot.snappedConnection);
            _newConnections.ForEach(c => c.EnableConnection());
            _openConnections.AddRange(_newConnections);
            _newConnections.Clear();

            if (CheckForReachedWater(out var waterPocket))
            {
                RemoveWaterPocketAndCheckIfWon(waterPocket);
            }
            else
            {
                _newConnections.AddRange(_currentRoot.outgoingConnections);
            }

            if (_waterPockets.Count > 0)
            {
                SpawnRoot();
            }
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
            _currentRoot.Init(_camera, _highlightMaterial, _floatingMaterial);
        }

        private bool CheckForReachedWater(out WaterPocket result)
        {
            foreach (var waterPocket in _waterPockets)
            {
                var waterBounds = waterPocket.GetComponent<Renderer>().bounds;
                foreach (var outgoing in _currentRoot.outgoingConnections)
                {
                    if (waterBounds.Intersects(outgoing.GetComponentInChildren<Renderer>().bounds))
                    {
                        result = waterPocket;
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        private void RemoveWaterPocketAndCheckIfWon(WaterPocket waterPocket)
        {
            _waterPockets.Remove(waterPocket);
            if (_waterPockets.Count == 0)
            {
                Debug.Log("WON");
                var nextLevel = int.Parse(SceneManager.GetActiveScene().name.Substring("Level_".Length)) + 1;
                LevelSelectScreen.StartLevel(nextLevel);
            }
        }
    }
}