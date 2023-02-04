using UnityEngine;

namespace GGJ23
{
    public class RootConnection : MonoBehaviour
    {
        [SerializeField] Transform mesh;
        [SerializeField] Transform nodeGizmo;

        void Awake()
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

        bool _isConnectionActive = false;
    }
}