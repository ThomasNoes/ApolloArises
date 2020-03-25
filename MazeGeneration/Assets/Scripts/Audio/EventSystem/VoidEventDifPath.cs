using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Game Events/Void Event")]
public class VoidEventDifPath : BaseGameEvent<Void>
{
    public void Raise() => Raise(new Void());
}