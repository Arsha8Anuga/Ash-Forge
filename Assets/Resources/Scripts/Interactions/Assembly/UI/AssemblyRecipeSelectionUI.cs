using System.Collections.Generic;
using UnityEngine;

public class AssemblyRecipeSelectionUI :
    MonoBehaviour
{
    [SerializeField]
    private GameObject root;

    [SerializeField]
    private Transform contentParent;

    [SerializeField]
    private AssemblyRecipeCard cardPrefab;

    [SerializeField]
    private bool debugLog;

    private AssemblyProcessor processor;

    private readonly List<AssemblyRecipeCard> cards =
        new List<AssemblyRecipeCard>();

    private bool isSelecting;

    void Awake()
    {
        Hide();
    }

    public void Bind(
        AssemblyProcessor processor)
    {
        this.processor = processor;
    }

    public void Show(
        List<AssemblyRecipeData> recipes)
    {
        Clear();

        isSelecting = false;

        if (root != null)
            root.SetActive(true);

        foreach (AssemblyRecipeData recipe
            in recipes)
        {
            if (recipe == null)
                continue;

            AssemblyRecipeCard card =
                Instantiate(
                    cardPrefab,
                    contentParent
                );

            card.Setup(
                recipe,
                HandleRecipeSelected
            );

            cards.Add(card);

            if (debugLog)
            {
                Debug.Log(
                    "[RecipeSelectionUI] Added card: " +
                    recipe.recipeName,
                    this
                );
            }
        }
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    void HandleRecipeSelected(
        AssemblyRecipeData recipe)
    {
        if (isSelecting)
            return;

        if (recipe == null)
            return;

        isSelecting = true;

        if (debugLog)
        {
            Debug.Log(
                "[RecipeSelectionUI] Selected: " +
                recipe.recipeName,
                this
            );
        }

        Hide();

        if (processor != null)
        {
            processor.AssembleRecipe(recipe);
        }

        Clear();

        isSelecting = false;
    }

    void Clear()
    {
        foreach (AssemblyRecipeCard card
            in cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        cards.Clear();
    }
}