using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

	public Sound[] sounds;
    float currentTime;
    public float delayTime = 4;
    string delayedSound;
    bool timerActive = false;

	void Awake()
	{

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

		}

        Play("AmbientSound");
    }

    private void Update()
    {
        if (timerActive)
        {
            if (delayTime <= Time.time - currentTime)
            {
                timerActive = false;
                Play(delayedSound);
            }
        }
    }

    public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);

        if (s == null)
		{
			Debug.LogWarning("Sound: " +name + " not found!");
			return;
		}

        s.source.volume = s.volume;
        s.source.pitch = s.pitch;

		s.source.Play();
	}

    public void DelayedPlay(string sound)
    {
        currentTime = Time.time;
        // delayTime = seconds;
        delayedSound = sound;
        timerActive = true;
    }

}
