public class AssemblyRecipeMatch
{
    public AssemblyRecipeData Recipe
    {
        get;
        private set;
    }

    public bool IsFullyMatched
    {
        get;
        private set;
    }

    public float Score
    {
        get;
        private set;
    }

    public int MatchedAmount
    {
        get;
        private set;
    }

    public int RequiredAmount
    {
        get;
        private set;
    }

    public int MissingAmount
    {
        get;
        private set;
    }

    public AssemblyRecipeMatch(
        AssemblyRecipeData recipe,
        bool isFullyMatched,
        float score,
        int matchedAmount,
        int requiredAmount,
        int missingAmount)
    {
        Recipe = recipe;
        IsFullyMatched = isFullyMatched;
        Score = score;
        MatchedAmount = matchedAmount;
        RequiredAmount = requiredAmount;
        MissingAmount = missingAmount;
    }
}