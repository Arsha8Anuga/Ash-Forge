public static class WorkstationTextBuilder
{
    public static string BuildRecipeHeader(
        WorkstationRecipeData recipe)
    {
        if (recipe == null)
            return "Recipe: ?";

        return
            recipe.recipeName +
            "\nType: " +
            recipe.recipeType +
            "\nReversible: " +
            recipe.reversibility;
    }

    public static string BuildRequirementText(
        WorkstationRecipeData recipe)
    {
        if (recipe == null ||
            recipe.requirements == null ||
            recipe.requirements.Length == 0)
        {
            return "Input: ?";
        }

        string text = "Input:\n";

        foreach (WorkstationRequirement req
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

    public static string BuildOutputText(
        WorkstationRecipeData recipe)
    {
        if (recipe == null ||
            recipe.outputs == null ||
            recipe.outputs.Length == 0)
        {
            return "Output: ?";
        }

        string text = "Output:\n";

        foreach (WorkstationOutput output
            in recipe.outputs)
        {
            if (output == null ||
                output.itemData == null)
                continue;

            text +=
                "- " +
                output.itemData.itemName +
                " x" +
                output.amount +
                "\n";
        }

        return text;
    }

    public static string BuildRuleText(
        WorkstationRecipeData recipe)
    {
        if (recipe == null)
            return "";

        string text =
            "Rule:\n" +
            recipe.reversibility;

        if (recipe.isDestructive)
            text += "\nDestructive";

        if (!string.IsNullOrEmpty(
            recipe.ruleNote))
        {
            text +=
                "\n" +
                recipe.ruleNote;
        }

        return text;
    }

    public static string BuildStepText(
        WorkstationRecipeData recipe,
        int activeIndex)
    {
        if (recipe == null ||
            recipe.toolSteps == null ||
            recipe.toolSteps.Length == 0)
        {
            return "Steps: Auto";
        }

        string text = "Steps:\n";

        for (int i = 0;
            i < recipe.toolSteps.Length;
            i++)
        {
            WorkstationToolStep step =
                recipe.toolSteps[i];

            if (step == null)
                continue;

            string prefix =
                i == activeIndex
                ? "> "
                : "- ";

            string done =
                i < activeIndex
                ? " done"
                : "";

            text +=
                prefix +
                step.toolType +
                done +
                "\n";
        }

        return text;
    }
}