using UnityEngine;

[CreateAssetMenu(
    menuName = "Assembly/Recipe Data"
)]
public class AssemblyRecipeData :
    ScriptableObject
{
    [Header("Identity")]
    public string recipeId;

    public string recipeName;

    [Header("Input")]
    public AssemblyRequirement[] requirements;

    [Header("Output")]
    public AssemblyOutput output;

    [Header("Production")]
    public float productionQuality = 80f;

    public float defectChance = 0.1f;
}