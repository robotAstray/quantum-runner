using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /**Singleton that manages general game stuff:
     * - make sure the camera "follows" the runners
     * - keep track of all the runners
     * - calculate the current max score
     * 
     */
    
    private static GameManager _gameManager;
    public static GameManager Instance
    {
        get
        {
            if (!_gameManager)
            {
                _gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
                if (_gameManager == null)
                {
                    Debug.LogError("There needs to be one active GameManager script on a GameObject in your scene.");
                }
                else
                {
                    _gameManager.Init();
                }
            }
            return _gameManager;
        }
    }
    
    [SerializeField] private GameObject runnerPrefab;
    [SerializeField] private Vector3 startPos = Vector3.zero;
    [SerializeField] private GameObject camera;
    [SerializeField] private Vector3 cameraOffset = Vector3.zero;
    [SerializeField] private GameObject scoreUI;

    private Dictionary<int, GameObject> runnerDict = new();
    private int nextId = 0;
    private int score = 0;
    private TMPro.TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        SpawnRunner(startPos);
        scoreText = scoreUI.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update camera movement
        if (runnerDict.Count > 0)
        {
            // first find out the middle of all runners     // todo can be optimized since the relative middle only changes when someone dies or spawns?
            var middleX = 0f;
            var middleZ = 0f;
            foreach (var val in runnerDict.Values)
            {
                var position = val.transform.position;
                middleX += position.x;
                middleZ += position.z;    // although I guess z-coordinate should be the same for everyone   
            }
            middleX /= runnerDict.Count;
            middleZ /= runnerDict.Count;

            camera.transform.position = cameraOffset + new Vector3(middleX, 0, middleZ);
            
            // compute max score
            score = 0;
            foreach (var val in runnerDict.Values)
            {
                var pc = val.GetComponent<PlayerController>();
                if (score < pc.GetScore())
                {
                    score = pc.GetScore();
                }
            }
            scoreText.text = score.ToString();
        }
        
    }

    private void Init()
    {
        // nothing to do currently
    }

    public void SpawnRunner(Vector3 position)
    {
        var runner = Instantiate(runnerPrefab, position, Quaternion.identity);
        var pc = runner.GetComponent<PlayerController>();
        pc.SetId(nextId);
        nextId++;       // todo potential bug if we would run out of IDs (integer overflow)

        runnerDict[pc.GetId()] = runner;
    }

    public void DestroyRunner(PlayerController pc)
    {
        var runner = runnerDict[pc.GetId()];
        runnerDict.Remove(pc.GetId());
        Destroy(runner);

        if (runnerDict.Count <= 0)
        {
            print("GAME OVER");
            SpawnRunner(startPos);
        }
    }
}
