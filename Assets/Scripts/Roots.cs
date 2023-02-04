using System.Collections.Generic;
using UnityEngine;

public class Roots : MonoBehaviour
{
    [SerializeField] List<Root> interactiveRoots;

    bool activateNodes = true;

    void Update()
    {

        if (!Input.GetKeyDown("w"))return ;


        foreach (var root in interactiveRoots)
            if (activateNodes)
            {
                root.ActivateNodes();
            }
            else
            {
                root.DeactivateNodes();
            }

        activateNodes = !activateNodes;



    }
}
