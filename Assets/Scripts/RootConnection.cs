using UnityEngine;

namespace GGJ23
{
    public class RootConnection : MonoBehaviour
    {
        [SerializeField] private Transform mesh;
        [SerializeField] private Transform nodeGizmo;

        private void Awake()
        {
            InitConnection();
            mesh.transform.localPosition = Vector3.zero;
            nodeGizmo.transform.localPosition = Vector3.zero;
        }

        public void InitConnection()
        {
            nodeGizmo.gameObject.SetActive(false);
            mesh.gameObject.SetActive(true);
        }
        
        public void EnableConnection()
        {
            mesh.gameObject.SetActive(false);
            nodeGizmo.gameObject.SetActive(true);
        }

        public void DisableConnection()
        {
            nodeGizmo.gameObject.SetActive(false);
            mesh.gameObject.SetActive(false);
        }
    }
}