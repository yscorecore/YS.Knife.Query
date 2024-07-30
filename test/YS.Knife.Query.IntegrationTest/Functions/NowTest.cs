using System;
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
        public void PathAndFunction()
        {
            ComparePathAndFunction(Operator.LessThan, typeof(DateTime), DateTime.Now.AddSeconds(-1), functionName, Array.Empty<ValueInfo>(), true);
            ComparePathAndFunction(Operator.GreaterThan, typeof(DateTime), DateTime.Now.AddSeconds(1), functionName, Array.Empty<ValueInfo>(), true);
            ComparePathAndFunction(Operator.LessThan, typeof(DateTime), DateTime.Now.AddSeconds(1), functionName, Array.Empty<ValueInfo>(), false);
            ComparePathAndFunction(Operator.GreaterThan, typeof(DateTime), DateTime.Now.AddSeconds(-1), functionName, Array.Empty<ValueInfo>(), false);
        }
        [Fact]
        public void FunctionAndPath()
        {
            CompareFunctionAndPath(Operator.LessThan, typeof(DateTime), DateTime.Now.AddSeconds(1), functionName, Array.Empty<ValueInfo>(), true);
            //CompareFunctionAndPath(Operator.GreaterThan, typeof(DateTime), DateTime.Now.AddSeconds(-1), functionName, Array.Empty<ValueInfo>(), true);
            //CompareFunctionAndPath(Operator.LessThan, typeof(DateTime), DateTime.Now.AddSeconds(-1), functionName, Array.Empty<ValueInfo>(), false);
            //CompareFunctionAndPath(Operator.GreaterThan, typeof(DateTime), DateTime.Now.AddSeconds(1), functionName, Array.Empty<ValueInfo>(), false);
        }


        //[Fact]
        //public void FunctionAndFunction()
        //{
        //var abc = new int[] { 1 };
        //var bcd = abc.AsQueryable()
        //    .Where(p => DateTime.Now == DateTime.Now)
        //    .ToList();
        //DateTime.Now compare DateTime.Now returns not true
        //CompareFunctionAndFunction(Operator.Equals, functionName, Array.Empty<ValueInfo>(), Array.Empty<ValueInfo>(), true);
        //}
    }
}
