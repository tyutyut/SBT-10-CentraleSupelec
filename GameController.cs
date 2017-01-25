using UnityEngine;
using System.Collections;
using MathNet.Numerics.RootFinding;

public class GameController : MonoBehaviour
{

    public GameObject player;
    public GameObject[] collectables;
    public GUIText missedCountText;
    public GUIText scoreText;
    public Vector3 spawnValues;
    public float spawnWait;  // time between the spawn of two object while a wave
    public float startWait;  // time till the spawning starts at the beginning of the game
    public float waitWait;   // time between two waves
    public float alpha, beta, gamma, omega, A, B; // Parameters for HDC equation
    public int mu; // Equal to +1 or -1; defines whether the VP's goal is inphase or antiphase coordination
    public int collectableCount;

    private Rigidbody rb;  // Used to access player's position & velocity
    private float Xn_1;  // Used to compute the value of x position of spawning of collectables x(t). Equals to x(t-dt)
    private float Xn_2;  // Equals to x(t-2dt)
    private float spawn_x_Value;  // Equals to x(t)
    private int score;
    private int missedCount;

    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        Xn_1 = rb.position.x;
        Xn_2 = rb.position.x;
        spawn_x_Value = rb.position.x;
        score = 0;
        missedCount = 0;
        UpdateScore();
        UpdateMissedCount();
        StartCoroutine(SpawnWaves());
    }
    // This may cause a framerate problem (it's computing 3 cubic polynomial roots every frame)
    void FixedUpdate()  // We use FixedUpdate to compute the position of spawning of collectables, because the computation needs to be done every fixed amount of time dt = Time.deltaTime
    {                   // ATTENTION la racine choisie n'est peut être pas la bonne... Il faut voir quelle racine considérer si le polynôme en a plusieurs
        MathNet.Numerics.Tuple<double, double, double> roots = Root_Newton_Polynomial_HDC(Xn_1, Xn_2, Time.deltaTime, alpha, beta, gamma, omega, A, B, mu);
        if (!(double.IsNaN(roots.Item1)))
        {
            if (!(double.IsNaN(roots.Item2)))
            {
                spawn_x_Value = Mathf.Min((float)roots.Item3 - Xn_1,(float)roots.Item1 - Xn_1,(float)roots.Item2 - Xn_1);
            }
            else
            {
                spawn_x_Value = (float)roots.Item1;
            }
        }
        else
        {
            if (!(double.IsNaN(roots.Item2)))
            {
                spawn_x_Value = (float)roots.Item2;
            }
            else
            {
                spawn_x_Value = (float)roots.Item3;
            }
        }
        Xn_2 = Xn_1;
        Xn_1 = spawn_x_Value;
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);

        while (true)
        {
            for(int i =0; i < collectableCount; i++)
            {
                GameObject collectable = collectables[Random.Range(0, collectables.Length)];
                Vector3 spawnPosition = SpawnHDC();
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(collectable, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waitWait);
        }

    }

    Vector3 SpawnRandomly()   // Returns a Vector3 used to spawn collectables at a random x position at the top of the screen
    {
        return new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
    }

    Vector3 SpawnCos()   // Returns a Vector3 used to spawn collectables with position x(t) = a * cos(t)
    {
        return new Vector3(spawnValues.x * Mathf.Cos(Time.time), spawnValues.y, spawnValues.z);
    }

    Vector3 SpawnHDC()
    {
        return new Vector3(2*spawn_x_Value * spawnValues.x, spawnValues.y, spawnValues.z);
    }

    float Polynomial_HDC (float Xn, float Xn_1, float Xn_2, float dt, float alpha, float beta, float gamma, float omega, float A, float B, int mu) // Returns the value of the polynomial from HDC equation, evaluated at Xn (as ugly to see as to type)
    {
        float coeff_X3 = alpha * (1 + beta / (dt * dt)) / dt - beta / dt;
        float coeff_X2 = alpha * (-Xn_1 - 2 * beta * Xn_1 / (dt * dt)) / dt + beta * (Xn_1 / dt + mu * rb.velocity.x) + 2 * mu * beta * rb.position.x / dt;  //rb.position.x is the player x position, and rb.velocity.x its x velocity
        float coeff_X1 = 1 / (dt * dt) + alpha * (2 * beta * Xn_1 * Xn_1 / (dt * dt) + beta * Xn_1 * Xn_1 / (dt * dt) - gamma) / dt + omega * omega - 2 * mu * beta * rb.position.x * (Xn_1 / dt + mu * rb.velocity.x) - (A + B * rb.position.x * rb.position.x) / dt;
        float coeff_X0 = (Xn_2 - 2 * Xn_1) / (dt * dt) - alpha * Xn_1 * (beta * Xn_1 * Xn_1 / (dt * dt) - gamma) / dt + (A + B * rb.position.x * rb.position.x) * (Xn_1 / dt + mu * rb.velocity.x);
        return coeff_X3 * Xn * Xn * Xn + coeff_X2 * Xn * Xn + coeff_X1 * Xn + coeff_X0;
    }
    
    MathNet.Numerics.Tuple<double, double, double> Root_Newton_Polynomial_HDC (float Xn_1, float Xn_2, float dt, float alpha, float beta, float gamma, float omega, float A, float B, int mu) // Returns the root of the polynomial from HDC Equation (one of these roots is the new value of spawning for the x axis)
    {
        float coeff_X3 = alpha * (1 + beta / (dt * dt)) / dt - beta / dt;
        float coeff_X2 = alpha * (-Xn_1 - 2 * beta * Xn_1 / (dt * dt)) / dt + beta * (Xn_1 / dt + mu * rb.velocity.x) + 2 * mu * beta * rb.position.x / dt;  //rb.position.x is the player x position, and rb.velocity.x its x velocity
        float coeff_X1 = 1 / (dt * dt) + alpha * (2 * beta * Xn_1 * Xn_1 / (dt * dt) + beta * Xn_1 * Xn_1 / (dt * dt) - gamma) / dt + omega * omega - 2 * mu * beta * rb.position.x * (Xn_1 / dt + mu * rb.velocity.x) - (A + B * rb.position.x * rb.position.x) / dt;
        float coeff_X0 = (Xn_2 - 2 * Xn_1) / (dt * dt) - alpha * Xn_1 * (beta * Xn_1 * Xn_1 / (dt * dt) - gamma) / dt + (A + B * rb.position.x * rb.position.x) * (Xn_1 / dt + mu * rb.velocity.x);
        var roots = Cubic.RealRoots((double)coeff_X0 / coeff_X3, (double)coeff_X1 / coeff_X3, (double)coeff_X2 / coeff_X3);  // The type of variables in this tuple is double; we shall convert to float in another function
        return roots;
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
}
