using UnityEngine;

public class EventListenExample : MonoBehaviour
{
    private void Awake()
    {
        InitializeListener();
    }

    private void ExampleFunction()
    {
        Debug.Log("Example Function executed by event!");
    }


/// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// ///
    public VoidEvent exampleEvent;
    private ExampleListener exampleListener;

    private void InitializeListener()   // Initialize the listener, preferably in awake if you have early event calls
    {
        exampleListener = new ExampleListener();
        exampleListener.GameEvent = exampleEvent;
        exampleListener.UnityEventResponse = new UnityVoidEvent();
        exampleListener.UnityEventResponse.AddListener(ExecuterFunction);
        exampleListener.RemoteEnable();
    }

    private void ExecuterFunction(Void @void) // Put everything you want to execute/run in this executer function:
    {
        ExampleFunction();

        exampleListener.RemoteDisable(); // Run disable if you don't expect more calls
    }
}

public class ExampleListener : BaseGameEventListener<Void, VoidEvent, UnityVoidEvent>
{
}
/// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// ///