using UnityEngine;
using System.Collections;

public class Collect : MonoBehaviour
{

    public int scoreValue;

    private GameController gameController;

    void Start ()  // The GameController is selected with this function, as it's needed for AddScore()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
    }

    void OnTriggerEnter(Collider other)  // Used to make items disappear when touching the player
    {
        if (other.CompareTag("Boundary"))
        {
            return;
        }
        Destroy(gameObject);
        gameController.AddScore(scoreValue);  // Add the score value of the collectable to the player's score (see PlayerController.cs)
    }
}
