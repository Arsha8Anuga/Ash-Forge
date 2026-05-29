using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum WorkstationUIState
{
    Hidden,
    Selection,
    Processing,
    Message
}

public class WorkstationUI :
    MonoBehaviour
{
    [Header("Roots")]
    [SerializeField]
    private GameObject root;

    [SerializeField]
    private GameObject selectionRoot;

    [SerializeField]
    private GameObject processingRoot;

    [SerializeField]
    private GameObject messageRoot;

    [Header("Selection")]
    [SerializeField]
    private Transform contentParent;

    [SerializeField]
    private WorkstationRecipeCard cardPrefab;

    [Header("Texts")]
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text stepText;

    [SerializeField]
    private TMP_Text messageText;

    [Header("Timing")]
    [SerializeField]
    private float defaultMessageDuration = 1.25f;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    private WorkstationBase workstation;

    private readonly List<WorkstationRecipeCard> cards =
        new List<WorkstationRecipeCard>();

    private WorkstationUIState currentState =
        WorkstationUIState.Hidden;

    private WorkstationUIState previousState =
        WorkstationUIState.Hidden;

    private WorkstationRecipeData currentRecipe;

    private int currentStepIndex = -1;

    private Coroutine messageRoutine;

    private bool selecting;

    void Awake()
    {
        Hide();
    }

    public void Bind(
        WorkstationBase workstation)
    {
        this.workstation = workstation;
    }

    public void ShowSelection(
        List<WorkstationRecipeData> recipes)
    {
        ClearCards();

        selecting = false;

        previousState = currentState;
        currentState = WorkstationUIState.Selection;

        SetRootActive(true);

        SetPanelState(
            selection: true,
            processing: false,
            message: false
        );

        if (recipes == null)
            return;

        foreach (WorkstationRecipeData recipe
            in recipes)
        {
            if (recipe == null)
                continue;

            WorkstationRecipeCard card =
                Instantiate(
                    cardPrefab,
                    contentParent
                );

            card.Setup(
                recipe,
                HandleRecipeSelected
            );

            cards.Add(card);
        }

        Log(
            "Show selection: " +
            cards.Count
        );
    }

    public void ShowProcessing(
        WorkstationRecipeData recipe,
        int stepIndex)
    {
        currentRecipe = recipe;
        currentStepIndex = stepIndex;

        previousState = currentState;
        currentState = WorkstationUIState.Processing;

        ClearCards();

        SetRootActive(true);

        SetPanelState(
            selection: false,
            processing: true,
            message: false
        );

        if (titleText != null)
        {
            titleText.text =
                WorkstationTextBuilder
                .BuildRecipeHeader(recipe);
        }

        if (stepText != null)
        {
            stepText.text =
                WorkstationTextBuilder
                .BuildStepText(
                    recipe,
                    stepIndex
                );
        }

        Log("Show processing");
    }

    public void ShowStep(
        WorkstationRecipeData recipe,
        int stepIndex)
    {
        ShowProcessing(
            recipe,
            stepIndex
        );
    }

    public void ShowBusy()
    {
        ShowTemporaryMessage(
            "Machine busy",
            0.75f,
            true
        );
    }

    public void ShowDone()
    {
        ShowTemporaryMessage(
            "Done",
            defaultMessageDuration,
            false
        );
    }

    public void ShowNoRecipe()
    {
        ShowTemporaryMessage(
            "No matching recipe",
            defaultMessageDuration,
            false
        );
    }

    public void ShowFailed(
        string reason)
    {
        if (string.IsNullOrEmpty(reason))
            reason = "Process failed";

        ShowTemporaryMessage(
            reason,
            defaultMessageDuration,
            false
        );
    }

    public void ShowMessageTimed(
        string message,
        float duration)
    {
        ShowTemporaryMessage(
            message,
            duration,
            false
        );
    }

    public void ShowMessage(
        string message)
    {
        ShowMessageImmediate(message);
    }

    void ShowTemporaryMessage(
        string message,
        float duration,
        bool restorePrevious)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        previousState = currentState;

        messageRoutine =
            StartCoroutine(
                TemporaryMessageRoutine(
                    message,
                    duration,
                    restorePrevious
                )
            );
    }

    IEnumerator TemporaryMessageRoutine(
        string message,
        float duration,
        bool restorePrevious)
    {
        ShowMessageImmediate(message);

        yield return new WaitForSeconds(
            Mathf.Max(
                0.1f,
                duration
            )
        );

        messageRoutine = null;

        if (restorePrevious)
        {
            RestorePreviousState();
        }
        else
        {
            Hide();
        }
    }

    void ShowMessageImmediate(
        string message)
    {
        currentState = WorkstationUIState.Message;

        SetRootActive(true);

        SetPanelState(
            selection: false,
            processing: false,
            message: true
        );

        if (messageText != null)
            messageText.text = message;

        Log("Message: " + message);
    }

    void RestorePreviousState()
    {
        if (previousState ==
            WorkstationUIState.Processing)
        {
            ShowProcessing(
                currentRecipe,
                currentStepIndex
            );

            return;
        }

        Hide();
    }

    void HandleRecipeSelected(
        WorkstationRecipeData recipe)
    {
        if (selecting)
            return;

        if (recipe == null)
            return;

        selecting = true;

        ClearCards();

        if (workstation != null)
        {
            workstation.SelectRecipe(
                recipe
            );
        }

        selecting = false;
    }

    public void Hide()
    {
        currentState =
            WorkstationUIState.Hidden;

        ClearCards();

        SetPanelState(
            selection: false,
            processing: false,
            message: false
        );

        SetRootActive(false);
    }

    void SetRootActive(
        bool active)
    {
        if (root != null)
            root.SetActive(active);
    }

    void SetPanelState(
        bool selection,
        bool processing,
        bool message)
    {
        if (selectionRoot != null)
            selectionRoot.SetActive(selection);

        if (processingRoot != null)
            processingRoot.SetActive(processing);

        if (messageRoot != null)
            messageRoot.SetActive(message);
    }

    void ClearCards()
    {
        foreach (WorkstationRecipeCard card
            in cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        cards.Clear();
    }

    void Log(
        string message)
    {
        if (!debugLog)
            return;

        Debug.Log(
            "[WorkstationUI] " +
            message,
            this
        );
    }
}