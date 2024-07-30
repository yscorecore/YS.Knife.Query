using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Functions
{
    internal class RandomInt : StaticFunction<int>
    {
        public static Random Random = new Random();
        public RandomInt() : base(() => Random.Next(0, It.Arg<int>()))
        {
        }

    }
}
