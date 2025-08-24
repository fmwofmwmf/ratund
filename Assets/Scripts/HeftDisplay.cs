using UnityEngine;

public class HeftDisplay : MonoBehaviour
{
    public GameObject progressDisplay;
    private float MAX_HEFT = 1000f;
    public float maxProgressDisplayWidth = 350f;

    public void updateHeft()
    {
        float playerHeft = Player.player != null ? Player.player.heft : 0f;

        float progress = Mathf.Clamp01(playerHeft / MAX_HEFT);
        float barWidth = progress * maxProgressDisplayWidth;

        Debug.Log($"Updating heft display: {playerHeft} / {MAX_HEFT} = {progress}, bar width = {barWidth}");

        RectTransform rt = progressDisplay.GetComponent<RectTransform>();
        if (rt != null)
        {
            Vector2 size = rt.sizeDelta;
            size.x = barWidth;
            rt.sizeDelta = size;
        }
    }
}
