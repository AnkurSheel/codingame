using System;
using System.Collections.Generic;
using System.Text;

namespace FallChallenge2020.Actions
{
    public class WaitAction : IAction
    {
        public string GetAction()
        {
            return "WAIT";
        }
    }
}
