using UnityEngine;

public class Yapper : MonoBehaviour
{
    public string text;

    public void Yap()
    {
        MessageDisplay.Enqueue(text);
    }
}
