using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour  // Used to make collectables move down when they appear. The choice not respect physics and apply a constant velocity to the object is for gameplay purpose
{

    public float speed;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.up * -speed;
    }
}
    
