using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEventRaiseExample : MonoBehaviour
{
    public VoidEvent testEvent;

    void Start()
    {
        testEvent?.Raise();
    }
}