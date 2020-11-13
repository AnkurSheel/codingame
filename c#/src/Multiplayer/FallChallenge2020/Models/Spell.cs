namespace FallChallenge2020.Models
{
    internal class Spell
    {
        public Spell(
            int actionId,
            int tier0IngredientCost,
            int tier1IngredientCost,
            int tier2IngredientCost,
            int tier3IngredientCost,
            bool castable)
        {
            ActionId = actionId;
            Castable = castable;
            IngredientsCost = new[] { tier0IngredientCost, tier1IngredientCost, tier2IngredientCost, tier3IngredientCost };
        }

        public int ActionId { get; }

        public bool Castable { get; }

        public int[] IngredientsCost { get; }

        public override string ToString()
        {
            return $"Spell {ActionId} {IngredientsCost} {Castable}";
        }
    }
}
