using System.Collections;
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

    [SerializeField] private float delay;

    public float Delay
    {
        get { return delay; }
        set { delay = value; }
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
            if (delay <= 0)
                unityEventResponse.Invoke(item);
            else
                StartCoroutine(DelayedEventRaise(item));
        }
    }

    private IEnumerator DelayedEventRaise(T item)
    {
        yield return new WaitForSeconds(delay);
        unityEventResponse.Invoke(item);
    }
}