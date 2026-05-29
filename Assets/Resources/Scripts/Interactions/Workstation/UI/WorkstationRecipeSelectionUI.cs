using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkstationRecipeSelectionUI :
    MonoBehaviour
{
    [SerializeField]
    private GameObject root;

    [SerializeField]
    private Transform contentParent;

    [SerializeField]
    private WorkstationRecipeCard cardPrefab;

    [SerializeField]
    private bool debugLog;

    private readonly List<WorkstationRecipeCard> cards =
        new List<WorkstationRecipeCard>();

    private Action<WorkstationRecipeData> onSelected;

    private bool isSelecting;

    void Awake()
    {
        Hide();
    }

    public void Show(
        List<WorkstationRecipeData> recipes,
        Action<WorkstationRecipeData> onSelected)
    {
        Clear();

        this.onSelected =
            onSelected;

        isSelecting = false;

        if (root != null)
            root.SetActive(true);

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

            if (debugLog)
            {
                Debug.Log(
                    "[WorkstationRecipeSelectionUI] Added card: " +
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
        WorkstationRecipeData recipe)
    {
        if (isSelecting)
            return;

        if (recipe == null)
            return;

        isSelecting = true;

        if (debugLog)
        {
            Debug.Log(
                "[WorkstationRecipeSelectionUI] Selected: " +
                recipe.recipeName,
                this
            );
        }

        Hide();

        onSelected?.Invoke(recipe);

        Clear();

        isSelecting = false;
    }

    void Clear()
    {
        foreach (WorkstationRecipeCard card
            in cards)
        {
            if (card != null)
            {
                Destroy(
                    card.gameObject
                );
            }
        }

        cards.Clear();
    }
}