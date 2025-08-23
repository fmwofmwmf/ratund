using UnityEngine;
using UnityEngine.Events;

// A UnityEvent that passes an int
[System.Serializable]
public class IntEvent : UnityEvent<int> { }

// A UnityEvent that passes a string
[System.Serializable]
public class StringEvent : UnityEvent<string> { }
