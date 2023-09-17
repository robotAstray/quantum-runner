using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public const string RUNNER_SPAWNED = "RunnerSpawned";
    public const string RUNNER_DIED = "RunnerDied";
    public const string WALL_BUMP = "WallBump";
    public const string TUNNELING = "Tunneling";
    public const string OBSTACLE_BUMP = "ObstacleBump";
    public const string COIN_PICKUP = "CoinPickup";
    public const string COLLAPSE = "Collapse";

    public class InternalManager
    {
        private Dictionary<string, UnityEvent> eventDictionary;    // for parameterless events

        public delegate void IntIntSignature(int a, int b);
        
        public event IntIntSignature onScoreUpdateListener;     // triggered when a coin is picked up

        public InternalManager()
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }

        public void TriggerScoreUpdate(int newScore, int scoreId)
        {
            if (onScoreUpdateListener != null)
            {
                onScoreUpdateListener(newScore, scoreId);
            }
        }

        public bool TryGetValue(string eventName, out UnityEvent unityEvent)
        {
            return eventDictionary.TryGetValue(eventName, out unityEvent);
        }

        public void Add(string eventName, UnityEvent unityEvent)
        {
            eventDictionary.Add(eventName, unityEvent);
        }

        public void TriggerEvent(string eventName)
        {
            if (Instance.TryGetValue(eventName, out var thisEvent))
            {
                if (thisEvent != null)
                {
                    thisEvent.Invoke();
                }
            }
        }
    }


    private static InternalManager _manager;

    public static InternalManager Instance
    {
        get
        {
            if (_manager == null)
            {
                _manager = new InternalManager();
            }

            return _manager;
        }
    }

    public static void StartListening (string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (Instance.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.AddListener (listener);
        } 
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance.Add(eventName, thisEvent);
        }
    }

    public static void StopListening (string eventName, UnityAction listener)
    {
        if (_manager == null) return;
        UnityEvent thisEvent = null;
        if (Instance.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener (listener);
        }
    }
}