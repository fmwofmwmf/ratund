using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public Outline toggle;
    public string _interactionPrompt = "Press E to interact";
    public float _interactionDistance = 3f;
    public bool _canInteract = true;
    
    public UnityEvent onInteracted;
    
    public string InteractionPrompt => _interactionPrompt;
    public float InteractionDistance => _interactionDistance;
    public bool CanInteract => _canInteract;
    
    public void Interact(GameObject player)
    {
        if (_canInteract)
        {
            Debug.Log($"Player interacted with {gameObject.name}");
            onInteracted?.Invoke();
        }
    }
    
    public void SetCanInteract(bool value)
    {
        _canInteract = value;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _canInteract ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, _interactionDistance);
    }
}
