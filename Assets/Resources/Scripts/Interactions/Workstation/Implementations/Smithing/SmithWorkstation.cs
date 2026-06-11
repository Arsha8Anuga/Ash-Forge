using System.Collections.Generic;
using UnityEngine;

public class SmithWorkstation : WorkstationBase
{
    [Header("Smithing")]
    [SerializeField]
    private SmithingAnvil anvil;

    [SerializeField]
    private bool requireAnvil = true;

    [SerializeField]
    private bool requireToolSteps = true;

    protected override void Awake()
    {
        base.Awake();

        if (anvil == null)
        {
            anvil =
                GetComponentInChildren
                <SmithingAnvil>();
        }
    }

    protected override bool CanStartRecipe(
        WorkstationRecipeData recipe,
        List<PhysicalItem> selectedItems)
    {
        if (!base.CanStartRecipe(
            recipe,
            selectedItems))
        {
            return false;
        }

        if (recipe == null)
            return false;

        if (requireAnvil &&
            (anvil == null || !anvil.IsReady))
        {
            Log("Smithing rejected: anvil is missing.");
            return false;
        }

        if (requireToolSteps &&
            !recipe.HasToolSteps)
        {
            Log("Smithing rejected: recipe has no hammer/tool steps.");
            return false;
        }

        return true;
    }
}
