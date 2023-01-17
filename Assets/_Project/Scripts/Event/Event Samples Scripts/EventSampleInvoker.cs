using UnityEngine;

namespace Project
{
    public class EventSampleInvoker : MonoBehaviour
    {
        public void Local()
        {
            GameEvent.onLocalEventSample.Invoke();
        }

        public void Server()
        {
            GameEvent.onGlobalEventSample.Invoke();
        }
    }
}