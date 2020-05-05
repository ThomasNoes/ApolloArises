using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class FinalTestSceneManager : MonoBehaviour
{

    public static FinalTestSceneManager instance;

    int[] evenOrder = new int[6] {0, 1, 2, 3, 4, 5};
    int[] oddOrder = new int[6] { 0, 3, 4, 1, 2, 5 };

    int[] order;

    int index = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int sec= (int) DateTime.Now.Second;

        if (sec % 2 == 0)
        {
            order = evenOrder;
        }
        else
        {
            order = oddOrder;
        }
    }

    public void GoToNextScene()
    {
        index++;
        if (index < order.Length)
        {
            SceneManager.LoadScene(order[index]);
        }
    }
}
