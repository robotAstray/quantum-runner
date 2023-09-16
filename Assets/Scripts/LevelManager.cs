using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private float spawnWidth = 1;
    // all the game objects that can be spawned and their properties
    [SerializeField] private List<GameObject> objects;
    [SerializeField] private List<Vector2> widths;
    [SerializeField] private List<Vector2> lengths;
    [SerializeField] private List<float> weights;   

    private float timer = 0f;   // for periodic instantiation

    private void Start()
    {
        if (objects.Count != widths.Count || objects.Count != lengths.Count || objects.Count != weights.Count)
        {
            print("Parameter lists have different sizes!");
        }
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
            int index = objects.Count - 1;
            var randVal = Random.Range(0, weights.Sum());
            float curWeight = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                curWeight += weights[i];
                if (curWeight > randVal)
                {
                    index = i;
                    break;
                }
            }

            var objWidth = Random.Range(widths[index].x, widths[index].y);
            var objLength = Random.Range(lengths[index].x, lengths[index].y);
            
            // spawn it randomly on the provided plane
            var posX = Random.Range(-spawnWidth / 2, spawnWidth / 2);
            // place it forward so not its middle but its end is at spawn position (to avoid spawning things into each other)
            var pos = transform.position + Vector3.right * posX + Vector3.forward * objLength / 2;

            var obj = Instantiate(objects[index], pos, Quaternion.identity);
            obj.transform.localScale = new(objWidth, 1, objLength);

            // decide how long to wait until next object is spawned
            timer = Random.Range(ParameterManager.Instance.minInterval, ParameterManager.Instance.maxInterval);
        }
    }
}
