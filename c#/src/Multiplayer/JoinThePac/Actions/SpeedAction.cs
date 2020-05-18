namespace JoinThePac.Actions
{
    internal class SpeedAction : IAction
    {
        public string GetAction(int id)
        {
            return $"SPEED {id}";
        }
    }
}
