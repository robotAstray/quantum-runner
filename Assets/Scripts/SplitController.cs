using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitController : MonoBehaviour
{
    /**Handles the cooldown of a SplitWall after splitting happened
     * 
     */
    
    private float _timer = 0f;
    private bool _canSplit = true;

    void Update()
    {
        if (!_canSplit)
        {
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                _canSplit = true;
                _timer = 0f;
            }
        }
    }

    public bool CanSplit() => _canSplit;

    public void InitiateSplit()
    {
        _timer = ParameterManager.Instance.splitCooldown;
        _canSplit = false;
    }
}
