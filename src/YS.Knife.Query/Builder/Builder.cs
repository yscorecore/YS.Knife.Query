using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Functions;

namespace YS.Knife.Query
{
    public class Builder
    {
        public static string CreateFilter<T>(Expression<Func<T, bool>> expression)
        {
            _ = expression ?? throw new ArgumentNullException(nameof(expression));
            return CreateFilter(expression.Parameters.Single(), expression.Body).ToString();
        }

        private static FilterInfo CreateFilter(ParameterExpression p, Expression body)
        {
            if (body is BinaryExpression binary)
            {
                if (binary.NodeType == ExpressionType.AndAlso)
                {
                    return FilterInfo.CreateAnd(CreateFilter(p, binary.Left), CreateFilter(p, binary.Right));

                }
                else if (binary.NodeType == ExpressionType.OrElse)
                {
                    return FilterInfo.CreateOr(CreateFilter(p, binary.Left), CreateFilter(p, binary.Right));
                }
                else
                {
                    return new FilterInfo
                    {
                        Left = CreateValue(p, binary.Left),
                        Right = CreateValue(p, binary.Right),
                        Operator = GetOperatorTypeByNodeType(binary.NodeType)
                    };
                }
            }
            else if (body is UnaryExpression unaryExpression)
            {
                if (body.NodeType == ExpressionType.Not)
                {
                    return new FilterInfo
                    {
                        Left = CreateValue(p, unaryExpression.Operand),
                        Right = ValueInfo.FromConstantValue(true),
                        Operator = Operator.NotEquals
                    };
                }
                else
                {
                    throw new NotSupportedException($"not support node type '{body.NodeType}'.");
                }

            }
            else if (body is MethodCallExpression callExpression)
            {
                return new FilterInfo
                {
                    Left = CreateValue(p, callExpression),
                    Right = ValueInfo.FromConstantValue(true),
                    Operator = Operator.Equals
                };
            }
            else
            {
                throw new NotSupportedException($"not support node type '{body.NodeType}'.");
            }
        }

        private static Operator GetOperatorTypeByNodeType(ExpressionType expType)
        {
            return expType switch
            {
                ExpressionType.Equal => Operator.Equals,
                ExpressionType.NotEqual => Operator.NotEquals,
                ExpressionType.GreaterThan => Operator.GreaterThan,
                ExpressionType.LessThan => Operator.LessThan,
                ExpressionType.LessThanOrEqual => Operator.LessThanOrEqual,
                ExpressionType.GreaterThanOrEqual => Operator.GreaterThanOrEqual,
                _ => throw new NotSupportedException(),
            };
        }

        private static ValueInfo CreateValue(ParameterExpression p, Expression expression)
        {
            if (expression is ConstantExpression consant)
            {
                return ValueInfo.FromConstantValue(consant.Value);
            }
            return CreateValueFromPaths(p, expression);
        }
        private static ValueInfo CreateValueFromPaths(ParameterExpression p, Expression expression)
        {
            List<ValuePath> paths = new List<ValuePath>();
            var current = expression;
            var isPaths = false;



            while (current != null)
            {
                if (current is ParameterExpression p1)
                {
                    if (p1 == p)
                    {
                        isPaths = true;
                        break;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                else if (current is UnaryExpression unaryExpression)
                {
                    if (unaryExpression.NodeType == ExpressionType.Convert)
                    {
                        current = unaryExpression.Operand;
                        continue;
                    }
                    throw new NotSupportedException($"not support node type '{unaryExpression.NodeType}'.");
                }

                else if (current is MemberExpression memberExpression)
                {
                    paths.Insert(0, new ValuePath { Name = memberExpression.Member.Name });
                    current = memberExpression.Expression;
                }
                else if (current is ConstantExpression constantExpression)
                {
                    paths.Insert(0, new ValuePath { IsConstant = true, ConstantValue = constantExpression.Value });
                    break;
                }
                else if (current is MethodCallExpression callExpression)
                {
                    var method = callExpression.Method;
                    if (callExpression.Method.IsStatic)
                    {
                        var firstArg = callExpression.Method.GetParameters().FirstOrDefault();

                        if (firstArg != null && InstanceFunctions.Contains(firstArg.ParameterType, method.Name))
                        {
                            var functionArgs = callExpression.Arguments.Skip(1).Select(t => CreateValue(p, t)).ToArray();
                            paths.Insert(0, new ValuePath { IsFunction = true, FunctionArgs = functionArgs, Name = method.Name });
                            current = callExpression.Object;
                            continue;
                        }
                    }
                    else
                    {
                        if (InstanceFunctions.Contains(method.DeclaringType, method.Name))
                        {
                            var functionArgs = callExpression.Arguments.Select(t => CreateValue(p, t)).ToArray();
                            paths.Insert(0, new ValuePath { IsFunction = true, FunctionArgs = functionArgs, Name = method.Name });
                            current = callExpression.Object;
                            continue;
                        }
                    }
                    throw new NotSupportedException($"not support method '{method.Name}' yet.");
                }
                else
                {
                    throw new NotSupportedException("can not create value path from expression.");
                }
            }
            if (isPaths)
            {
                return ValueInfo.FromPaths(paths);
            }
            else
            {
                var val = Expression.Lambda(expression).Compile().DynamicInvoke();
                return ValueInfo.FromConstantValue(val);
            }
        }



    }
}
