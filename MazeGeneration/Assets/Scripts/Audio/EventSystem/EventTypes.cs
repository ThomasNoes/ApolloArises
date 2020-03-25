using UnityEngine.Events;

public class EventTypes {}

[System.Serializable] public struct Void { }
[System.Serializable] public class UnityVoidEvent : UnityEvent<Void> { }
[System.Serializable] public class UnityIntEvent : UnityEvent<int> { }
[System.Serializable] public class UnityFloatEvent : UnityEvent<float> { }