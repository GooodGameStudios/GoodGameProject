using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Highlightable : MonoBehaviour
{
    [SerializeField] private string outlineLayerName = "Outline";

    private int outlineLayer;
    private int defaultLayer;
    private bool isHighlighted;

    private void Awake()
    {
        defaultLayer = gameObject.layer;
        outlineLayer = LayerMask.NameToLayer(outlineLayerName);
    }

    public void SetHighlighted(bool highlighted)
    {
        if (isHighlighted == highlighted) return;
        if (outlineLayer < 0) return;

        isHighlighted = highlighted;
        gameObject.layer = highlighted ? outlineLayer : defaultLayer;
    }
}
