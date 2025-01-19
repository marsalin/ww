using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [Header("Movement")] public bool canMove;
    public float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public bool isSprinting;
    public float targetSpeed;
    public float stamina = 3f;
    public bool coffee;
    public float lanternSpeed = 0.1f;
    public float lanternRange = 0.5f;
    private static readonly int Speed = Animator.StringToHash("Speed");
    public float playerSpeed;
    public bool rotated;
    public Vector3 lastPlayerRotation;

    [Header("Mouse settings")] public float mouseRange = 70f;
    public float vertical;
    private float mouseSensitivity = 2f;

    [Header("Canvas")] public Camera mainCamera;
    public Slider staminaSlider;
    public TMP_Text pressEText;
    public GameObject vignette;
    public GameObject deathVignette;

    [Header("Animate")] public Animator animator;
    public GameObject lanternCube;
    public Vector3 lanternPos;
    public GameObject[] finger;
    public GameObject leftHandPos;
    public GameObject[] leftFinger;
    public float leftHandTarget;
    public GameObject leftElbowPos;
    public GameObject handDefaultPos;
    public GameObject finger0, finger6, finger7, finger8, finger12, finger13, fingeri;
    public GameObject fingerBreath0, fingerBreath12, fingerBreath13, fingerBreathi;
    private float leftHandWeight;
    private CharacterController characterController;
    private Maze maze;

    [Header("Coffee")] public float coffeeTimer;
    public bool drinkCoffee;
    public GameObject coffeeGameObject;
    public float coffeeDrinkTimer;
    public float coffeeMoveTimer;
    public bool coffeeDrinking;
    public Vector3 coffeeLastPos;
    public GameObject coffeeHandRot;
    public Transform coffeeHoldPos;
    public GameObject coffeeDrinkingPos;
    public GameObject coffeePlane;
    public GameObject coffeeLerp;

    [Header("Interaction")] public LayerMask interactLayer;
    public bool holdingBreath;
    public bool lastChance;
    public bool isDead;
    public int playerHealth;
    public float deathTime;
    public float interactionRange;

    [Header("Enemy")] public float enemyspawnTimer;
    public GameObject enemyPrefab;
    public bool spawn = false;
    public float spawnTimer;
    public GameObject butterflyPrefab;
    public bool spawnButterfly;

    [Header("Sound")] public bool isPlayingWalkSound;
    public bool isPlayingSprintSound;
    public AudioClip breathWalk;
    public AudioClip breathSprint;
    public AudioClip footstepWalk;
    public AudioClip footstepRugWalk;
    public AudioClip footstepSprint;
    public AudioClip footstepRugSprint;
    public bool hasStopped;
    public AudioClip exhale;
    public AudioSource breathingSource;
    public AudioSource exhaleSource;
    public AudioClip deathSound;
    public AudioClip gotHitSound;
    
    [Header("End")] public bool endGame;
    public GameObject endGameObject;
    private MenuManager menuManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canMove = false;
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        maze = FindFirstObjectByType<Maze>();
        playerHealth = 2;
        pressEText.enabled = false;
        vignette.SetActive(false);
        deathVignette.SetActive(false);
        endGameObject.SetActive(false);
        BreathSound(breathWalk);
        lastPlayerRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        if (canMove)
            MouseLook();
        CheckRotation();
        Lantern();
        Spawner();
        Interaction();
        PlayerHealth();
        PlaySound();
        Coffee();
        if (Input.GetButtonDown("Restart"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LateUpdate()
    {
        Finger();
        LeftFinger();
    }

    public void PlayerMovement()
    {
        float moveForwards = Input.GetAxis("Vertical");
        float moveRight = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Breath") && playerSpeed < 0.1f)
            holdingBreath = true;

        if (Input.GetButtonUp("Breath"))
            holdingBreath = false;

        if (Input.GetButtonDown("Sprint"))
            isSprinting = true;

        if (Input.GetButtonUp("Sprint"))
            isSprinting = false;

        if ((holdingBreath || isSprinting) && stamina > 0 && !coffee)
        {
            stamina -= Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 5);
        }
        else if (coffee)
        {
            stamina += Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 5);
        }
        else
        {
            isSprinting = false;
            holdingBreath = false;
            stamina += Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 5);
        }

        staminaSlider.value = stamina;
        if (staminaSlider.value <= 0)
            Exhale();
        MovementSpeed();
        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, Time.deltaTime * 10f);
        Vector3 movement = (transform.right * moveRight + transform.forward * moveForwards).normalized;
        if (canMove)
            characterController.SimpleMove(movement * moveSpeed);
        playerSpeed = movement.magnitude;
        animator.SetFloat("Speed", playerSpeed);
        animator.SetBool("isSprinting", isSprinting);
    }

    public void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);

        vertical -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        vertical = Mathf.Clamp(vertical, -mouseRange, mouseRange);
        
        mainCamera.transform.localRotation = Quaternion.Euler(vertical, 0, 0);
    }

    public void MovementSpeed()
    {
        if (isSprinting)
            targetSpeed = sprintSpeed;
        else
            targetSpeed = walkSpeed;
    }

    public void CheckRotation()
    {
        if (lastPlayerRotation != transform.rotation.eulerAngles)
        {
            rotated = true;
            lastPlayerRotation = transform.rotation.eulerAngles;
        }
        else
            rotated = false;
        animator.SetBool("rotated", rotated);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (!enabled) return;
        float lerpTime = 4.0f;
        lanternPos = lanternCube.transform.position;
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        animator.SetIKPosition(AvatarIKGoal.RightHand, lanternPos);

        Quaternion handRotation = lanternCube.transform.rotation;
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);

        animator.SetIKHintPosition(AvatarIKHint.RightElbow, transform.position + transform.right * 2f);

        if (holdingBreath || coffeeDrinking)
            leftHandTarget = 1.0f;
        else
            leftHandTarget = 0.0f;

        animator.SetIKPosition(AvatarIKGoal.LeftHand,
            holdingBreath ? leftHandPos.transform.position
                : (coffeeDrinking && coffeeHoldPos != null ? coffeeHoldPos.transform.position : handDefaultPos.transform.position));
        Quaternion leftHandRotation = leftHandPos.transform.rotation;
        Quaternion coffeeHandRotation = coffeeHandRot.transform.rotation;
        animator.SetIKRotation(AvatarIKGoal.LeftHand, holdingBreath ? leftHandRotation : coffeeHandRotation);
        animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowPos.transform.position);

        leftHandWeight = Mathf.Lerp(leftHandWeight, leftHandTarget, Time.deltaTime * lerpTime);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
        animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftHandWeight);
        
    }

    public void Finger()
    {
        for (int i = 0; i < finger.Length; i++)
        {
            finger[i].transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
    }
    
    public void LeftFinger()
    {
        for (int i = 0; i < leftFinger.Length; i++)
        {
            if (i == 0)
                leftFinger[0].transform.localRotation = Quaternion.Lerp(leftFinger[0].transform.localRotation,
                    Quaternion.Euler(coffeeDrinking ? finger0.transform.localPosition : fingerBreath0.transform.localPosition),
                    leftHandWeight);
            else if (i == 6)
                leftFinger[6].transform.localRotation = Quaternion.Lerp(leftFinger[6].transform.localRotation,
                    Quaternion.Euler(coffeeDrinking ? finger6.transform.localPosition : fingeri.transform.localPosition),
                    leftHandWeight);
            else if (i == 7)
                leftFinger[7].transform.localRotation = Quaternion.Lerp(leftFinger[7].transform.localRotation,
                    Quaternion.Euler(coffeeDrinking ? finger7.transform.localPosition : fingeri.transform.localPosition),
                    leftHandWeight);
            else if (i == 8)
                leftFinger[8].transform.localRotation = Quaternion.Lerp(leftFinger[8].transform.localRotation,
                    Quaternion.Euler(coffeeDrinking ? finger8.transform.localPosition : fingeri.transform.localPosition),
                    leftHandWeight);
            else if (i == 12)
                leftFinger[12].transform.localRotation = Quaternion.Lerp(leftFinger[12].transform.localRotation,
                    Quaternion.Euler(coffeeDrinking ? finger12.transform.localPosition : fingerBreath12.transform.localPosition),
                    leftHandWeight);
            else if (i == 13)
                leftFinger[13].transform.localRotation = Quaternion.Lerp(leftFinger[13].transform.localRotation,
                    Quaternion.Euler(coffeeDrinking ? finger13.transform.localPosition : fingerBreath13.transform.localPosition),
                    leftHandWeight);
            else
                leftFinger[i].transform.localRotation = Quaternion.Lerp(leftFinger[i].transform.localRotation,
                    Quaternion.Euler(coffeeDrinking ? fingeri.transform.localPosition : fingerBreathi.transform.localPosition),
                    leftHandWeight);
        }
    }

    public void Lantern()
    {
        float newX = Mathf.Sin(Time.time * lanternSpeed) * lanternRange;
        lanternCube.transform.localRotation = Quaternion.Euler(newX - 12, 0, 0);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Start"))
        {
            spawn = true;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("End"))
        {
            endGame = true;
            Destroy(other.gameObject);
        }
    }
    public Vector3 RandomNavMeshPosition(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
        randomDirection.Normalize();
        randomDirection *= radius;
        randomDirection += transform.position;
        Vector3 finalPos = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, 1))
            finalPos = hit.position;
        return finalPos;
    }
    public void Spawner()
    {
        float gridWidth = 8f;
        Vector2Int startCell = maze.entranceCellPos;
        Vector3 entranceCellWorldPos =
            new Vector3(startCell.y * gridWidth, 0.0f, (startCell.x * gridWidth) - gridWidth);
        Vector3 spawnPos = new Vector3(entranceCellWorldPos.x, entranceCellWorldPos.y,
            (entranceCellWorldPos.z + 8.0f));

        if (spawn)
        {
            enemyspawnTimer -= Time.deltaTime;

            if (enemyspawnTimer <= 0)
            {
                Instantiate(enemyPrefab, RandomNavMeshPosition(Random.Range(GameManagerInstance.Instance.minRange, GameManagerInstance.Instance.maxRange)), Quaternion.identity);
                spawn = false;
            }
        }

        if (playerHealth == 1 && spawnButterfly)
        {
            Instantiate(butterflyPrefab, transform.position, Quaternion.identity);
            spawnButterfly = false;
        }
    }

    public void Interaction()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionRange,
                interactLayer))
        {
            Debug.DrawLine(mainCamera.transform.position, hit.point, Color.magenta);
            if (hit.collider.gameObject.CompareTag("Wardrobe") || hit.collider.gameObject.CompareTag("Bed") ||
                hit.collider.gameObject.CompareTag("Radio") || hit.collider.gameObject.CompareTag("Book") ||
                hit.collider.gameObject.CompareTag("Trunk") || hit.collider.gameObject.CompareTag("Cat") ||
                hit.collider.gameObject.CompareTag("Door") || hit.collider.gameObject.CompareTag("WardrobeDoll") ||
                hit.collider.gameObject.CompareTag("TV") || hit.collider.gameObject.CompareTag("LightSwitch") ||
                hit.collider.gameObject.CompareTag("Frame"))
            {
                pressEText.enabled = true;
            }

            if (hit.collider.gameObject.CompareTag("Coffee"))
            {
                pressEText.enabled = true;
                if (Input.GetButtonDown("Interact"))
                {
                    coffeeGameObject = hit.collider.gameObject.transform.parent.gameObject;
                    coffeeLastPos = coffeeGameObject.transform.position;
                    coffeeHoldPos = coffeeGameObject.transform.GetChild(0).GetChild(0);
                    coffeePlane = coffeeGameObject.transform.GetChild(1).gameObject;
                    coffeeLerp = coffeeGameObject.transform.GetChild(2).gameObject;
                }
            }

            if (holdingBreath)
                pressEText.enabled = false;

            if (Input.GetButtonDown("Interact") && !holdingBreath)
            {
                IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                interactable?.Interact();
            }
        }
        else
            pressEText.enabled = false;
    }

    public void PlayerHealth()
    {
        if (playerHealth == 1)
        {
            lastChance = true;
            vignette.SetActive(true);
        }
        else if (playerHealth == 0)
        {
            mainCamera.transform.SetParent(mainCamera.GetComponent<CameraScript>().cameraPos); 
            isDead = true;
            canMove = false;
            deathVignette.SetActive(true);
            animator.SetBool("Death", true);
            enabled = false;
        }
        else
        {
            lastChance = false;
            isDead = false;
        }
    }

    public void Coffee()
    {
        coffeeDrinkTimer -= Time.deltaTime;
        if (coffeeDrinkTimer >= 0)
        {
            coffeeDrinking = true;
            canMove = false;
        }
        else
        {
            coffeeDrinking = false;
            canMove = true;
            if (coffeeGameObject != null)
                Destroy(coffeeGameObject);
        }

        if (drinkCoffee)
        {
            coffeeTimer -= Time.deltaTime;
            if (coffeeTimer >= 0)
                coffee = true;
            else
            {
                coffee = false;
                coffeeTimer = 8.0f;
                drinkCoffee = false;
            }
        }

        if (coffeeDrinking && coffeeGameObject != null)
        {
            coffeeMoveTimer -= Time.deltaTime;
            float coffeeDrinkSpeed = 5.0f;
            if (coffeeMoveTimer <= 0.0f)
            {
                if (coffeeDrinkTimer >= 1.0f)
                {
                    coffeeGameObject.transform.position = Vector3.Lerp(coffeeGameObject.transform.position,
                        coffeeDrinkingPos.transform.position, Time.deltaTime * coffeeDrinkSpeed);
                    coffeePlane.transform.localPosition = Vector3.Lerp(coffeePlane.transform.localPosition, coffeeLerp.transform.localPosition, Time.deltaTime * coffeeDrinkSpeed);
                
                    Vector3 lookDir = transform.position - coffeeGameObject.transform.position;
                    lookDir.y = 0.0f;
                    coffeeGameObject.transform.rotation = Quaternion.Lerp(coffeeGameObject.transform.rotation,
                        Quaternion.LookRotation(lookDir, Vector3.up), Time.deltaTime * 15.0f);
                }
                else
                {
                    coffeeGameObject.transform.position = Vector3.Lerp(coffeeGameObject.transform.position, handDefaultPos.transform.position, Time.deltaTime * coffeeDrinkSpeed);
                } 
            }
        }
    }

    public void PlaySound()
    {
        if (isSprinting && !isPlayingSprintSound)
        {
            BreathSound(breathSprint);
            isPlayingSprintSound = true;
            isPlayingWalkSound = false;
            hasStopped = false;
        }
        else if (!isSprinting && !isPlayingWalkSound && !holdingBreath)
        {
            BreathSound(breathWalk);
            isPlayingWalkSound = true;
            isPlayingSprintSound = false;
            hasStopped = false;
        }
        else if (holdingBreath && stamina > 0 && !hasStopped)
        {
            StopBreathSound();
            isPlayingWalkSound = false;
            isPlayingSprintSound = false;
            hasStopped = true;
        }
    }
    public void Exhale()
    {
        if (exhaleSource != null)
            Destroy(exhaleSource.gameObject);
        exhaleSource = AudioManagerScript.Instance.PlaySound2D(exhale);
    }

    public void BreathSound(AudioClip breathingClip)
    {
        float volume = 0.2f;
        if (breathingSource != null)
            Destroy(breathingSource.gameObject);
        breathingSource = AudioManagerScript.Instance.PlaySound2D(breathingClip, volume, loop: true);
    }
    public void StopBreathSound()
    {
        breathingSource?.Stop();
    }

    public void GotHitSound()
    {
        AudioManagerScript.Instance.PlaySound2D(gotHitSound);
    }
    public void DeathSound()
    {
        AudioManagerScript.Instance.PlaySound2D(deathSound);
    }
    public void WalkingFootsteps(AnimationEvent animationevent)
    {
        AudioManagerScript.Instance.FootstepSound(footstepWalk, footstepRugWalk, gameObject);
    }

    public void SprintingFootsteps(AnimationEvent animationevent)
    {
        AudioManagerScript.Instance.FootstepSound(footstepSprint, footstepRugSprint, gameObject);
    }
}