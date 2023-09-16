using UnityEngine;

public class LevelGenCollider : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        GetComponentInParent<LevelManager>().ActivateSpawning();
    }
}
