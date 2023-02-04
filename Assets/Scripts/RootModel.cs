using UnityEngine;

namespace GGJ23
{
    public class RootModel : MonoBehaviour
    {
        public Root container;

        void OnTriggerEnter(Collider other)
        {
            //container.HandleTriggerEnter(other);
        }

        void OnTriggerExit(Collider other)
        {
            //container.HandleTriggerExit(other);
        }
    }
}