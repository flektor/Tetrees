using UnityEngine;

namespace GGJ23
{
    public class RootConnection : MonoBehaviour
    {
        [SerializeField] private Transform mesh;
        [SerializeField] private Transform nodeGizmo;

        private void Awake()
        {
           // DisableConnection();
           EnableConnection();
        }

        public void EnableConnection()
        {
            if (_isConnectionActive) return;

            _isConnectionActive = true;
            mesh.gameObject.SetActive(false);
            nodeGizmo.gameObject.SetActive(true);
        }

        public void DisableConnection()
        {
            if (!_isConnectionActive) return;

            _isConnectionActive = false;
            nodeGizmo.gameObject.SetActive(false);
            mesh.gameObject.SetActive(true);
        }

        private bool _isConnectionActive = false;
    }
}