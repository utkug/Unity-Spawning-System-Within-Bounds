using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnSystem: MonoBehaviour
{
    // References to UI elements
    public Text waveText;
    public Animator waveAnim;

    // Zombie wave settings
    public int maxZombie = 10;
    public int zombieInGame = 0;
    public int totalZombie = 0;
    public int wave1 = 10;
    public int wave2 = 25;
    public int wave3 = 40;
    private int[] waves;
    public int currentWave = 0;
    public int killedZombie = 0;

    // References for spawning
    public GameObject zombiePrefab;
    int maxSpawnAttempts = 5;
    public Transform raycastOrigin; // Assign the transform of the object from where you want to cast the ray.

    // Spawn timing 
    public float countdown = 1f;
    private bool isReadyToSpawn = true;

    // Spawn area boundaries
    public Transform x1, x2, z1, z2;

    private void Start()
    {
        // Initialize zombie waves
        waves = new int[3];
        waves[0] = wave1;
        waves[1] = wave2;
        waves[2] = wave3;
    }

    private void Update()
    {
        // Check if current wave is completed
        if (killedZombie >= waves[currentWave] && killedZombie != 0)
        {
            // Display wave completion message and trigger wave animation
            waveText.text = "Wave " + (currentWave + 1) + " completed";
            isReadyToSpawn = false;
            waveText.gameObject.SetActive(true);
            waveAnim.SetTrigger("Wave");
            StartCoroutine(Do());
            currentWave += 1;
        }
        if (isReadyToSpawn) { GenerateRandomPoint(); }
    }

    private void SendRaycast(Vector3 targetLocation)
    {
        // Cast a ray to the target location
        Vector3 direction = targetLocation - raycastOrigin.position;
        float distance = direction.magnitude;

        // Use Debug.DrawRay to visualize the ray in the scene view for debugging purposes.
        Debug.DrawRay(raycastOrigin.position, direction, Color.red, 5f);

        RaycastHit hitInfo;
        if (Physics.Raycast(raycastOrigin.position, direction, out hitInfo, distance))
        {
            // Check if the ray hits the floor and spawn zombie if it does
            if (hitInfo.collider.gameObject.CompareTag("Floor"))
            {
                SpawnZombie(targetLocation);
            }
        }
        else
        {
            // Handle case where ray doesn't hit anything
        }
    }

    private void SpawnZombie(Vector3 spawnPoint)
    {
        // Instantiate a zombie at the given spawn point
        Instantiate(zombiePrefab, spawnPoint, Quaternion.identity);
        zombieInGame++;
        totalZombie++;
        Debug.Log("Spawned");
    }

    private void GenerateRandomPoint()
    {
        // Generate random spawn points within the specified boundaries
        if (totalZombie < waves[currentWave])
        {
            for (int i = 0; i < maxSpawnAttempts; i++)
            {
                Vector3 randomPoint = new Vector3(
                    Random.Range(x1.position.x, x2.position.x),
                    Random.Range(0.8f, 5),
                    Random.Range(z1.position.z, z2.position.z)
                );
                SendRaycast(randomPoint);
            }
        }
    }

    IEnumerator Do()
    {
        // Wait for a few seconds before starting the next wave
        yield return new WaitForSeconds(5);
        waveText.text = "Wave " + (currentWave + 1) + " is coming";
        yield return new WaitForSeconds(5);
        waveAnim.SetTrigger("WaveOff");
        isReadyToSpawn = true;
    }
}
