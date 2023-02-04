using UnityEngine;

public class Node : MonoBehaviour
{
    void Awake()
    {
        _meshRenderer = GetComponent<Renderer>();
        _initColor = _meshRenderer.material.color;
    }

    void OnTriggerEnter(Collider other)
    {

        Debug.Log("entered");
        _meshRenderer.material.color = Color.red;
    }

    void OnTriggerExit(Collider other)
    {
        _meshRenderer.material.color = _initColor;
    }

    Renderer _meshRenderer;
    Color _initColor;

}
