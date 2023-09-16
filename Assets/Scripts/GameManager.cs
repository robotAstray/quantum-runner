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
    [SerializeField] private GameObject scoreUI;

    private Vector3 cameraOffset;
    private Dictionary<int, GameObject> runnerDict = new();
    private int nextId = 0;
    private int score = 0;
    private TMPro.TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = camera.transform.position;
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

    public GameObject SpawnRunner(Vector3 position)
    {
        var runner = Instantiate(runnerPrefab, position, Quaternion.identity);
        var pc = runner.GetComponent<PlayerController>();
        pc.SetId(nextId);
        nextId++;       // todo potential bug if we would run out of IDs (integer overflow)

        runnerDict[pc.GetId()] = runner;
        return runner;
    }

    public void DestroyRunner(int id)
    {
        var runner = runnerDict[id];
        runnerDict.Remove(id);
        Destroy(runner);

        if (runnerDict.Count <= 0)
        {
            print("GAME OVER");
            SpawnRunner(startPos);
        }
    }

    public void CollapseRunner(int id)
    {
        foreach (var key in runnerDict.Keys)
        {
            if (key != id) DestroyRunner(key);
        }
    }

    public void FuseRunner(int id1, int id2)
    {
        // since this method will be called from both runners (since both runners collide with each other) we have to
        // make sure to only fuse them once, hence we compare their unique ids
        if (id1 < id2)
        {
            var runner1 = runnerDict[id1];
            var pc1 = runner1.GetComponent<PlayerController>();
            var runner2 = runnerDict[id2];
            var pc2 = runner2.GetComponent<PlayerController>();

            // compute combined score for the fused runner and its spawn position
            int combinedScore = pc1.GetScore() + pc2.GetScore() + 1;
            Vector3 midPos = (runner1.transform.position + runner2.transform.position) / 2;

            // create the new runner
            var fusedRunner = SpawnRunner(midPos);
            fusedRunner.GetComponent<PlayerController>().SetScore(combinedScore);
            
            // destroy the old runners
            DestroyRunner(pc1.GetId());
            DestroyRunner(pc2.GetId());
        }
    }
}
