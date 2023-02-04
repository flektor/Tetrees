using UnityEngine;

public class Drag : MonoBehaviour
{



    private void Update()
    {
        if (placementState != RootPlacementState.Rotating) return;



        var mousePosition = Camera.main.WorldToScreenPoint(Input.mousePosition);

        var delta = mousePosition - _collider.transform.position;
         
        var angle = Vector3.Angle(Vector3.up, delta);

      //  transform.Rotate(transform.position, angle);

        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle);

        Debug.DrawLine(_collider.transform.position, mousePosition);



    }

    void OnMouseDown()
    {
        placementState = RootPlacementState.Dragging;
        _initTransformPosition = transform.position;
        _dropPosition = _initTransformPosition;
        _initMousePosition = Input.mousePosition - Camera.main.WorldToScreenPoint(_initTransformPosition);
    }

    void OnMouseDrag()
    {
        if (_collider == null)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - _initMousePosition);
            return;
        }

        transform.position = _collider.transform.position;
    }

    void OnMouseUp()
    {
        transform.position = _dropPosition;
        if (_isAttached)
        {
            placementState = RootPlacementState.Rotating;
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Node"))
        {
            _dropPosition = _initTransformPosition;
            _isAttached = false;
            return;
        }
        _isAttached = true;
        _collider = other;
        _dropPosition = other.transform.position;
    }

    void OnTriggerExit(Collider other)
    {
        _collider = null;
        _dropPosition = _initTransformPosition;
    }

    RootPlacementState placementState = RootPlacementState.Dragging;


    bool _isAttached = false;

    Vector3 _prevMousePosition;

    Collider _collider;
    Vector3 _initMousePosition;
    Vector3 _initTransformPosition;
    Vector3 _dropPosition;

}

public enum RootPlacementState
{
    Dragging,
    Rotating,
    Locked
}
