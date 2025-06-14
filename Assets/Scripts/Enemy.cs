using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public Player player;
    public Maze maze;
    public NavMeshAgent agent;
    public Vector3 middleCellPos;
    public Animator enemyAnimator;
    public float viewRange;
    public float hearingRange;
    public LayerMask playerLayer;
    public bool playerFound;
    [FormerlySerializedAs("hitPlayer")] public bool hitPlayerAnimation;
    [FormerlySerializedAs("hitRange")] public float hitRangeAnimation;
    public bool hitPlayer;
    public float timer = 8f;
    public Vector3 lastPlayerPos;
    public float snapDistance;
    public float minDistance;
    public float maxDistance;
    public bool isTargetable;
    public bool endAnimation;
    public bool playerFinished;
    public bool goMiddleCell;
    public AudioClip footstepWalk, footstepRugWalk, hitSound, laughSound;
    public AudioClip[] randomSounds;
    public float soundTimer;
    private AudioSource randomSource;
    public AudioClip gotHitSound;
    private OpenDoor openDoor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        maze = FindFirstObjectByType<Maze>();
        Vector2Int middleCell = maze.mazeMiddleCellPos;
        middleCellPos = new Vector3(middleCell.x * 8, 0, middleCell.y * 8);
        hitPlayerAnimation = false;
        isTargetable = true;
        Difficulty();
        if (GameManagerInstance.Instance.level == "easy")
            agent.speed = 2.5f;
        else
            agent.speed = 3.0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        playerFound = FindPlayer();
        FollowPlayer();
        Animate();
        PlayerFinishedLevel();
        RandomSounds();
        
        if (goMiddleCell)
            agent.destination = middleCellPos;
    }

    private bool SnapPosition(Vector3 pos, float maxDistance, out Vector3 snappedPos)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, maxDistance, NavMesh.AllAreas))
        {
            snappedPos = hit.position;
            return true;
        }
        snappedPos = Vector3.zero;
        return false;
    }

    public Vector3 RandomNavMeshPosition(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
        randomDirection.Normalize();
        randomDirection *= radius;
        randomDirection += player.transform.position;
        Vector3 finalPos = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, 1))
            finalPos = hit.position;
        return finalPos;
    }

    public void RandomSounds()
    {
        soundTimer -= Time.deltaTime;
        if (soundTimer <= 0)
        {
            randomSource = AudioManagerScript.Instance.PlaySound3D(randomSounds[Random.Range(0, randomSounds.Length)], RandomNavMeshPosition(Random.Range(0, 18)), pitch: Random.Range(0.8f, 1.2f));
            randomSource.gameObject.AddComponent<AudioLowPassFilter>().cutoffFrequency = 8500.0f;
            soundTimer = 20.0f;
        }
    }
    public void FollowPlayer()
    {
        if (player.inWardrobe)
            hitRangeAnimation = 3.5f;
        else
            hitRangeAnimation = 2.5f;
        
        if (((playerFound || AudioManagerScript.Instance.hearingSomething) && !playerFinished) || ((!playerFound && !playerFinished) && (player.exhaleSource != null ||
                player.breathingSource != null) && Vector3.Distance(transform.position, player.transform.position) <= hearingRange))
        {
            Vector3 snappedPos;
            if (SnapPosition(player.transform.position, snapDistance, out snappedPos))
            {
                lastPlayerPos = snappedPos;
            }
            agent.destination = player.transform.position;
            timer = 1f;
        }
        
        else if (!playerFound)
        {
            if (timer > 0)
            {
                agent.destination = lastPlayerPos;
                if (Vector3.Distance(lastPlayerPos, transform.position) < 2)
                    timer -= Time.deltaTime;
            }
            else
            {
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    Vector3 randomPos = RandomNavMeshPosition(Random.Range(minDistance, maxDistance));
                    agent.SetDestination(randomPos);
                }
            }
        }
    }

    public bool FindPlayer() 
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= viewRange)
        {
            Vector3 playerDirection = ((player.transform.position + transform.up) - (transform.position + transform.up * 2.25f)).normalized;
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up * 2.25f, playerDirection, out hit, viewRange, playerLayer))
            {
                Debug.DrawRay(transform.position + transform.up * 2.25f, playerDirection * hit.distance, Color.yellow);
                if (hit.collider.gameObject.GetComponent<Player>())
                {
                    Quaternion targetRotation = Quaternion.LookRotation(playerDirection.IgnoreY(), Vector3.up);
                    transform.rotation = targetRotation;
                    hitPlayerAnimation = Vector3.Distance(transform.position, player.transform.position) <= hitRangeAnimation;
                    return true;
                }
            }
        }
        hitPlayerAnimation = false;
        return false;
    }
    
    public void Difficulty()
    {
        if (GameManagerInstance.Instance.level == "easy")
        {
            viewRange = 20.0f;
            hearingRange = 4.0f;
        }
        else if (GameManagerInstance.Instance.level == "medium")
        {
            viewRange = 30.0f;
            hearingRange = 6.0f;
        }
        else if (GameManagerInstance.Instance.level == "hard")
        {
            viewRange = 40.0f;
            hearingRange = 8.0f;
        }
    }
    public void Animate()
    {
        float speed = agent.velocity.magnitude;
        enemyAnimator.SetFloat("Speed", speed);
        enemyAnimator.SetBool("Hit", hitPlayerAnimation);
        enemyAnimator.SetBool("HitPlayer", hitPlayer);
        enemyAnimator.SetBool("EndAnimation", endAnimation);
    }
    
    public void AttackEvent(AnimationEvent animationEvent) 
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= hitRangeAnimation)
        {
            player.playerHealth--;
            if (player.playerHealth != 0)
                AudioManagerScript.Instance.PlaySound2D(gotHitSound);
            player.playerHealth = Mathf.Clamp(player.playerHealth, 0, 2);
            hitPlayer = true;
            player.spawnButterfly = true;
        }
    }

    public void HitAnimationEnd(AnimationEvent animationEvent)
    {
        hitPlayer = false;
        if (player.playerHealth == 0)
        {
            player.enabled = false;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("Death");
        }
    }

    public void EndAnimation(AnimationEvent animationEvent)
    {
        agent.isStopped = false;
        endAnimation = false;
        goMiddleCell = true;
        enemyAnimator.SetBool("EndWalk", true);
    }

    public void PlayerFinishedLevel()
    {
        if (player.endGame)
        {
            playerFinished = true;
            float gridWidth = 8f;
            Vector2Int exitCell = maze.exitCellPos;
            Vector3 endPos = new Vector3(exitCell.y * gridWidth, 0.0f, (exitCell.x * gridWidth) + 2);
            agent.destination = endPos;

            if (Vector3.Distance(transform.position, endPos) <= 2)
            {
                agent.isStopped = true;
                playerFound = false;
                endAnimation = true;
                player.endGame = false;
            }
        }
    }
    public void EnemyFootsteps(AnimationEvent animationevent)
    {
        float volume = 1.0f;
        AudioManagerScript.Instance.FootstepSound(footstepWalk, footstepRugWalk, gameObject, volume);
    }

    public void HitSound(AnimationEvent animationEvent)
    {
        AudioManagerScript.Instance.PlaySound3D(hitSound, transform.position);
    }

    public void LaughSound(AnimationEvent animationEvent)
    {
        float volume = 2.0f;
        AudioManagerScript.Instance.PlaySound3D(laughSound, transform.position, volume);
    }
}