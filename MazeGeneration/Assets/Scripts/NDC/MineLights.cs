using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineLights : MonoBehaviour {
    private Light[] lights;
    public GameObject directionalLight;

    void Start () {
        lights = gameObject.GetComponentsInChildren<Light> ();
    }

    void Update () {
        if (Input.GetKeyDown ("b")) {
            TurnOnLights ();
        }
        if (Input.GetKeyDown ("v")) {
            TurnOffLights ();
        }
    }

    public void TurnOnLights () {
        foreach (Light light in lights) {
            light.enabled = true;
            light.GetComponentInParent<Renderer> ().material.EnableKeyword ("_EMISSION");
            //FindObjectOfType<AudioManager> ().Play ("GeneratorStart");
            directionalLight.SetActive (true);
        }
    }

    public void TurnOffLights () {
        foreach (Light light in lights) {
            light.enabled = false;
            light.GetComponentInParent<Renderer> ().material.DisableKeyword ("_EMISSION");
            directionalLight.SetActive (false);
        }
    }
}