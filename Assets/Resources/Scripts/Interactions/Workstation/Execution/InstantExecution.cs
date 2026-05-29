using System.Collections.Generic;
using UnityEngine;

public class InstantExecution :
    WorkstationExecutionBase
{
    public override void Begin(
        WorkstationBase workstation,
        WorkstationRecipeData recipe,
        List<PhysicalItem> items)
    {
        base.Begin(
            workstation,
            recipe,
            items
        );

        Complete();
    }
}