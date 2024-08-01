using System;
using System.Linq;
using System.Linq.Expressions;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Functions
{
    public abstract class StaticFunction : IFunction
    {
        public ValueExpressionDesc Execute(FunctionContext context)
        {
            if (context.ExecuteContext.LastExpression != null)
            {
                throw new Exception("static function should not has instance.");
            }
            return ExecuteStaticFunction(context);
        }
        protected abstract ValueExpressionDesc ExecuteStaticFunction(FunctionContext context);

    }
    public class StaticFunction<T> : StaticFunction
    {
        public StaticFunction(Expression<Func<T>> body)
        {

            this.body = body;
        }
        private Expression<Func<T>> body;

        protected override ValueExpressionDesc ExecuteStaticFunction(FunctionContext context)
        {
            var argumentReplacer = new ArgumentReplacer((index, type) =>
            {
                return GetArgumentExpression(index, type, context);
            });
            var expression = argumentReplacer.Visit(body.Body);
            if (argumentReplacer.ArgumentCount != context.Arguments.Length)
            {
                throw new Exception($"The argument count for function '{context.Name}' not match.");
            }
            return ValueExpressionDesc.FromExpression(expression);
        }
        private Expression GetArgumentExpression(int index, Type type, FunctionContext context)
        {
            if (index > context.Arguments.Length)
            {
                throw new Exception($"Missing the {index + 1}th parameter for function '{context.Name}'.");
            }
            var valueInfo = context.Arguments[index];
            var expression = LambdaUtils.ExecuteValueInfoAndConvert(context.ExecuteContext, valueInfo,type);
            return expression.Expression;
        }

        class ArgumentReplacer : ExpressionVisitor
        {
            private readonly Func<int, Type, Expression> argumentExpressionFactory;

            public ArgumentReplacer(Func<int, Type, Expression> argumentExpressionFactory)
            {
                this.argumentExpressionFactory = argumentExpressionFactory;
            }
            public int ArgumentCount { get; private set; }
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(It)
                    && node.Method.Name == nameof(It.Arg))
                {
                    return argumentExpressionFactory(ArgumentCount++, node.Type);
                }
                else
                {
                    return base.VisitMethodCall(node);
                }
            }
        }
    }
}
