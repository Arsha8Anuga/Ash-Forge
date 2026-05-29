using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkstationRecipeCard : MonoBehaviour
{
    [Header("Text")]
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text requirementText;

    [SerializeField]
    private TMP_Text outputText;

    [SerializeField]
    private TMP_Text stepText;

    [Header("Button")]
    [SerializeField]
    private Button button;

    [Header("Debug")]
    [SerializeField]
    private bool debugLog;

    private WorkstationRecipeData recipe;

    private Action<WorkstationRecipeData> onSelected;

    private bool clicked;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    public void Setup(
        WorkstationRecipeData recipe,
        Action<WorkstationRecipeData> onSelected)
    {
        this.recipe = recipe;
        this.onSelected = onSelected;

        clicked = false;

        RefreshText();
        RefreshButton();
    }

    void RefreshText()
    {
        if (titleText != null)
        {
            titleText.text =
                recipe != null
                ? recipe.recipeName
                : "Unknown Recipe";
        }

        if (requirementText != null)
        {
            requirementText.text =
                WorkstationTextBuilder
                .BuildRequirementText(recipe);
        }

        if (outputText != null)
        {
            outputText.text =
                WorkstationTextBuilder
                .BuildOutputText(recipe);
        }

        if (stepText != null)
        {
            stepText.text =
                WorkstationTextBuilder
                .BuildStepText(
                    recipe,
                    -1
                );
        }
    }

    void RefreshButton()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button == null)
            return;

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(
            HandleClick
        );

        button.interactable = true;
    }

    void HandleClick()
    {
        if (clicked)
            return;

        clicked = true;

        if (button != null)
            button.interactable = false;

        if (debugLog)
        {
            Debug.Log(
                "[WorkstationRecipeCard] Selected: " +
                (
                    recipe != null
                    ? recipe.recipeName
                    : "NULL"
                ),
                this
            );
        }

        onSelected?.Invoke(recipe);
    }
}