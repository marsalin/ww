using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Butterfly : MonoBehaviour
{
    [Header("Other")]
    private Player player;
    private Maze maze;
    public NavMeshAgent agent;

    [Header("Position")]
    public Vector3 exitPosDestination;

    [Header("Butterfly")] 
    public GameObject butterfly;
    public GameObject leftWing;
    public GameObject rightWing;
    public float speed;
    public float rotationSpeed;
    public bool wingOpen;
    public float wingFlapTime;
    public float height;
    public TrailRenderer trailRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        maze = FindFirstObjectByType<Maze>();
        butterfly = GameObject.FindGameObjectWithTag("Butterfly");
    }

    // Update is called once per frame
    void Update()
    {
        ButterflyStart();
        WingRotation();
    }

    public void ButterflyStart()
    {
        agent.destination = new Vector3( maze.exitCellWorldPosRoom.x, maze.exitCellWorldPosRoom.y, maze.exitCellWorldPosRoom.z - 8);
        trailRenderer.endWidth = 0.1f;
    }
    
    public void WingRotation()
    {
        wingFlapTime -= Time.deltaTime;
        if (wingFlapTime <= 0)
        {
            wingOpen = !wingOpen;
            wingFlapTime = 0.5f;
        }
        leftWing.transform.localRotation = Quaternion.Lerp(leftWing.transform.localRotation, Quaternion.Euler(wingOpen ? -60.0f : 18.0f, wingOpen ? 30.0f : -3.0f, wingOpen ? -150.0f : -95.0f), Time.deltaTime * rotationSpeed);
        rightWing.transform.localRotation = Quaternion.Lerp(rightWing.transform.localRotation, Quaternion.Euler(wingOpen ? 65.0f : -18.0f, wingOpen ? 45.0f : 6.0f, wingOpen ? -25.0f : -95.0f), Time.deltaTime * rotationSpeed);
        
        float newY = Mathf.Sin(Time.time * speed) * height;
        butterfly.transform.position = new Vector3(butterfly.transform.position.x, newY, butterfly.transform.position.z);
    }
}
