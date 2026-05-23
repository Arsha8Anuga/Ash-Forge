using UnityEngine;

public class SnapPostProcessor
{
    private readonly GameObject owner;

    private readonly string defaultLayer;

    private readonly string snappedLayer;

    public SnapPostProcessor(
        GameObject owner,
        string defaultLayer,
        string snappedLayer)
    {
        this.owner = owner;
        this.defaultLayer = defaultLayer;
        this.snappedLayer = snappedLayer;
    }

    public void OnSnapped()
    {
        SnapLayerUtility.SetLayerRecursive(
            owner,
            snappedLayer
        );

        RefreshInteractable();
        RecalculateWeightChain();
    }

    public void OnUnsnapped()
    {
        SnapLayerUtility.SetLayerRecursive(
            owner,
            defaultLayer
        );

        RefreshInteractable();
        RecalculateWeightChain();
    }

    void RefreshInteractable()
    {
        InteractableObject interactable =
            owner.GetComponent<InteractableObject>();

        if (interactable != null)
        {
            interactable.RefreshRigidbody();
        }
    }

    void RecalculateWeightChain()
    {
        WeightChain chain =
            owner.GetComponentInParent<WeightChain>();

        if (chain != null)
        {
            chain.Recalculate();
        }
    }
}