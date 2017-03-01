using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeSelection : MonoBehaviour
{

    private GameObject activeCanvas;
    public string[] themes;
    public ArrayList themesObjects = new ArrayList();
    int counter = 0;
    Vector3 offset = new Vector3(130, 0, 0);
    private static Vector3 selectOffset = new Vector3(0, 0, 25);

    // Use this for initialization
    void Start()
    {
        InitialiseCanvas();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("left"))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown("right"))
        {
            MoveRight();
        }
    }

    void InitialiseCanvas()
    {
        counter = 0;
        foreach (string s in themes)
        {
            Debug.Log(s);
            GameObject canvas = new GameObject();
            themesObjects.Add(canvas);
            canvas.name = s + " canvas";
            RectTransform rect = canvas.AddComponent<RectTransform>();
            Canvas c = canvas.AddComponent<Canvas>();
            canvas.AddComponent<ThemeCanvas>();
            //CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
            Image image = canvas.AddComponent<Image>();
            if (counter == 0)
            {
                canvas.transform.position = new Vector3(0, 0, -25);
                activeCanvas = canvas;
            }
            else
            {
                canvas.transform.position = new Vector3(130f * counter, 0, 0);
            }

            counter++;
        }


    }
    public void MoveRight()
    {
        foreach (GameObject theme in themesObjects)
        {
            theme.GetComponent<ThemeCanvas>().Translate(-offset, selectOffset);
        }
    }

    public void MoveLeft()
    {
        foreach (GameObject theme in themesObjects)
        {
            theme.GetComponent<ThemeCanvas>().Translate(offset, selectOffset);
        }
    }
}


