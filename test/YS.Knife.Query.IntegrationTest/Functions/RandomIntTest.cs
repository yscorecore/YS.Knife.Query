using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static YS.Knife.Query.IntegrationTest.Functions.StaticFunctionTestUtils;

namespace YS.Knife.Query.IntegrationTest.Functions
{
    public class RandomIntTest
    {
        string functionName = "RandomInt";
        [Fact]
        public void ConstantAndFunction()
        {
            CompareConstantAndFunction(Operator.GreaterThan,
                typeof(int), 10, functionName,
                new ValueInfo[] { ValueInfo.FromConstantValue(10) }, true);
        }
    }
}
