using UnityEngine;
using UnityEngine.Events;

// A UnityEvent that passes an int
[System.Serializable]
public class IntUnityEvent : UnityEvent<int> { }

// A UnityEvent that passes a float
[System.Serializable]
public class FloatUnityEvent : UnityEvent<float> { }

// A UnityEvent that passes a string
[System.Serializable]
public class StringUnityEvent : UnityEvent<string> { }
