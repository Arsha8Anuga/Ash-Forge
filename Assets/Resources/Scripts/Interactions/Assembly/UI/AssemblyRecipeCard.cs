using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssemblyRecipeCard :
    MonoBehaviour
{
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text requirementText;

    [SerializeField]
    private TMP_Text outputText;

    [SerializeField]
    private Button button;

    [SerializeField]
    private bool debugLog;

    private AssemblyRecipeData recipe;

    private Action<AssemblyRecipeData> onSelected;

    private bool clicked;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    public void Setup(
        AssemblyRecipeData recipe,
        Action<AssemblyRecipeData> onSelected)
    {
        this.recipe = recipe;
        this.onSelected = onSelected;

        clicked = false;

        if (titleText != null)
            titleText.text = recipe.recipeName;

        if (requirementText != null)
            requirementText.text =
                BuildRequirementText(recipe);

        if (outputText != null)
            outputText.text =
                BuildOutputText(recipe);

        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(HandleClick);
            button.interactable = true;
        }
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
                "[RecipeCard] Clicked: " +
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

    string BuildRequirementText(
        AssemblyRecipeData recipe)
    {
        if (recipe == null ||
            recipe.requirements == null)
            return "Input: ?";

        string text = "Input:\n";

        foreach (AssemblyRequirement req
            in recipe.requirements)
        {
            if (req == null ||
                req.itemData == null)
                continue;

            text +=
                "- " +
                req.itemData.itemName +
                " x" +
                req.amount +
                "\n";
        }

        return text;
    }

    string BuildOutputText(
        AssemblyRecipeData recipe)
    {
        if (recipe == null ||
            recipe.output == null ||
            recipe.output.itemData == null)
            return "Output: ?";

        return
            "Output:\n" +
            recipe.output.itemData.itemName +
            " x" +
            recipe.output.amount;
    }
}