using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YS.Knife.Query.Functions;
using static YS.Knife.Query.IntegrationTest.Functions.StaticFunctionTestUtils;

namespace YS.Knife.Query.IntegrationTest.Functions
{
    public class RandomIntTest
    {
        static RandomIntTest()
        {
            StaticFunctions.Add(functionName, () => Random.Next(0, It.Arg<int>()));
        }
        public static Random Random = new Random();
        const string functionName = "RandomInt";
        [Fact]
        public void ConstantAndFunction()
        {
            CompareConstantAndFunction(Operator.GreaterThan,
                typeof(int), 10, functionName,
                new ValueInfo[] { ValueInfo.FromConstantValue(10) }, true);
        }
    }
}
