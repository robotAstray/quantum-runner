using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    /**Handles movement of all objects in the world (used in BaseObject prefab so everything moves at a constant pace
     * towards the player.
     * 
     */
    
    void Update()
    {
        transform.position += ParameterManager.Instance.forwardSpeed * Time.deltaTime * Vector3.back;
        
        // destroy game object if it's too far away from player (player is positioned at z==0)
        if (transform.position.z < -100)    // hacky despawn method, could be turned down I think but seemed very okay right now during testing
        {
            Destroy(gameObject);
        }
    }
}
