using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundController : MonoBehaviour
{
    /**Singleton that handles sound
     * 
     */
    
    private static SoundController _manager;
    public static SoundController Instance
    {
        get
        {
            if (!_manager)
            {
                _manager = FindObjectOfType(typeof(SoundController)) as SoundController;
                if (_manager == null)
                {
                    Debug.LogError("There needs to be one active SoundController script on a GameObject in your scene.");
                }
                else
                {
                    _manager.Init();
                }
            }
            return _manager;
        }
    }

    [SerializeField] public AudioClip coinPickup;

    private UnityAction onCoinPickup;

    private void Init()
    {
        // currently nothing to do
    }

    private void Start()
    {
        onCoinPickup = new UnityAction(OnCoinPickup);
        EventManager.StartListening(EventManager.COIN_PICKUP, onCoinPickup);
    }

    private void OnCoinPickup()
    {
        
    }
}
