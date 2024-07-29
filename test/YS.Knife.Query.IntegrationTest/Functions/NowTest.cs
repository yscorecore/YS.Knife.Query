using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static YS.Knife.Query.IntegrationTest.Functions.StaticFunctionTestUtils;

namespace YS.Knife.Query.IntegrationTest.Functions
{
    public class NowTest
    {
        string functionName = "Now";
        [Fact]
        public void ConstantAndFunction()
        {
            CompareConstantAndFunction(Operator.LessThan, typeof(DateTime), DateTime.Now.AddSeconds(-1), functionName, Array.Empty<ValueInfo>(), true);
            CompareConstantAndFunction(Operator.GreaterThan, typeof(DateTime), DateTime.Now.AddSeconds(1), functionName, Array.Empty<ValueInfo>(), true);
            CompareConstantAndFunction(Operator.LessThan, typeof(DateTime), DateTime.Now.AddSeconds(1), functionName, Array.Empty<ValueInfo>(), false);
            CompareConstantAndFunction(Operator.GreaterThan, typeof(DateTime), DateTime.Now.AddSeconds(-1), functionName, Array.Empty<ValueInfo>(), false);
        }
        [Fact]
        public void FunctionAndConstant()
        {
            CompareFunctionAndConstant(Operator.LessThan, typeof(DateTime), DateTime.Now.AddSeconds(1), functionName, Array.Empty<ValueInfo>(), true);
            CompareFunctionAndConstant(Operator.GreaterThan, typeof(DateTime), DateTime.Now.AddSeconds(-1), functionName, Array.Empty<ValueInfo>(), true);
            CompareFunctionAndConstant(Operator.LessThan, typeof(DateTime), DateTime.Now.AddSeconds(-1), functionName, Array.Empty<ValueInfo>(), false);
            CompareFunctionAndConstant(Operator.GreaterThan, typeof(DateTime), DateTime.Now.AddSeconds(1), functionName, Array.Empty<ValueInfo>(), false);
        }
        [Fact]
        public void FunctionAndFunction()
        {
            //var abc = new int[] { 1 };
            //var bcd = abc.AsQueryable()
            //    .Where(p => DateTime.Now == DateTime.Now)
            //    .ToList();
            //DateTime.Now compare
            CompareFunctionAndFunction(Operator.Equals, functionName, Array.Empty<ValueInfo>(), Array.Empty<ValueInfo>(), true);
        }
    }
}
