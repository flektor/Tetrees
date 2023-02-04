using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField] List<Tip> tips;

    public void ActivateNodes()
    {
        foreach (var tip in tips)
        {
            tip.ActivateNode();
        }
    }

    public void DeactivateNodes()
    {
        foreach (var tip in tips)
        {
            tip.DeactivateNode();
        }
    }

    public void RemoveTip(Tip tip)
    {
        Destroy(tip);
    }


}
