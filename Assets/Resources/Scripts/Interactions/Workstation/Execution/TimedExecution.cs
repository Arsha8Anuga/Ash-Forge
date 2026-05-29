using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedExecution :
    WorkstationExecutionBase
{
    private Coroutine routine;

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

        if (routine != null)
            StopCoroutine(routine);

        routine =
            StartCoroutine(Process());
    }

    IEnumerator Process()
    {
        float time =
            Mathf.Max(
                0.1f,
                recipe.processTime
            );

        yield return new WaitForSeconds(time);

        routine = null;

        Complete();
    }

    public override void Cancel()
    {
        base.Cancel();

        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }
}