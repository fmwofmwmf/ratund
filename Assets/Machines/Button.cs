using UnityEngine;
using UnityEngine.Events;

public class PressableButton : MonoBehaviour
{
    [Header("Button Settings")]
    public Transform buttonTop;        // The moving part of the button
    public float pressDepth = 0.1f;    // How far it moves down
    public float pressSpeed = 10f;     // How fast it moves
    public float returnSpeed = 5f;     // How fast it goes back up
    
    [Header("Events")]
    public UnityEvent onPressed;

    private Vector3 initialPos;
    private bool isPressed = false;
    private int objectsOnButton = 0; // track how many objects are on the button

    void Start()
    {
        if (buttonTop == null) buttonTop = transform;
        initialPos = buttonTop.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        objectsOnButton++;
        if (!isPressed)
        {
            StopAllCoroutines();
            StartCoroutine(PressAnimation());
        }
    }

    void OnTriggerExit(Collider other)
    {
        objectsOnButton = Mathf.Max(0, objectsOnButton - 1);

        if (objectsOnButton == 0 && isPressed)
        {
            StopAllCoroutines();
            StartCoroutine(ReturnAnimation());
        }
    }

    System.Collections.IEnumerator PressAnimation()
    {
        isPressed = true;

        Vector3 targetPos = initialPos - new Vector3(0, 0, pressDepth);

        // Move down
        while (Vector3.Distance(buttonTop.localPosition, targetPos) > 0.001f)
        {
            buttonTop.localPosition = Vector3.MoveTowards(
                buttonTop.localPosition,
                targetPos,
                pressSpeed * Time.deltaTime
            );
            yield return null;
        }

        buttonTop.localPosition = targetPos; // snap cleanly
        onPressed?.Invoke(); // 🔔 Fire event ONCE
    }

    System.Collections.IEnumerator ReturnAnimation()
    {
        Vector3 targetPos = initialPos;

        // Move up
        while (Vector3.Distance(buttonTop.localPosition, targetPos) > 0.001f)
        {
            buttonTop.localPosition = Vector3.MoveTowards(
                buttonTop.localPosition,
                targetPos,
                returnSpeed * Time.deltaTime
            );
            yield return null;
        }

        buttonTop.localPosition = targetPos; // snap cleanly
        isPressed = false;
    }
}
