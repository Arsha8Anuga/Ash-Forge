using System.Collections.Generic;
using UnityEngine;

public abstract class WorkstationExecutionBase :
    MonoBehaviour
{
    protected WorkstationBase workstation;

    protected WorkstationRecipeData recipe;

    protected List<PhysicalItem> items;

    public bool IsRunning
    {
        get;
        private set;
    }

    public virtual void Begin(
        WorkstationBase workstation,
        WorkstationRecipeData recipe,
        List<PhysicalItem> items)
    {
        this.workstation = workstation;
        this.recipe = recipe;
        this.items = items;

        IsRunning = true;
    }

    protected void Complete()
    {
        if (!IsRunning)
            return;

        IsRunning = false;

        if (workstation != null)
            workstation.CompleteWork();
    }

    public virtual void Cancel()
    {
        IsRunning = false;
    }
}