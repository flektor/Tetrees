using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGJ23
{
    public class Root : MonoBehaviour
    {
        private const int OBSTACLE_LAYER = 6;
        private const int PIECE_LAYER = 0;
        private const int UI_LAYER = 5;

        public enum PlacementState
        {
            Dragging,
            Rotating,
            Locked
        }

        public Material floatingMaterial;
        public Material highlightMaterial;

        public RootConnection snappedConnection;

        public RootConnection[] outgoingConnections;

        private PlacementState _placementState;

        private IEnumerable<MeshRenderer> _meshRenderers;

        private int _collisions = 0;

        private void Awake()
        {
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            var colliders = GetComponentsInChildren<Collider2D>();
            foreach (var c in colliders)
            {
                c.isTrigger = true;
            }

            SetCollidersLayer(OBSTACLE_LAYER);

            _meshRenderers = GetComponentsInChildren<MeshRenderer>().Where(o => o.gameObject.layer != UI_LAYER);
        }

        public void HandleUpdate(List<RootConnection> openConnections, float threshold,
            out PlacementState placementState)
        {
            switch (_placementState)
            {
                case PlacementState.Dragging when Input.GetMouseButtonDown(0):
                    TryStartRotating(openConnections, threshold);
                    break;
                case PlacementState.Dragging:
                    HandleDrag(openConnections, threshold);
                    break;
                case PlacementState.Rotating when Input.GetMouseButtonDown(1):
                    HandleBackToDrag(openConnections, threshold);
                    break;
                case PlacementState.Rotating when _collisions == 0 && Input.GetMouseButtonDown(0):
                {
                    LockDown();
                    break;
                }
                case PlacementState.Rotating:
                {
                    HandleRotate();
                    break;
                }
            }

            placementState = _placementState;
        }

        private void HandleRotate()
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = mouseWorldPos - (Vector2) transform.position;
            var angle = Vector3.SignedAngle(Vector3.down, delta, Vector3.forward);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void HandleDrag(List<RootConnection> openConnections, float snapThreshold)
        {
            var newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z = transform.position.z;

            if (CloseEnoughToOpenConnection(openConnections, snapThreshold))
            {
                transform.position = snappedConnection.transform.position;
            }
            else
            {
                transform.position = newPosition;
            }
        }

        private void TryStartRotating(List<RootConnection> openConnections, float threshold)
        {
            if (CloseEnoughToOpenConnection(openConnections, threshold))
            {
                _placementState = PlacementState.Rotating;

                SetCollidersLayer(PIECE_LAYER);
            }
        }

        private bool CloseEnoughToOpenConnection(List<RootConnection> openConnections, float threshold)
        {
            snappedConnection = null;
            var currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = 0;

            foreach (var openConnection in openConnections)
            {
                var delta = currentPos - openConnection.transform.position;
                if (delta.sqrMagnitude < threshold)
                {
                    snappedConnection = openConnection;
                    return true;
                }
            }

            return false;
        }

        private void HandleBackToDrag(List<RootConnection> openConnections, float unSnapThreshold)
        {
            _placementState = PlacementState.Dragging;
            HandleDrag(openConnections, unSnapThreshold);
            SetCollidersLayer(OBSTACLE_LAYER);
        }

        private void LockDown()
        {
            //TODO check if colliding? / don't do if collided
            _placementState = PlacementState.Locked;
            SetCollidersLayer(OBSTACLE_LAYER);
        }

        private void SetCollidersLayer(int layer)
        {
            var colliders = GetComponentsInChildren<Collider2D>();
            foreach (var c in colliders)
            {
                c.gameObject.layer = layer;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer != OBSTACLE_LAYER)
            {
                return;
            }

            _collisions++;
            if (_collisions == 1)
            {
                SetMaterial(highlightMaterial);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer != OBSTACLE_LAYER)
            {
                return;
            }

            _collisions--;
            if (_collisions == 0)
            {
                SetMaterial(floatingMaterial);
            }
        }

        public void SetMaterial(Material material)
        {
            foreach (var meshRenderer in _meshRenderers)
            {
                meshRenderer.material = material;
            }
        }
    }
}