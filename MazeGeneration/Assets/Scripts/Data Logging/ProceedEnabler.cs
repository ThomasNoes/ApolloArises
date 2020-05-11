using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceedEnabler : MonoBehaviour
{
    private void Start()
    {
        Invoke("DeactivateButton",5.0f);
    }

    public void DeactivateButton()
    {
        gameObject.SetActive(false);
    }

    public void ActivateButton()
    {
        gameObject.SetActive(true);
    }

    public void ActivateAfterTime(float time)
    {
        Invoke("ActivateButton", time);
    }
}
