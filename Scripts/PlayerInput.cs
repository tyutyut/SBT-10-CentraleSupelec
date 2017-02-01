using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Vector3[] position = new Vector3[2];
    private Vector3 leftPos;
    private Vector3 rightPos;
    private static PlayerInput instance;

    void Awake()
    {
        instance = this;
    }

    public static PlayerInput Instance
    {
        get
        {
            return instance;
        }
    }

    public Vector3[] handPosition() // Returns an array of vector 3 representing the left hand position and the right hand position
    {
        KinectManager kinectManager = KinectManager.Instance;

        if (kinectManager != null)
        {
            uint playerID = kinectManager.GetPlayer1ID();

            if (kinectManager.IsJointTracked(playerID, 7)) // left hand
            {
                leftPos = kinectManager.GetJointPosition(playerID, 7);
            }

            if (kinectManager.IsJointTracked(playerID, 11)) // right hand
            {
                rightPos = kinectManager.GetJointPosition(playerID, 11);
            }

            position[0] = leftPos;
            position[1] = rightPos;

            return position;

        }
        return position;
    }

    public bool IsLeftHandUp() //Returns true if left hand is up
    {
        KinectManager kinectManager = KinectManager.Instance;

        if (kinectManager != null)
        {
            uint playerID = kinectManager.GetPlayer1ID();

            if (kinectManager.IsJointTracked(playerID, 7) && kinectManager.IsJointTracked(playerID, 5))
            {
                return (kinectManager.GetJointPosition(playerID, 7).y >= kinectManager.GetJointPosition(playerID, 5).y); // a hand is considered as up if it's above the elbow
            }

        }
        return false;
    }

    public bool IsRightHandUp()//Returns true if right hand is up
    {
        KinectManager kinectManager = KinectManager.Instance;

        if (kinectManager != null)
        {
            uint playerID = kinectManager.GetPlayer1ID();

            if (kinectManager.IsJointTracked(playerID, 11) && kinectManager.IsJointTracked(playerID, 9))
            {
                return (kinectManager.GetJointPosition(playerID, 11).y >= kinectManager.GetJointPosition(playerID, 9).y);
            }

        }
        return false;
    }
}
