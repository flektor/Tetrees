 using UnityEngine; 

public class Drag : MonoBehaviour
{
      
    void OnMouseDown()
    {
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
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Node"))
        {
            _dropPosition = _initTransformPosition;
            return;
        }
        _collider = other;
        _dropPosition = other.transform.position;
    }

    void OnTriggerExit(Collider other)
    {
        _collider = null;
        _dropPosition = _initTransformPosition;
    }
     
     
    Collider _collider;
    Vector3 _initMousePosition;
    Vector3 _initTransformPosition;
    Vector3 _dropPosition;

}


