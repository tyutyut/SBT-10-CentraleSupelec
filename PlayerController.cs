using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax;
}

public class PlayerController : MonoBehaviour
{

    public float speed;
    public Boundary boundary;

    private Rigidbody rb;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate ()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");  // moveHorizontal = +1 (or -1) if right (or left) arrow key is held down

        Vector3 mouvement = new Vector3(moveHorizontal, 0.0f, 0.0f);
        rb.velocity = mouvement * speed;

        rb.position = new Vector3  // Force the player to stay in the screen (values fixed in the Boundary class)
        (
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            0.0f
        );
    }
}
