namespace FallChallenge2020.Models
{
    internal class Scroll
    {
        public int ActionId { get; }

        public int[] IngredientsCost { get; }

        public int Tax { get; }

        public bool Repeatable { get; }

        public override string ToString()
        {
            return $"Scroll {ActionId} {IngredientsCost[0]} {IngredientsCost[1]} {IngredientsCost[2]} {IngredientsCost[3]} {Tax}";
        }
        public Scroll(
            int actionId,
            int tier0IngredientCost,
            int tier1IngredientCost,
            int tier2IngredientCost,
            int tier3IngredientCost,
            int tax,
            bool repeatable)
        {
            ActionId = actionId;
            IngredientsCost = new[] { tier0IngredientCost, tier1IngredientCost, tier2IngredientCost, tier3IngredientCost };
            Tax = tax;
            Repeatable = repeatable;
        }

    }
}
