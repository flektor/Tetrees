using System;
using UnityEngine;

namespace GGJ23
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _minZoom = 10;
        [SerializeField] private float _maxZoom = 60;

        private float _targetSize;

        private void Start()
        {
            _targetSize = _camera.orthographicSize;
        }

        public Vector3 GetMouseWorldPos()
        {
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            return pos;
        }

        public void ZoomToFitPoint(Vector3 point)
        {
            if (IsNotVisible(point))
            {
                _targetSize += 10*Time.deltaTime;
            }
        }

        private bool IsNotVisible(Vector3 point)
        {
            var viewportPoint = _camera.WorldToViewportPoint(point);
            return viewportPoint.x is < 0 or > 1 || viewportPoint.y is < 0 or > 1;
        }

        private void Update()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                Zoom(Input.mouseScrollDelta.y);
            }

            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetSize, 0.5f);
        }

        private void Zoom(float zoom)
        {
            var mouseWorldPosStart = _camera.ScreenToWorldPoint(Input.mousePosition);
            _targetSize -= zoom;

            var mouseWorldPosDiff = mouseWorldPosStart - _camera.ScreenToWorldPoint(Input.mousePosition);
            _camera.transform.position += mouseWorldPosDiff;
        }
    }
}