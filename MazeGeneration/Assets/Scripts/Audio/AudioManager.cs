using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public SoundEvent[] soundEvents;
    private AudioSource thisAudioSource;

    private void Awake()
    {
        thisAudioSource = GetComponent<AudioSource>();
        thisAudioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (soundEvents != null)
            for (int i = 0; i < soundEvents.Length; i++)
            {
                soundEvents[i].thisAudioManager = this;
                soundEvents[i].soundEventListener = new SoundVoidEventListener();
                soundEvents[i].soundEventListener.GameEvent = soundEvents[i].triggerEvent;
                soundEvents[i].soundEventListener.SoundEventClass = soundEvents[i];
                soundEvents[i].soundEventListener.Enable();
                soundEvents[i].SetUpEvent(i);
                soundEvents[i].audioSource = thisAudioSource;

                if (soundEvents[i].runOnStart)
                    soundEvents[i].EventRaised(0);
            }
    }

    public void StartCoroutine(int id)
    {
        if (soundEvents[id] != null)
            StartCoroutine(soundEvents[id].WaitForDelay());
    }

    public void ExternalRaiseAll()
    {
        for (int i = 0; i < soundEvents.Length; i++)
        {
            soundEvents[i].EventRaised(0);
        }
    }

    public void ExternalRaiseAtIndex(int index)
    {
        if (index >= 0 && index < soundEvents.Length)
            soundEvents[index].EventRaised(0);
    }
}

[System.Serializable]
public class SoundEvent
{
    #region Class Variables
    [HideInInspector] public AudioManager thisAudioManager;
    [HideInInspector] public AudioClip audioClip;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public int soundEventId;
    [HideInInspector] public SoundVoidEventListener soundEventListener;
    [HideInInspector] public float triggerDelay = 0.0f, soundVolume = 1.0f, soundPitch = 1.0f;
    [HideInInspector] public VoidEvent triggerEvent;
    [HideInInspector] public bool runOnce, runOnStart, loopSound, instantiated;

    private bool eventFired = false;
    private float parsedValue = 0;
    private WaitForSeconds triggerDelayWait; 
    #endregion

    public void SetUpEvent(int id)
    {
        triggerDelayWait = new WaitForSeconds(triggerDelay);
        soundEventId = id;
    }

    public void ResetEventFired()
    {
        eventFired = false;
    }

    public void EventRaised(float value)
    {
        parsedValue = value;

        if (!eventFired)
        {
            if (triggerDelay <= 0)
                PlaySound();
            else if (triggerDelay > 0)
                thisAudioManager.StartCoroutine(soundEventId);
            // Co-routines must be started from mono-behaviour class
        }

        if (runOnce)
        {
            eventFired = true;
            soundEventListener.Disable();
        }
    }

    private void PlaySound()
    {
        if (audioClip == null)
            return;

        audioSource.loop = loopSound;
        audioSource.clip = audioClip;
        audioSource.volume = soundVolume;
        audioSource.pitch = soundPitch;
        audioSource.Play();
        Debug.Log("Playing Sound: " + audioClip.name);
    }

    public IEnumerator WaitForDelay()
    {
        yield return triggerDelayWait;
        PlaySound();
    }
}

    #region SoundEventListener
    public abstract class SoundEventListener<T, E, SE> :
    IGameEventListener<T> where E : BaseGameEvent<T> where SE : SoundEvent
    {
        [SerializeField] private E gameEvent;
        [SerializeField] private SE soundEventClass;

        public E GameEvent
        {
            get { return gameEvent; }
            set { gameEvent = value; }
        }

        public SE SoundEventClass
        {
            get { return soundEventClass; }
            set { soundEventClass = value; }
        }

        public void Enable()
        {
            if (gameEvent == null)
            {
                return;
            }

            GameEvent.RegisterListener(this);
        }

        public void Disable()
        {
            if (gameEvent == null)
            {
                return;
            }

            GameEvent.UnregisterListener(this);
        }

        public void OnEventRaised(T item)
        {
            if (item.GetType() == typeof(Void))
            {
                SoundEventClass.EventRaised(0);
            }
            else if (item.GetType() == typeof(float))
            {
                // Currently disabled, no need for float events for now
                float tempFloat = float.Parse(item.ToString());
                // SoundEventClass.EventRaised(tempFloat);
            }
        }
    }

    public class SoundVoidEventListener : SoundEventListener<Void, VoidEvent, SoundEvent>
    {
    }
    #endregion