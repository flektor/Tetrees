using UnityEngine;

public class Tip : MonoBehaviour
{
    [SerializeField] Transform rootTip;
    [SerializeField] Transform node;

    void Awake()
    {
        DeactivateNode();
    }

    public void ActivateNode()
    {
        if (_isNodeActive) return;

        _isNodeActive = true;
        rootTip.gameObject.SetActive(false);
        node.gameObject.SetActive(true);
    }

    public void DeactivateNode()
    {
        if (!_isNodeActive) return;

        _isNodeActive = false;
        node.gameObject.SetActive(false);
        rootTip.gameObject.SetActive(true);
    }

    bool _isNodeActive = false;


}
