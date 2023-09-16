using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /**Controls the player movement and its interactions (splitting, dying).
     * ("RunnerController" might have been a better name)
     */
    
    private int _id = -1;           // to easier keep track of the individual runners
    private bool _isFrozen = true;  // on creation there is some time before the player can control this runner's movement
    private float _freezeTimer = 0f;
    private int _score = 0;         // local score of a runner
    private Vector3 _modelSize;
    
    // Start is called before the first frame update
    void Start()
    {
        _freezeTimer = ParameterManager.Instance.freezePeriod;
        _modelSize = GetComponentInChildren<SphereCollider>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isFrozen)
        {
            // no input is handled when runner is frozen
            _freezeTimer -= Time.deltaTime;
            if (_freezeTimer <= 0)
            {
                Unfreeze();                
            }
        }
        else
        {
            // input handling
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += ParameterManager.Instance.sideSpeed * Time.deltaTime * Vector3.left;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.position += ParameterManager.Instance.sideSpeed * Time.deltaTime * Vector3.right;
            }
        }
    }

    public void SetId(int newId)
    {
        if (newId == 0) Unfreeze();     // hacky solution but just for testing so the very first runner doesn't have to wait
        
        if (_id < 0)
        {
            _id = newId;
        }
    }

    public int GetId() => _id;

    public int GetScore() => _score;

    public void SetScore(int score)
    {
        _score = score;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SplitWall"))
        {
            var splitController = collision.gameObject.GetComponent<SplitController>();
            if (splitController != null)
            {
                var width = collision.collider.bounds.size.x + _modelSize.x;
                
                // check whether we are on the left or right side of the wall
                Vector3 forceVector;
                if (transform.position.x < collision.transform.position.x)
                    forceVector = Vector3.right * width;
                else
                    forceVector = Vector3.left * width;
                
                // perform a split if possible
                if (splitController.CanSplit())
                {
                    splitController.InitiateSplit();
            
                    Tunnel(forceVector);
                    Bounce(-forceVector);   // another explicit vector/force might be better
                }
                else
                {
                    // else just bounce off
                    Bounce(-forceVector);
                }
            }
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            collision.gameObject.GetComponentInParent<AudioSource>().Play();
            Die();
        }
        else if (collision.gameObject.CompareTag("Coin"))
        {
            collision.gameObject.GetComponentInParent<AudioSource>().Play();
            PickedUpCoin();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            var pc = collision.gameObject.GetComponent<PlayerController>();
            GameManager.Instance.FuseRunner(_id, pc.GetId());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CollapseSite"))
        {
            other.gameObject.GetComponentInParent<AudioSource>().Play();
            GameManager.Instance.CollapseRunner(_id);
        }
    }

    private void PickedUpCoin()
    {
        _score += 1;
        EventManager.Instance.TriggerScoreUpdate(_score, _id);
    }

    private void Tunnel(Vector3 offset)
    {
        var runner = GameManager.Instance.SpawnRunner(transform.position + offset);
        runner.GetComponentInParent<AudioSource>().Play();
    }

    private void Bounce(Vector3 offset)
    {
        transform.position += offset * ParameterManager.Instance.bounceMult;
    }

    private void Die()
    {
        GameManager.Instance.DestroyRunner(_id);
    }

    private void Unfreeze()
    {
        _isFrozen = false;
        _freezeTimer = 0f;
    }
}
