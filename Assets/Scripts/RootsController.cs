using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GGJ23
{
    public class RootsController : MonoBehaviour
    {
        [SerializeField] private float _snapThreshold;
        [SerializeField] private Material _barkMaterial;
        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private Material _floatingMaterial;
        [SerializeField] private Material _timeoutMaterial;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private float _removeRootTime;
        [SerializeField] private TMP_Text _removeRootTimerLabel;
        [SerializeField] private GameObject _snapVfx;

        [SerializeField] private TMP_Text _youWinText;
        [SerializeField] private TMP_Text _youLoseText;
        [SerializeField] private TMP_Text _waterProgressText;
        [SerializeField] private Image _timerProgressBar;

        [SerializeField] private GameObject _timerCanvas;

        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _placeSound;
        [SerializeField] private AudioClip _winSound;
        [SerializeField] private AudioClip _waterReachedSound;

        [SerializeField] private AudioClip _timoutSound;

        [SerializeField] private AudioClip _looseSound;
        
        private List<RootConnection> _openConnections = new();
        private readonly List<RootConnection> _newConnections = new();
        private List<WaterPocket> _waterPockets;

        private Root _currentRoot;
        private RootSpawner _rootSpawner;
        private bool _pause;
        private bool _canPlayTimerSound = true;

        private float _removeRootCurrentTime;

        private RootConnection _timeOutRoot;

        private int _waterPocketsToReach;

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

            _waterPocketsToReach = _waterPockets.Count;
            UpdateProgressLabel();

            _removeRootCurrentTime = _removeRootTime;

            PickNewTimeOutRoot();

            SpawnRoot();
        }

        private void Update()
        {
            if (_pause) return;

            UpdateRootTimer();

            if (_currentRoot)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _currentRoot = _rootSpawner.HoldRoot(_currentRoot);
                    _currentRoot.Init(_cameraController, _highlightMaterial, _floatingMaterial, _barkMaterial);
                }
                _currentRoot.HandleUpdate(_openConnections, _snapThreshold, out var placementState);

                if (placementState == Root.PlacementState.Locked)
                {
                    PlaceRoot();
                }
            }
        }

        private void UpdateProgressLabel()
        {
            _waterProgressText.text = $"{(_waterPocketsToReach-_waterPockets.Count)}/{_waterPocketsToReach}";
        }

        private void UpdateRootTimer()
        {
            _removeRootCurrentTime -= Time.deltaTime;
            _removeRootTimerLabel.text = $"{Mathf.RoundToInt(_removeRootCurrentTime)}";
            _timerProgressBar.fillAmount = (_removeRootCurrentTime / _removeRootTime);

            var pos = _timeOutRoot ? _timeOutRoot.transform.position + new Vector3(0, 0, -5) : new Vector3(1000, 0, 0);
            _timerCanvas.transform.position = pos;

            if (_removeRootCurrentTime <= 1.4f && _canPlayTimerSound)
            {
                _canPlayTimerSound = false;
                PlaySound(_timoutSound);
            }

            if (_removeRootCurrentTime <= -0.3f)
            {
                _canPlayTimerSound = true;
                _removeRootCurrentTime = _removeRootTime;
                if(_timeOutRoot)
                {
                    _timeOutRoot.InitConnection();
                    _openConnections.Remove(_timeOutRoot);
                }

                if (_currentRoot.snappedConnection == _timeOutRoot)
                {
                    _currentRoot.HandleBackToDrag(_openConnections, _snapThreshold);
                }

                if (_openConnections.Count <= 0)
                {
                    _pause = true;
                    StartCoroutine(LoseRoutine());
                }
                else
                {
                    PickNewTimeOutRoot();
                }
            }
        }

        private void PickNewTimeOutRoot()
        {
            if (_openConnections.Count <= 0)
            {
                _timeOutRoot = null;
                return;
            }

            _timeOutRoot = _openConnections[Random.Range(0, _openConnections.Count)];
            _timeOutRoot.SetGizmoMaterial(_timeoutMaterial);
        }

        private void PlaceRoot()
        {
            if (_pause) return;
            _currentRoot.SetMaterial(_barkMaterial);
            _currentRoot.snappedConnection.DisableConnection();
            _openConnections.Remove(_currentRoot.snappedConnection);

            if (_currentRoot.snappedConnection == _timeOutRoot)
            {
                PickNewTimeOutRoot();
            }

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

            if (!_pause && _waterPockets.Count > 0)
            {
                SpawnRoot();
            }
        }

        private void SpawnRoot()
        {
            if (_pause) return;
            _currentRoot = _rootSpawner.SpawnRoot();
            _currentRoot.Init(_cameraController, _highlightMaterial, _floatingMaterial, _barkMaterial);
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
            UpdateProgressLabel();
            if (_waterPockets.Count == 0)
            {
                Debug.Log("WON");
                _pause = true;
                StartCoroutine(VictoryRoutine());
            }
            else
            {
                PlaySound(_waterReachedSound);
            }
        }

        private IEnumerator VictoryRoutine()
        {
            _youWinText.gameObject.SetActive(true);
            PlaySound(_winSound);
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("StartScreen");
        }

        private void PlaySound(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }

        private IEnumerator LoseRoutine()
        {
            _youLoseText.gameObject.SetActive(true);
            PlaySound(_looseSound);
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("StartScreen");
        }
    }
}