using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // all the game objects that can be spawned
    [SerializeField] private GameObject obstacle;
    [SerializeField] private GameObject splitWall;
    [SerializeField] private GameObject coin;

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
            var posX = Random.Range(-planeWidth / 2, planeWidth / 2);
            // add a small forward offset so it looks like they're coming through the horizon
            var pos = transform.position + Vector3.right * posX + Vector3.forward * 2;

            var randVal = Random.value;
            if (randVal < ParameterManager.Instance.obstacleChance)
            {
                Instantiate(obstacle, pos, Quaternion.identity);
            }
            else if (randVal < ParameterManager.Instance.obstacleChance + ParameterManager.Instance.coinChance)
            {
                Instantiate(coin, pos, Quaternion.identity);
            }
            else
            {
                var obj = Instantiate(splitWall, pos, Quaternion.identity);
                var wallWidth = Random.Range(ParameterManager.Instance.minWallWidth,
                    ParameterManager.Instance.maxWallWidth);
                var wallLength = Random.Range(ParameterManager.Instance.minWallLength,
                    ParameterManager.Instance.minWallLength);
                obj.transform.localScale = new(wallWidth, 1, wallLength);
            }
            
            // decide how long to wait until next object is spawned
            timer = Random.Range(ParameterManager.Instance.minInterval, ParameterManager.Instance.maxInterval);
        }
    }
}
