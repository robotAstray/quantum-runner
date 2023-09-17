using UnityEngine;

public class ParameterManager : MonoBehaviour
{
    /**Manages all parameters for an easy way to change some settings and test things out.
     * Not a beautiful solution but should work well enough for prototyping.
     * 
     */
    
    private static ParameterManager _manager;
    public static ParameterManager Instance
    {
        get
        {
            if (!_manager)
            {
                _manager = FindObjectOfType(typeof(ParameterManager)) as ParameterManager;
                if (_manager == null)
                {
                    Debug.LogError("There needs to be one active ParameterManager script on a GameObject in your scene.");
                }
                else
                {
                    _manager.Init();
                }
            }
            return _manager;
        }
    }

    [Header("Player")]
    [SerializeField] public float forwardSpeed = 7f;    // how fast objects move towards the player
    [SerializeField] public float sideSpeed = 5f;       // how fast the player can move sideways
    [SerializeField] public float freezePeriod = 3f;    // how long a new runner is frozen
    [SerializeField] public float bounceMult = 0f;      // how much too bounce off walls (0 = deactivated)

    [Header("Level Generator")] 
    // intervals for spawning new objects
    [SerializeField] public float minInterval = 3f;
    [SerializeField] public float maxInterval = 5;

    [Header("Split Walls")] 
    [SerializeField] public float splitCooldown = 5f;   // how long to wait until one wall can cause a new split
    
    private void Init()
    {
        // currently nothing to do
    }
}
