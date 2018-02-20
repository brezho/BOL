using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace X.Repository.Databases.Helpers
{
    public class WhereClauseVisitor<T> : ExpressionVisitor
    {
        public string FieldOpen = "[";
        public string FieldClose = "]";

        StringBuilder sb = new StringBuilder();
        int parametersCount = 0;
        int visitCounters = 0;
        List<object> parameters = new List<object>();

        Expression Root;
        Expression Previous;
        Expression Current;

        public override Expression Visit(Expression node)
        {
            visitCounters++;
            Previous = Current;
            Current = node;
            return base.Visit(node);
        }

        public Tuple<string, object[]> Convert(Expression<Func<T, bool>> expression)
        {
            Root = expression.Body;
            Visit(expression.Body);
            return Tuple.Create(sb.ToString(), parameters.ToArray());
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            sb.Append("(");
            Visit(node.Left);
            switch (node.NodeType)
            {
                case ExpressionType.LessThan: sb.Append("<"); break;
                case ExpressionType.LessThanOrEqual: sb.Append("<="); break;
                case ExpressionType.GreaterThan: sb.Append(">"); break;
                case ExpressionType.GreaterThanOrEqual: sb.Append(">="); break;
                case ExpressionType.AndAlso: sb.Append(" AND "); break;
                case ExpressionType.OrElse: sb.Append(" OR "); break;
                case ExpressionType.Equal: sb.Append(" = "); break;
                case ExpressionType.NotEqual: sb.Append(" <> "); break;
                case ExpressionType.Not: sb.Append(" <> "); break;
            }
            Visit(node.Right);
            sb.Append(")");

            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:

                    sb.Append(" NOT( ");
                    Visit(node.Operand);
                    sb.Append(" ) ");
                    return node;

                case ExpressionType.Convert:
                    Visit(node.Operand);
                    return node;
            }
            return base.VisitUnary(node);
        }


        protected override Expression VisitMember(MemberExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.MemberAccess:
                    if (node.Expression == null)// null=static member
                    {
                        var memberName = node.Member.Name;
                        var field = node.Member as FieldInfo;
                        if (field != null)
                        {
                            var value = field.GetValue(null);
                            sb.Append("@" + parametersCount);
                            parametersCount++;
                            parameters.Add(value);
                        }
                        else
                        {
                            var prop = node.Member as PropertyInfo;
                            if (prop != null)
                            {
                                var value = prop.GetValue(null, null);
                                sb.Append("@" + parametersCount);
                                parametersCount++;
                                parameters.Add(value);
                            }
                        }
                    }
                    else
                    {
                        object @object = null;
                        if (node.Expression is ConstantExpression)
                        {
                            @object = ((ConstantExpression)node.Expression).Value;
                            var memberName = node.Member.Name;
                            var field = node.Member as FieldInfo;
                            if (field != null)
                            {
                                var value = field.GetValue(@object);
                                sb.Append("@" + parametersCount);
                                parametersCount++;
                                parameters.Add(value);
                            }
                            else
                            {
                                var prop = node.Member as PropertyInfo;
                                if (prop != null)
                                {
                                    var value = prop.GetValue(@object, null);
                                    sb.Append("@" + parametersCount);
                                    parametersCount++;
                                    parameters.Add(value);
                                }
                            }
                        }
                        if (node.Expression is ParameterExpression)
                        {
                            if (node.Type == typeof(Boolean) && !(Previous is BinaryExpression))
                                sb.Append(FieldOpen + node.Member.Name + FieldClose + " = 1");
                            else sb.Append(FieldOpen + node.Member.Name + FieldClose);
                        }
                        return null;
                    }


                    break;
            }
            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object.Type.Match(typeof(IList<>)))
            {
                var meth = node.Method.Name.ToLowerInvariant();

                if (node.Method.Name.Equals("contains", StringComparison.InvariantCultureIgnoreCase))
                {
                    Visit(node.Arguments[0]);
                    sb.Append(" IN (");
                    Visit(node.Object);
                    sb.Append(")");
                    return null;
                }
            }
            if (node.Object.Type.Match(typeof(string)))
            {
                if (node.Method.Name.Equals("startswith", StringComparison.InvariantCultureIgnoreCase))
                {
                    Visit(node.Object);
                    sb.Append(" like ");
                    sb.Append("@" + parametersCount);
                    parametersCount++;
                    parameters.Add(node.Arguments[0].ToString().TrimStart('"').TrimEnd('"') + "%");
                    return null;
                }
                if (node.Method.Name.Equals("endswith", StringComparison.InvariantCultureIgnoreCase))
                {
                    Visit(node.Object);
                    sb.Append(" like ");
                    sb.Append("@" + parametersCount);
                    parametersCount++;
                    parameters.Add("%" + node.Arguments[0].ToString().TrimStart('"').TrimEnd('"'));
                    return null;
                }
                if (node.Method.Name.Equals("contains", StringComparison.InvariantCultureIgnoreCase))
                {
                    Visit(node.Object);
                    sb.Append(" like ");
                    sb.Append("@" + parametersCount);
                    parametersCount++;
                    parameters.Add("%" + node.Arguments[0].ToString().TrimStart('"').TrimEnd('"') + "%");
                    return null;
                }
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            sb.Append("@" + parametersCount);
            parametersCount++;
            parameters.Add(node.Value);
            return base.VisitConstant(node);
        }
    }
}
