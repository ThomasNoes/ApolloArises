using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Sound Events/Sound Event")]
public class VoidEvent : BaseGameEvent<Void>
{
    public void Raise() => Raise(new Void());
}