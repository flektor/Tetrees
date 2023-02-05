using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GGJ23.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GGJ23
{
    public class RootsController : MonoBehaviour
    {
        [SerializeField] private float _snapThreshold;
        [SerializeField] private Material _barkMaterial;
        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private Material _floatingMaterial;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private GameObject _snapVfx;
        [SerializeField] private GameObject _timeoutVfx;
        [SerializeField] private AudioSource _audioSource;
        
        [SerializeField] private AudioClip _placeSound;  
        [SerializeField] private AudioClip _winSound;
        
        private List<RootConnection> _openConnections = new();
        private readonly List<RootConnection> _newConnections = new();
        private List<WaterPocket> _waterPockets;

        private Root _currentRoot;
        private RootSpawner _rootSpawner;
        private bool _won;

        private void Start()
        {
            _rootSpawner = FindObjectOfType<RootSpawner>();
            _openConnections = FindObjectsOfType<RootConnection>().ToList();
            _openConnections.ForEach(c => c.EnableConnection());

            _waterPockets = FindObjectsOfType<WaterPocket>().ToList();
            _waterPockets.ForEach(w =>
            {
                var wt = w.transform;
                var p = wt.position;
                p.z = 2.3f;
                wt.position = p;
            });

            SpawnRoot();
        }

        private void Update()
        {
            if (_won) return;
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
            if (_won) return;
            _currentRoot.SetMaterial(_barkMaterial);
            _currentRoot.snappedConnection.DisableConnection();
            _openConnections.Remove(_currentRoot.snappedConnection);
            _newConnections.ForEach(c => c.EnableConnection());
            _openConnections.AddRange(_newConnections);
            _newConnections.Clear();
            _currentRoot.SpawnPlaceVfx(_snapVfx);
            PlaySound(_placeSound);

            if (CheckForReachedWater(out var waterPocket))
            {
                RemoveWaterPocketAndCheckIfWon(waterPocket);
            }
            else
            {
                _newConnections.AddRange(_currentRoot.outgoingConnections);
            }

            if (!_won && _waterPockets.Count > 0)
            {
                SpawnRoot();
            }
        }

        private void SpawnRoot()
        {
            if (_won) return;
            _currentRoot = _rootSpawner.SpawnRoot();
            _currentRoot.Init(_cameraController, _highlightMaterial, _floatingMaterial);
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
                _won = true;
                StartCoroutine(VictoryRoutine());
            }
        }

        private IEnumerator VictoryRoutine()
        {
            PlaySound(_winSound);
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene("StartScreen");
        }
        
        private void PlaySound(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}