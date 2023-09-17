using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

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
    [FormerlySerializedAs("camera")] [SerializeField] private GameObject cameraObj;
    [SerializeField] private GameObject scoreUI;

    [SerializeField] private GameObject highScoreUI;

    private Vector3 cameraOffset;
    private Dictionary<int, GameObject> runnerDict = new();
    private int nextId = 0;
    private int maxScore = 0;
    private int maxScoreId = -1;
    private TMP_Text scoreText;

    private int highScore = 0;
    private TMP_Text highScoreText;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = cameraObj.transform.position;
        SpawnRunner(startPos);
        scoreText = scoreUI.GetComponent<TMP_Text>();

        highScoreText = highScoreUI.GetComponent<TMP_Text>();
        
        EventManager.Instance.onScoreUpdateListener += UpdateScore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        // nothing to do currently
    }

    private void UpdateScore(int newScore, int scoreId)
    {
        if (newScore > maxScore)
        {
            maxScore = newScore;
            scoreText.text = maxScore.ToString();
            maxScoreId = scoreId;   
        }

        // keep track of high score
        if (maxScore > highScore)
        {
            highScore = maxScore;
            highScoreText.text = highScore.ToString();
        }
    }

    public GameObject SpawnRunner(Vector3 position)
    {
        var runner = Instantiate(runnerPrefab, position, Quaternion.identity);
        var pc = runner.GetComponent<PlayerController>();
        pc.SetId(nextId);
        nextId++;       // todo potential bug if we would run out of IDs (integer overflow)
        
        EventManager.Instance.TriggerEvent(EventManager.RUNNER_SPAWNED);
        
        runnerDict[pc.GetId()] = runner;
        return runner;
    }

    public void DestroyRunner(int id)
    {
        var runner = runnerDict[id];
        runnerDict.Remove(id);
        
        // check if we need to update the score
        if (id == maxScoreId)
        {
            // compute new max score since potentially
            int score = 0;
            int scoreId = 0;
            foreach (var val in runnerDict.Values)
            {
                var pc = val.GetComponent<PlayerController>();
                if (score < pc.GetScore())
                {
                    score = pc.GetScore();
                    scoreId = pc.GetId();
                }
            }

            maxScore = -1;  // reset its value so we can overwrite it
            UpdateScore(score, scoreId);
        }
        
        Destroy(runner);

        if (runnerDict.Count <= 0)
        {
            print("GAME OVER");
            SpawnRunner(startPos);
        }

        EventManager.Instance.TriggerEvent(EventManager.RUNNER_DIED);
    }

    public void CollapseRunner(int id)
    {
        var keys = new int[runnerDict.Count];
        runnerDict.Keys.CopyTo(keys, 0);
        foreach (var key in keys)
        {
            if (key != id) DestroyRunner(key);
        }
        
        EventManager.Instance.TriggerEvent(EventManager.COLLAPSE);
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
