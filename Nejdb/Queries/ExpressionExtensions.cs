using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nejdb.Queries
{
    public static class ExpressionExtensions
    {
        public static string ToMemberPath<TDocument, TProperty>(this Expression<Func<TDocument, TProperty>> property)
        {
            var expression = property.Body;

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                switch (unaryExpression.NodeType)
                {
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        expression = unaryExpression.Operand;
                        break;
                }

            }
            var me = expression as MemberExpression;

            if (me == null)
                throw new InvalidOperationException("No idea how to convert " + property.Body.NodeType + ", " + property.Body + " to a member expression");

            var parts = new List<string>();
            while (me != null)
            {
                parts.Insert(0, me.Member.Name);
                me = me.Expression as MemberExpression;
            }
            return String.Join(".", parts.ToArray());
        }
         
    }
}