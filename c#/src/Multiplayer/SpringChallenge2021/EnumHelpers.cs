using System;
using System.Collections.Generic;
using System.Linq;

namespace SpringChallenge2021
{
    public static class EnumHelpers
    {
        public static IReadOnlyCollection<T> GetAllValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
