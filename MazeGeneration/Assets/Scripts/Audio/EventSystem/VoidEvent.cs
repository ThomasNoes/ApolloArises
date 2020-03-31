using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Game Events/Void and Sound Event")]
public class VoidEvent : BaseGameEvent<Void>
{
    public void Raise() => Raise(new Void());
}