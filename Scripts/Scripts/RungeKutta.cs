using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HDCParameters
{
    public float alpha, beta, gamma, omega, A, B, C, psi, mu;
}

public class RungeKutta : MonoBehaviour
{
    public GameObject player;
    public HDCParameters param;

    private Rigidbody rb;
    private float xn; 
    private float dxn;
    private float h; //h is the time between two computation of xn & dxn
    private bool test; //Used in FixedUpdate() to compute k. only one fixedDeltaTime on two
    private float k1, k2, k3, k4; //Used to solve HDC diff equation by RK (4th order) method
    private float y, dy; // Position and speed of the player

    private void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        h = 2 * Time.fixedDeltaTime;
        k1 = 0;
        k2 = 0;
        k3 = 0;
        k4 = 0;
        test = true;
        xn = 0;
        dxn = 0;
        y = rb.position.x;
        dy = 0;
    }

    private void FixedUpdate()
    {
        dy = (rb.position.x - y) / Time.fixedDeltaTime;
        y = rb.position.x;

        //k1,k4 are computed at every tn, with tn+1 = tn + h. k2 & k3 are computed at every tn + h/2
        if (test)
        {
            k4 = HDCFunction(xn + h * dxn + (h * h) * k2 / 2, dxn + h * k3);
            xn = xn + h * dxn + (h * h) * (k1 + k2 + k3) / 6;
            dxn = dxn + h * (k1 + 2 * k2 + 2 * k3 + k4) / 6;
            //print("xn = " + xn.ToString() + " ; " + "dxn = " + dxn.ToString() + " ; " + "h = " + h.ToString());
            gameObject.transform.Translate(new Vector3(xn - gameObject.transform.position.x, 0.0f, 0.0f));
            k1 = HDCFunction(xn, dxn);
            test = !test;
        }

        else
        {
            k2 = HDCFunction(xn + h * dxn / 2, dxn + h * k1 / 2);
            k3 = HDCFunction(xn + h * dxn / 2 + (h * h) * k1 / 4, dxn + h * k2 / 2);
            test = !test;
        }
    }

    float HDCFunction(float x, float dx)
    {
        float internalDynamics = (param.alpha * x * x + param.beta * dx * dx - param.gamma) * dx + param.omega * param.omega * x;
        float coupling = (dx - param.mu * dy) * (param.A + param.B * Mathf.Pow(x - param.mu * y, 2)) + param.C * (Mathf.Cos(param.psi) * (dx - dy) + Mathf.Sin(param.psi) * param.omega * y);
        return coupling - internalDynamics;
    }
}
