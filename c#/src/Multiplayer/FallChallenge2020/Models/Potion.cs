namespace FallChallenge2020.Models
{
    internal class Potion
    {
        public Potion(
            int actionId,
            int tier0IngredientCost,
            int tier1IngredientCost,
            int tier2IngredientCost,
            int tier3IngredientCost,
            int price)
        {
            ActionId = actionId;
            Price = price;
            IngredientsCost = new[] { tier0IngredientCost, tier1IngredientCost, tier2IngredientCost, tier3IngredientCost };
        }

        public int ActionId { get; }

        public int Price { get; }

        public int[] IngredientsCost { get; }

        public override string ToString()
        {
            return $"Potion {ActionId} {IngredientsCost} {Price}";
        }
    }
}
