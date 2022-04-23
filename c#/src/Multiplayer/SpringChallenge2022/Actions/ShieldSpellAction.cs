namespace SpringChallenge2022.Actions
{
    public class ShieldSpellAction : IAction
    {
        private readonly int _id;
        private const int Range = 2200;

        public ShieldSpellAction(int id)
        {
            _id = id;
        }

        public string GetOutputAction()
            => $"SPELL SHIELD {_id}";
    }
}
