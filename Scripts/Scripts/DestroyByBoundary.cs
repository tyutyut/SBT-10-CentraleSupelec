using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour
{

    private GameController gameController;

    void Start()  // The GameController is selected with this function, as it's needed for AddMissed()
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

    void OnTriggerExit(Collider other)  // Make collectables disappear when they leave the playable zone
    {
        Destroy(other.gameObject);
        gameController.AddMissed(); // Add one the counter of missed collectables
    }
}
