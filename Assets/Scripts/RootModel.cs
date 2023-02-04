using UnityEngine;

namespace GGJ23
{
    public class RootModel : MonoBehaviour
    {
        public Root container;

        private void OnTriggerEnter(Collider other)
        {
            //container.HandleTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            //container.HandleTriggerExit(other);
        }
    }
}