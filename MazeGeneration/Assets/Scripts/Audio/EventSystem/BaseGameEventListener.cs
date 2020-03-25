using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGameEventListener<T, E, UER> : MonoBehaviour,
    IGameEventListener<T> where E : BaseGameEvent<T> where UER : UnityEvent<T>
{
    [SerializeField] private E gameEvent;

    public E GameEvent
    {
        get { return gameEvent; }
        set { gameEvent = value; }
    }

    [SerializeField] private UER unityEventResponse;

    public UER UnityEventResponse
    {
        get { return unityEventResponse; }
        set { unityEventResponse = value; }
    }

    private void OnEnable()
    {
        if (gameEvent == null)
            return;

        GameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (gameEvent == null)
            return;

        GameEvent.UnregisterListener(this);
    }
    public void RemoteEnable()
    {
        if (gameEvent == null)
        {
            return;
        }

        GameEvent.RegisterListener(this);
    }

    public void RemoteDisable()
    {
        if (gameEvent == null)
        {
            return;
        }

        GameEvent.UnregisterListener(this);
    }

    public void OnEventRaised(T item)
    {
        if (unityEventResponse != null)
        {
            unityEventResponse.Invoke(item);
        }
    }
}