using UnityEngine;

public class LevelGenCollider : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        // hacky solution to the problem that a SplitWall has multiple triggers that can exit:
        // only activate spawning again if the collider has the same tag as the grandparent (base object)
        var obj = other.gameObject;
        if (obj.transform.parent != null && obj.transform.parent.parent != null)
        {
            var grandParent = obj.transform.parent.parent;
            if (obj.CompareTag(grandParent.tag))
            {
                GetComponentInParent<LevelManager>().ActivateSpawning();
            }
            else
            {
                //print($"Wrong tag: {obj.tag} vs {grandParent.tag}");
            }
        }
        else
        {
            //print("no grandparent");
        }
    }
}
