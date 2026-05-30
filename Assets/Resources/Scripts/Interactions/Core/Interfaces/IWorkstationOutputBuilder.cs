using System.Collections.Generic;

public interface IWorkstationOutputBuilder
{
    bool CanBuildOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems
    );

    List<StoredItemStack> BuildOutputs(
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems
    );
}