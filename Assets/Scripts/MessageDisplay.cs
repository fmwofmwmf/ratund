using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MessageDisplay : MonoBehaviour
{
    public static MessageDisplay Instance; // 🔥 static reference

    [Header("UI References")]
    public TMP_Text uiText;             // TMP text component
    public float typeSpeed = 0.05f;     // Time between letters
    public float messageDelay = 1.5f;   // Delay after a message finishes before the next starts

    private Queue<string> messageQueue = new Queue<string>();
    private bool isDisplaying = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (uiText != null)
            uiText.text = "";
    }

    /// <summary>
    /// Add a message to the queue.
    /// </summary>
    public static void Enqueue(string message)
    {
        if (Instance == null)
        {
            Debug.LogWarning("MessageDisplayTMP not in scene!");
            return;
        }

        Instance.messageQueue.Enqueue(message);

        if (!Instance.isDisplaying)
        {
            Instance.StartCoroutine(Instance.ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isDisplaying = true;

        while (messageQueue.Count > 0)
        {
            string nextMessage = messageQueue.Dequeue();
            yield return StartCoroutine(TypeMessage(nextMessage));
            yield return new WaitForSeconds(messageDelay);
        }
        uiText.gameObject.SetActive(false);
        isDisplaying = false;
    }

    private IEnumerator TypeMessage(string message)
    {
        uiText.text = "";
        foreach (char c in message)
        {
            uiText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
}