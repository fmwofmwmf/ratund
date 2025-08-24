using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    public Image crosshairImage;
    public Color defaultColor = Color.white;
    public Color interactableColor = Color.yellow;
    public float crosshairSize = 20f;
    
    [Header("Interaction Text Settings")]
    public TextMeshProUGUI interactionText;
    
    private RectTransform _crosshairRect;
    private bool _isInteractionAvailable = false;
    
    void Start()
    {
        SetupCrosshair();
        SetupInteractionText();
    }
    
    private void SetupCrosshair()
    {
        if (crosshairImage == null)
        {
            CreateCrosshair();
        }
        
        _crosshairRect = crosshairImage.GetComponent<RectTransform>();
        _crosshairRect.sizeDelta = new Vector2(crosshairSize, crosshairSize);
        
        _crosshairRect.anchorMin = new Vector2(0.5f, 0.5f);
        _crosshairRect.anchorMax = new Vector2(0.5f, 0.5f);
        _crosshairRect.anchoredPosition = Vector2.zero;
        
        SetCrosshairColor(defaultColor);
    }
    
    private void SetupInteractionText()
    {
        if (interactionText == null)
        {
            CreateInteractionText();
        }
        
        interactionText.text = "";
        interactionText.gameObject.SetActive(false);
    }

    
    private void CreateCrosshair()
    {
        Texture2D crosshairTexture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        for (int i = 0; i < 32; i++)
        {
            pixels[i * 32 + 16] = Color.white;
            pixels[16 * 32 + i] = Color.white;
        }
        
        crosshairTexture.SetPixels(pixels);
        crosshairTexture.Apply();
        
        Sprite crosshairSprite = Sprite.Create(crosshairTexture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        
        GameObject crosshairGO = new GameObject("Crosshair");
        crosshairGO.transform.SetParent(transform);
        crosshairImage = crosshairGO.AddComponent<Image>();
        crosshairImage.sprite = crosshairSprite;
    }
    
    private void CreateInteractionText()
    {
        GameObject textGO = new GameObject("InteractionText");
        textGO.transform.SetParent(transform);
        interactionText = textGO.AddComponent<TextMeshProUGUI>();
    }
    
    public void SetCrosshairColor(Color color)
    {
        if (crosshairImage != null)
        {
            crosshairImage.color = color;
        }
    }
    
    public void SetInteractableState(bool isLookingAtInteractable, string promptText = "")
    {
        _isInteractionAvailable = isLookingAtInteractable;
        
        // Update crosshair color
        SetCrosshairColor(isLookingAtInteractable ? interactableColor : defaultColor);
        
        // Update interaction text
        if (isLookingAtInteractable && !string.IsNullOrEmpty(promptText))
        {
            interactionText.text = promptText;
            interactionText.gameObject.SetActive(true);
        }
        else
        {
            interactionText.text = "";
            interactionText.gameObject.SetActive(false);
        }
    }
    
    public bool IsInteractionAvailable => _isInteractionAvailable;
}
