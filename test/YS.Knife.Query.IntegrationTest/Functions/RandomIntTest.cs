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
            CompareConstantAndFunction(Operator.LessThan,
                typeof(int), 5, functionName,
                new ValueInfo[] { new ValueInfo { IsConstant = true, ConstantValue = 10 } }, true);
        }
    }
}
