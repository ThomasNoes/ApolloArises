using System;
using UnityEngine;

namespace EventCallbacks
{
    public abstract class Event<T> where T : Event<T> {

        public string Description;

        private bool hasFired;
        public delegate void EventListener(T info);
        private static event EventListener listeners;
        
        public static void RegisterListener(EventListener listener) {
            listeners += listener;
        }

        public static void UnregisterListener(EventListener listener) {
            listeners -= listener;
        }

        public void FireEvent() {
            if (hasFired) {
                throw new Exception("This event has already fired, to prevent infinite loops you can't refire an event");
            }
            hasFired = true;
            if (listeners != null) {
                listeners(this as T);
            }
            
        }
    }

    public class GenerateTerrainEvent : Event<GenerateTerrainEvent>
    {
        public GameObject go;
        public int[] wallArray;
        public float tileWidth;

    }

    public class FusePuzzlePlugIn : Event<FusePuzzlePlugIn>
    {
        public GameObject go;
        public GameObject sGo;
    }

    public class FusePuzzlePlugOut : Event<FusePuzzlePlugOut>
    {
        public GameObject go;
        public GameObject sGo;
    }
}