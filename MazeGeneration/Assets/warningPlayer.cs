using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class warningPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        GetComponent<TextMeshPro>().text = DataLogger2.GetPlayAreaSize().ToString();

    }


}
