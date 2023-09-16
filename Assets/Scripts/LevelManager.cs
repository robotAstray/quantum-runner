using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    /**Singleton that handles random level generation
     * 
     */
    
    private static LevelManager _manager;
    public static LevelManager Instance
    {
        get
        {
            if (!_manager)
            {
                _manager = FindObjectOfType(typeof(LevelManager)) as LevelManager;
                if (_manager == null)
                {
                    Debug.LogError("There needs to be one active LevelManager script on a GameObject in your scene.");
                }
                else
                {
                    _manager.Init();
                }
            }
            return _manager;
        }
    }

    // all the game objects that can be spawned and their properties
    [SerializeField] private List<GameObject> objects;
    [SerializeField] private List<Vector2> widths;
    [SerializeField] private List<Vector2> lengths;
    [SerializeField] private List<float> chances;   

    private float timer = 0f;   // for periodic instantiation
    private float planeWidth = 0f;

    private void Start()
    {
        // maybe don't use full width so nothing spawns directly at the border?
        planeWidth = 50;     // todo fix magic number
    }

    private void Init()
    {
        // nothing to do currently
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        // spawn new object if enough time passed
        if (timer <= 0)
        {
            int index;
            var randVal = Random.value;
            for (index = 0; index < chances.Count && randVal > 0; index++)
            {
                randVal -= chances[index];
            }

            var objWidth = Random.Range(widths[index].x, widths[index].y);
            var objLength = Random.Range(lengths[index].x, lengths[index].y);
            
            // spawn it randomly on the provided plane
            var posX = Random.Range(-planeWidth / 2, planeWidth / 2);
            // place it forward so not its middle but its end is at spawn position (to avoid spawning things into each other)
            var pos = transform.position + Vector3.right * posX + Vector3.forward * objLength / 2;

            var obj = Instantiate(objects[index], pos, Quaternion.identity);
            obj.transform.localScale = new(objWidth, 1, objLength);

            // decide how long to wait until next object is spawned
            timer = Random.Range(ParameterManager.Instance.minInterval, ParameterManager.Instance.maxInterval);
        }
    }
}
