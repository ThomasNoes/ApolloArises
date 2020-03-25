using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Data", menuName = "Dialog Data")]
public class DialogData : ScriptableObject
{
    [TextArea(4, 4)]
    public string text;

    [Space(10)]

    [Header("How fast should the text be?")]

    public int textSpeed = 10;

    [Space(10)]

    [Header("Choose an effect")]

    public effect dialogEffect = 0;

    public enum effect : int
    {
        none = 0,
        success = 1,
        surprise = 2,
        confetti = 3,
        question = 4,
        remark = 5
    }


}
