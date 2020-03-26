using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LabelName : MonoBehaviour
{

    TextMeshProUGUI text;
    Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = transform.parent.parent.name;

        canvas = transform.parent.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

}
