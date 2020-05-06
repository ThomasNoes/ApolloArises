using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoggerTester : MonoBehaviour
{
    public List<string> data;
    public DataHandler dataHandler;

    void Start()
    {
       dataHandler?.SendData(data);
    }
}
