using System;
using UnityEngine;

public class OutlineToggle : MonoBehaviour
{
    [Header("Outline Settings")]
    public Material outlineMaterial;   // The material with outline shader
    private Material[] originalMaterials;

    private Renderer rend;
    private bool outlined = false;
    public bool disableAtStart;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterials = rend.materials;
        }
    }

    private void Start()
    {
        DisableOutline();
    }

    /// <summary>
    /// Enable outline effect.
    /// </summary>
    public void EnableOutline()
    {
        if (rend == null || outlineMaterial == null || outlined) return;

        // Create new material array with outline appended
        Material[] mats = new Material[originalMaterials.Length + 1];
        for (int i = 0; i < originalMaterials.Length; i++)
            mats[i] = originalMaterials[i];
        mats[mats.Length - 1] = outlineMaterial;

        rend.materials = mats;
        outlined = true;
    }

    /// <summary>
    /// Disable outline effect.
    /// </summary>
    public void DisableOutline()
    {
        if (rend == null || !outlined) return;

        rend.materials = originalMaterials;
        outlined = false;
    }

    /// <summary>
    /// Toggle outline on/off.
    /// </summary>
    public void ToggleOutline()
    {
        if (outlined)
            DisableOutline();
        else
            EnableOutline();
    }
}
