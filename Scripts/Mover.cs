using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour  // Used to make collectables move down when they appear. The choice not respect physics and apply a constant velocity to the object is for gameplay purpose
{

    public float speed;

    private Rigidbody rb;
    private GameController gameController;

    void Start()
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
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!gameController.Paused())
        {
            rb.transform.Translate(new Vector3(0.0f, -speed * Time.fixedDeltaTime));
        }
    }

}
    
