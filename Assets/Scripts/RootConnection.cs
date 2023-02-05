using UnityEngine;

namespace GGJ23
{
    public class RootConnection : MonoBehaviour
    {
        [SerializeField] private Transform mesh;
        [SerializeField] private Renderer nodeGizmo;

        private void Awake()
        {
            InitConnection();
            mesh.localPosition = Vector3.zero;
            nodeGizmo.transform.localPosition = Vector3.zero;
        }

        public void SetGizmoMaterial(Material material)
        {
            nodeGizmo.material = material;
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