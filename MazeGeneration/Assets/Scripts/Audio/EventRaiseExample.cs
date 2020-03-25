using UnityEngine;

public class EventRaiseExample : MonoBehaviour
{
    public VoidEvent testEvent;

    void Start()
    {
        testEvent?.Raise();
    }
}