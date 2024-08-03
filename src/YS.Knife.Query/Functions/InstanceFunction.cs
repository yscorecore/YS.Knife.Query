using System;
using System.Linq;
using System.Linq.Expressions;
using YS.Knife.Query.ExpressionConverters;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Functions
{
    public abstract class InstanceFunction : IFunction
    {
        public ValueExpressionDesc Execute(FunctionContext context)
        {
            if (context.ExecuteContext.LastExpression == null)
            {
                throw new Exception("instance function should has instance.");
            }
            return ExecuteInstanceFunction(context);
        }
        protected abstract ValueExpressionDesc ExecuteInstanceFunction(FunctionContext context);

    }

    public class InstanceFunction<Source, Target> : InstanceFunction
    {
        public InstanceFunction(Expression<Func<Source, Target>> body)
        {
            this.body = body;
        }
        private Expression<Func<Source, Target>> body;
        protected override ValueExpressionDesc ExecuteInstanceFunction(FunctionContext context)
        {
            var argumentReplacer = new ArgumentReplacer((index, type) =>
            {
                return GetArgumentExpression(index, type, context);
            });
            var instanceExpressionDesc = context.ExecuteContext.LastExpression;
            if (instanceExpressionDesc == null)
            {
                throw new Exception("can not find a instance.");
            }

            var instance = GetSourceInstance(instanceExpressionDesc);
            var firstParam = body.Parameters.Single();
            var parameterReplacer = new ParameterReplacer(firstParam, instance);

            //replace instance
            var expression = parameterReplacer.Visit(body.Body);
            expression = argumentReplacer.Visit(expression);
            if (argumentReplacer.ArgumentCount != context.Arguments.Length)
            {
                throw new Exception($"The argument count for function '{context.Name}' not match.");
            }
            return ValueExpressionDesc.FromExpression(expression);
        }
        private Expression GetSourceInstance(ValueExpressionDesc expressionDesc)
        {
            if (expressionDesc.ValueType != typeof(Source))
            {
                if (ExpressionConverterFactory.CanConverter(expressionDesc.ValueType, typeof(Source), out var converter))
                {
                    return converter.Convert(expressionDesc.Expression, typeof(Source));
                }
                else
                {
                    throw new Exception("can not convert to instance type");
                }
            }
            else
            {
                return expressionDesc.Expression;
            }
        }
        private Expression GetArgumentExpression(int index, Type type, FunctionContext context)
        {
            if (index > context.Arguments.Length)
            {
                throw new Exception($"Missing the {index + 1}th parameter for function '{context.Name}'.");
            }
            var valueInfo = context.Arguments[index];
            var expression = LambdaUtils.ExecuteValueInfoAndConvert(context.ExecuteContext, valueInfo, type);
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

        class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression m_parameter;
            private readonly Expression m_replacement;

            public ParameterReplacer(ParameterExpression parameter, Expression replacement)
            {
                this.m_parameter = parameter;
                this.m_replacement = replacement;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (ReferenceEquals(node, m_parameter))
                    return m_replacement;
                return node;
            }
        }
    }
}
