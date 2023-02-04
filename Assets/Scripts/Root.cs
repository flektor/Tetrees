using System.Collections.Generic;
using UnityEngine;

namespace GGJ23
{
    public class Root : MonoBehaviour
    {
        public enum PlacementState
        {
            Dragging,
            Rotating,
            Locked
        }
        
        public RootConnection[] outgoingConnections;

        private PlacementState _placementState;

        public void HandleUpdate(List<RootConnection> openConnections, float threshold, out PlacementState placementState)
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
                case PlacementState.Rotating when Input.GetMouseButtonDown(0):
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
            Vector3 delta = mouseWorldPos - (Vector2)transform.position;
            var angle = Vector3.SignedAngle(Vector3.down, delta, Vector3.forward);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void HandleDrag(List<RootConnection> openConnections, float snapThreshold)
        {
            var newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z = transform.position.z;

            if (CloseEnoughToOpenConnection(openConnections, snapThreshold, out var connection))
            {
                //Debug.Log($"Connected to open connection: {connection}");
                transform.position = connection.transform.position;
            }
            else
            {
                //Debug.Log($"Dragging to {newPosition} at Frame {Time.frameCount}");
                transform.position = newPosition;
            }
        }

        private void TryStartRotating(List<RootConnection> openConnections, float threshold)
        {
            if (CloseEnoughToOpenConnection(openConnections, threshold, out var connection))
            {
                _placementState = PlacementState.Rotating;
            }
        }

        private bool CloseEnoughToOpenConnection(List<RootConnection> openConnections, float threshold, out RootConnection connection)
        {
            connection = null;
            var currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = transform.position.z;
            
            foreach (var openConnection in openConnections)
            {
                var delta = currentPos - openConnection.transform.position;
                if (delta.sqrMagnitude < threshold)
                {
                    connection = openConnection;
                    return true;
                }
            }

            return false;
        }

        private void HandleBackToDrag(List<RootConnection> openConnections, float unSnapThreshold)
        {
            _placementState = PlacementState.Dragging;
            HandleDrag(openConnections, unSnapThreshold);
        }

        private void LockDown()
        {
            //TODO check if colliding? / don't do if collided
            _placementState = PlacementState.Locked;
        }
        
        //
        // public void HandleTriggerEnter(Collider other)
        // {
        //     if (other.CompareTag("Node"))
        //     {
        //         _collider = other;
        //         transform.position = _collider.transform.position;
        //     }
        // }
        //
        // public void HandleTriggerExit(Collider other)
        // {
        //     _collider = null;
        //     _placementState = PlacementState.Dragging;
        // }
    }
}