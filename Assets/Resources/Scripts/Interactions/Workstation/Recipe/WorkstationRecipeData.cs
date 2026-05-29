using UnityEngine;

[CreateAssetMenu(
    menuName = "Workstation/Recipe"
)]
public class WorkstationRecipeData :
    ScriptableObject
{
    [Header("Identity")]
    public string recipeId;

    public string recipeName;

    [TextArea]
    public string description;

    [Header("Recipe Type")]
    public WorkstationRecipeType recipeType =
        WorkstationRecipeType.Assembly;

    [Header("Reversibility")]
    public WorkstationReversibility reversibility =
        WorkstationReversibility.None;

    public bool isDestructive;

    [TextArea]
    public string ruleNote;

    [Header("Input")]
    public WorkstationRequirement[] requirements;

    [Header("Output")]
    public WorkstationOutput[] outputs;

    [Header("Timed Process")]
    public float processTime = 3f;

    [Header("Tool Process")]
    public WorkstationToolStep[] toolSteps;

    [Header("Quality")]
    [Range(0f, 100f)]
    public float baseQuality = 80f;

    [Range(0f, 100f)]
    public float baseDefect = 0f;

    public bool HasToolSteps =>
        toolSteps != null &&
        toolSteps.Length > 0;

    public bool HasOutput =>
        outputs != null &&
        outputs.Length > 0;

    public bool IsDisassembly =>
        recipeType ==
        WorkstationRecipeType.Disassembly;

    public bool IsAssembly =>
        recipeType ==
        WorkstationRecipeType.Assembly;

    public bool CanReverse =>
        reversibility ==
        WorkstationReversibility.Full ||
        reversibility ==
        WorkstationReversibility.Partial;
}