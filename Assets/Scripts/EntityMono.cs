using UnityEngine;

public class EntityMono : MonoBehaviour
{ 
        [ContextMenu("DoSomething")] 
        public void DoSomething()
        {
                Debug.Log("Something in entity");
        }
}