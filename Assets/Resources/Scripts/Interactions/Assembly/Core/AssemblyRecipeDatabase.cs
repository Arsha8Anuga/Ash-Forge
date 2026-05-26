using UnityEngine;

public class AssemblyRecipeDatabase :
    MonoBehaviour
{
    [SerializeField]
    private AssemblyRecipeData[] recipes;

    public AssemblyRecipeData[] Recipes =>
        recipes;
}