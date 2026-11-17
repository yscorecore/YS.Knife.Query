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
                    if (unaryExpression.Operand is MethodCallExpression operandCall &&
                        TryCreateContainsFilter(p, operandCall, out var notContainsFilter))
                    {
                        return notContainsFilter.Not();
                    }
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
                if (TryCreateContainsFilter(p, callExpression, out var containsFilter))
                {
                    return containsFilter;
                }
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

        // 函数参数转换：LambdaExpression 反向为 FilterInfo（存为常量），其他按普通值路径处理
        private static ValueInfo ConvertFunctionArgument(ParameterExpression outerParam, Expression expression)
        {
            if (expression is LambdaExpression lambda)
            {
                // Any/Count 的 Lambda 参数：把 body 作为内部 Filter，以 Lambda 参数为基准
                var innerParam = lambda.Parameters.Single();
                var innerFilter = CreateFilter(innerParam, lambda.Body);
                return ValueInfo.FromConstantValue(innerFilter);
            }
            return CreateValue(outerParam, expression);
        }

        // 把集合的 Contains 调用转换为 in 操作符，例如：
        // arr.Contains(p.Name)      => Name in ["a","b"]
        // p.Tags.Contains("a")      => "a" in Tags
        private static bool TryCreateContainsFilter(ParameterExpression p, MethodCallExpression callExpression, out FilterInfo filter)
        {
            filter = null;
            if (callExpression.Method.Name != nameof(Enumerable.Contains))
            {
                return false;
            }
            Expression source;
            Expression target;
            if (callExpression.Method.IsStatic)
            {
                // .NET 8+ 编译器对 string/primitive 数组的 .Contains() 优化为 MemoryExtensions.Contains
                // 我们同时接受 Enumerable.Contains 和 MemoryExtensions.Contains（以及任何正确签名的静态扩展方法）
                var dt = callExpression.Method.DeclaringType;
                if ((dt != typeof(Enumerable) && dt != typeof(System.MemoryExtensions))
                    || callExpression.Arguments.Count != 2)
                {
                    return false;
                }
                source = callExpression.Arguments[0];
                target = callExpression.Arguments[1];
            }
            else
            {
                // string.Contains 属于字符串实例函数，继续交给 ValuePath 逻辑处理
                if (callExpression.Object == null || callExpression.Arguments.Count != 1 ||
                    callExpression.Method.DeclaringType == typeof(string))
                {
                    return false;
                }
                source = callExpression.Object;
                target = callExpression.Arguments[0];
            }

            // .NET 8+ MemoryExtensions.Contains 会把 source 包装成 op_Implicit(ReadOnlySpan)
            // 解包这个隐式转换，拿到真正的集合表达式
            source = UnwrapImplicitConversion(source);

            filter = new FilterInfo
            {
                OpType = CombinSymbol.SingleItem,
                Left = CreateValue(p, target),
                Right = CreateValue(p, source),
                Operator = Operator.In
            };
            return true;
        }

        // 如果 expr 是方法名 op_Implicit 的 MethodCallExpression（如 ReadOnlySpan 的隐式转换），
        // 取其第一个参数返回；否则原样返回
        private static Expression UnwrapImplicitConversion(Expression expr)
        {
            if (expr is MethodCallExpression mc && mc.Method.Name == "op_Implicit" && mc.Arguments.Count >= 1)
            {
                return mc.Arguments[0];
            }
            return expr;
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
                else if (current is NewArrayExpression newArrayExpression)
                {
                    // 数组字面量，如 new[] { "a", "b" }
                    return ValueInfo.FromConstantValue(Evaluate(newArrayExpression));
                }
                else if (current is MethodCallExpression callExpression)
                {
                    var method = callExpression.Method;
                    if (method.IsStatic)
                    {
                        var firstArg = method.GetParameters().FirstOrDefault();

                        if (firstArg != null && InstanceFunctions.Contains(firstArg.ParameterType, method.Name))
                        {
                            // 扩展方法（或任何把实例放在第一个参数的静态调用）
                            var functionArgs = callExpression.Arguments.Skip(1)
                                .Select(t => ConvertFunctionArgument(p, t)).ToArray();
                            paths.Insert(0, new ValuePath { IsFunction = true, FunctionArgs = functionArgs, Name = method.Name });
                            // 实例在第一个参数（不是 callExpression.Object，静态方法 Object 永远是 null）
                            current = callExpression.Arguments[0];
                            continue;
                        }
                    }
                    else
                    {
                        if (InstanceFunctions.Contains(method.DeclaringType, method.Name))
                        {
                            var functionArgs = callExpression.Arguments.Select(t => ConvertFunctionArgument(p, t)).ToArray();
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
                return ValueInfo.FromConstantValue(Evaluate(expression));
            }
        }
        private static object Evaluate(Expression expression)
        {
            try
            {
                return Expression.Lambda(expression).Compile().DynamicInvoke();
            }
            catch (Exception ex)
            {
                throw new NotSupportedException($"can not evaluate expression '{expression}'.", ex);
            }
        }



    }
}
