using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using MathNet.Numerics.RootFinding;

public class GameController : MonoBehaviour
{

    public GameObject player;
    public GameObject spawner;
    public GameObject[] collectables;
    public GUIText missedCountText;
    public GUIText scoreText;
    public Canvas pauseCanvas;
    public Vector3 spawnValues;
    public Vector2 spawnWaitValues; // Values of spawnWait; .x = base value and .y = accelerated spawning
    public float startWait;  // time till the spawning starts at the beginning of the game
    public float waitWait;   // time between two waves
    public int collectableCount;

    private float spawnWait;  // time between the spawn of two object while a wave
    private float spawnWaitGoal;
    private int score;
    private int missedCount;
    private bool paused; // Indicates if the game is paused or not.
    private bool testSpawnWait; // Indicates if the spawnWait has been changed recently
    private IEnumerator coroutine;

    public bool Paused() // Allow other scripts to detect if the game is paused or not
    {
        return paused;
    }

    void Start()
    {
        paused = false;
        BaseSettings();
        coroutine = SpawnWaves(); // We keep a copy of the script, in order to be able to stop the coroutine
        StartCoroutine(coroutine);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    IEnumerator SpawnWaves() //Spawns collectables, with a certain spawning function (ex: SpawnCos())
    {
        yield return new WaitForSeconds(startWait);

        while (true)
        {
            for(int i =0; i < collectableCount; i++)
            {
                if (!paused)
                {
                    GameObject collectable = collectables[Random.Range(0, collectables.Length)];
                    Vector3 spawnPosition = SpawnCos();
                    Quaternion spawnRotation = Quaternion.identity;
                    Instantiate(collectable, spawnPosition, spawnRotation);
                }
                while (paused) //Stops the spawing while the game is paused
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(spawnWait);
                if (testSpawnWait) // The spawnWait is graduately adaptated to to spawnWaitGoal if testSpawnWait = true (meaning the goal has changed recently)
                {
                    AdaptSpawnWait();
                }
            }
            yield return new WaitForSeconds(waitWait);
        }

    }

    Vector3 SpawnHDC()
    {
        return spawner.transform.position;
    }

    Vector3 SpawnRandomly()   // Returns a Vector3 used to spawn collectables at a random x position at the top of the screen
    {
        return new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
    }

    Vector3 SpawnCos()   // Returns a Vector3 used to spawn collectables with position x(t) = a * cos(t)
    {
        return new Vector3(spawnValues.x * Mathf.Cos(Time.time), spawnValues.y, spawnValues.z);
    }

    Vector3 SpawnCosRandom()
    {
        return 0.3f * SpawnCos() + 0.7f * SpawnRandomly();
    }

    private void AdaptSpawnWait()
    {
        if (Mathf.Abs(2 * (spawnWait - spawnWaitGoal) / (spawnWait + spawnWaitGoal)) > 0.01) // takes 7 iterations of AdaptSpawnWait()
        {
            spawnWait = (spawnWait + spawnWaitGoal) / 2;
        }

        else
        {
            spawnWait = spawnWaitGoal;
            testSpawnWait = false;
        }
    }

    private void ChangeSpawnWait() //Change the goal for spawnwait, which will be changed in the coroutine via AdaptSpawnWait()
    {
        if (spawnWaitGoal == spawnWaitValues.x)
        {
            spawnWaitGoal = spawnWaitValues.y;
        }
        else
        {
            spawnWaitGoal = spawnWaitValues.x;
        }
        testSpawnWait = true;
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    public void AddMissed ()
    {
        missedCount += 1;
        UpdateMissedCount();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateMissedCount()
    {
        missedCountText.text = "Missed: " + missedCount;
    }

    public void PauseGame()
    {
        paused = !paused;
        pauseCanvas.gameObject.SetActive(paused);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Main");
    }

    private void BaseSettings() //Set objects of the scene to their initial value
    {
        spawnWait = spawnWaitValues.x;
        spawnWaitGoal = spawnWaitValues.x;
        testSpawnWait = false;
        score = 0;
        missedCount = 0;
        UpdateScore();
        UpdateMissedCount();
    }
}
