﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.Filter.Operators
{
    internal abstract class BaseExpressionOperator : IExpressionOperator
    {
        public abstract Operator Operator { get; }

        public abstract Expression CreatePredicateExpression(ExpressionOperatorContext context);
       
    }
}
