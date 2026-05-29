using System.Collections.Generic;
using UnityEngine;

public class ToolSequenceExecution :
    WorkstationExecutionBase,
    IWorkstationToolReceiver
{
    [SerializeField]
    private float defaultCooldown = 0.25f;

    [SerializeField]
    private bool debugLog;

    private int currentStepIndex;

    private float nextStepTime;

    private bool running;

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

        currentStepIndex = 0;

        running = true;

        nextStepTime =
            Time.time + defaultCooldown;

        if (recipe == null)
        {
            FailExecution(
                "Recipe is null."
            );

            return;
        }

        if (recipe.toolSteps == null ||
            recipe.toolSteps.Length == 0)
        {
            Log("No tool steps. Completing instantly.");

            CompleteExecution();

            return;
        }

        workstation.ShowStep(
            recipe,
            currentStepIndex
        );

        Log(
            "Begin execution: " +
            recipe.recipeName
        );
    }

    public void ReceiveToolAction(
        WorkstationTool tool,
        bool triggerHeld,
        float holdDuration)
    {
        if (!IsRunning)
            return;
            
        WorkstationToolHit hit =
            new WorkstationToolHit
            {
                tool = tool,
                item = null,
                hitItem = false,
                position = tool != null
                    ? tool.transform.position
                    : Vector3.zero,
                velocity = tool != null
                    ? tool.GetVelocity()
                    : 0f
            };

        ProcessToolStep(
            hit,
            triggerHeld,
            holdDuration
        );
    }

    public void ReceiveToolHit(
        WorkstationToolHit hit)
    {
        if (!IsRunning)
            return;

        bool triggerHeld = false;

        float holdDuration = 0f;

        if (hit.tool != null)
        {
            triggerHeld =
                hit.tool.TriggerHeld;

            holdDuration =
                hit.tool.HoldDuration;
        }

        ProcessToolStep(
            hit,
            triggerHeld,
            holdDuration
        );
    }

    void ProcessToolStep(
        WorkstationToolHit hit,
        bool triggerHeld,
        float holdDuration)
    {
        if (!running)
        {
            Log("Rejected: execution not running.");
            return;
        }

        if (workstation == null ||
            recipe == null)
        {
            Log("Rejected: missing workstation or recipe.");
            return;
        }

        if (Time.time < nextStepTime)
        {
            Log("Rejected: cooldown.");
            return;
        }

        WorkstationToolStep step =
            GetCurrentStep();

        if (step == null)
        {
            Log("Rejected: no current step.");
            return;
        }

        if (!ValidateStep(
            step,
            hit,
            triggerHeld,
            holdDuration))
        {
            return;
        }

        CompleteCurrentStep(step);
    }

    WorkstationToolStep GetCurrentStep()
    {
        if (recipe == null)
            return null;

        if (recipe.toolSteps == null)
            return null;

        if (currentStepIndex < 0 ||
            currentStepIndex >=
            recipe.toolSteps.Length)
        {
            return null;
        }

        return recipe.toolSteps[currentStepIndex];
    }

    bool ValidateStep(
        WorkstationToolStep step,
        WorkstationToolHit hit,
        bool triggerHeld,
        float holdDuration)
    {
        if (step == null)
            return false;

        if (hit.tool == null)
        {
            Log("Rejected: tool is null.");
            return false;
        }

        if (hit.tool.ToolType !=
            step.toolType)
        {
            Log(
                "Rejected: wrong tool. Expected " +
                step.toolType +
                ", got " +
                hit.tool.ToolType
            );

            return false;
        }

        if (!hit.tool.IsActive)
        {
            Log("Rejected: tool is not active.");
            return false;
        }

        if (!IsContactValid(
            step,
            hit))
        {
            Log("Rejected: invalid contact mode.");
            return false;
        }

        if (step.requiresHold)
        {
            if (!triggerHeld)
            {
                Log("Rejected: trigger not held.");
                return false;
            }

            if (holdDuration <
                step.holdDuration)
            {
                Log(
                    "Rejected: hold too short. " +
                    holdDuration +
                    " / " +
                    step.holdDuration
                );

                return false;
            }
        }

        if (step.useVelocityCheck)
        {
            float velocity =
                hit.velocity;

            if (velocity < step.minVelocity)
            {
                Log(
                    "Rejected: velocity too low. " +
                    velocity
                );

                return false;
            }

            if (velocity > step.maxVelocity)
            {
                Log(
                    "Rejected: velocity too high. " +
                    velocity
                );

                return false;
            }
        }

        return true;
    }

    bool IsContactValid(
        WorkstationToolStep step,
        WorkstationToolHit hit)
    {
        switch (step.contactMode)
        {
            case WorkstationToolContactMode.ZoneOnly:
                return true;

            case WorkstationToolContactMode.RequireItemContact:
                return hit.hitItem &&
                    hit.item != null;

            case WorkstationToolContactMode.Either:
                return true;
        }

        return false;
    }

    void CompleteCurrentStep(
        WorkstationToolStep step)
    {
        Log(
            "Step completed: " +
            currentStepIndex +
            " " +
            step.toolType
        );

        currentStepIndex++;

        nextStepTime =
            Time.time +
            Mathf.Max(
                defaultCooldown,
                step.stepCooldown
            );

        workstation.ShowStep(
            recipe,
            currentStepIndex
        );

        if (currentStepIndex >=
            recipe.toolSteps.Length)
        {
            CompleteExecution();
        }
    }

    void CompleteExecution()
    {
        if (!running)
            return;

        running = false;

        Log("Execution completed.");

        Complete();
    }

    void FailExecution(
        string reason)
    {
        running = false;

        Log("Execution failed: " + reason);
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[ToolSequenceExecution] " +
            message,
            this
        );
    }

    public override void Cancel()
    {
        base.Cancel();

        currentStepIndex = 0;

        nextStepTime = 0f;
    }
}