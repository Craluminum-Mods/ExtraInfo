namespace ExtraInfo;

public static class CollectibleExtensions
{
    public static bool Matches(this CreatureDiet diet, CollectibleObject collectible)
    {
        EnumFoodCategory foodSourceCategory = collectible.NutritionProps?.FoodCategory ?? EnumFoodCategory.NoNutrition;
        string[] foodSourceTags = collectible.Attributes?["foodTags"].AsArray<string>();
        return diet.Matches(foodSourceCategory, foodSourceTags);
    }
}