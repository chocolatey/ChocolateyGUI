using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Chocolatey.Gui.Utilities.Extensions
{
    public static class LinqExtensions
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>> CachedTypes =
            new ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>>();

        private static PropertyInfo GetProperty<T>(string propertyName)
        {
            var type = typeof(T);
            ConcurrentDictionary<string, PropertyInfo> propDic;
            if (!CachedTypes.TryGetValue(type, out propDic))
            {
                propDic = new ConcurrentDictionary<string, PropertyInfo>();
                CachedTypes.TryAdd(type, propDic);
            }

            PropertyInfo targetProp;
            if (!propDic.TryGetValue(propertyName, out targetProp))
            {
                targetProp = type.GetProperty(propertyName);
                if (targetProp == null)
                    return null;
                propDic.TryAdd(propertyName, targetProp);
            }
            return targetProp;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            if ((new OrderByVistor(query.Expression)).HasOrderBy)
                throw new InvalidOperationException("You can't call OrderBy on a query that is already ordered. Try using ThenBy.");

            var targetProp = GetProperty<T>(propertyName);
            if (targetProp == null)
                throw new ArgumentException("There is no property with that name");

            var parameters = new [] {
                Expression.Parameter(query.ElementType, "query") 
            };

            var queryExpr = query.Expression;
            queryExpr = Expression.Call(
                typeof(Queryable),
                "OrderBy", new [] { query.ElementType, targetProp.PropertyType },
                queryExpr,
                Expression.Lambda(Expression.Property(parameters[0], targetProp), parameters[0]));

            return query.Provider.CreateQuery<T>(queryExpr);
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            if ((new OrderByVistor(query.Expression)).HasOrderBy)
                throw new InvalidOperationException("You can't call OrderByDescending on a query that is already ordered. Try using ThenByDescending.");

            var targetProp = GetProperty<T>(propertyName);
            if (targetProp == null)
                throw new ArgumentException("There is no property with that name");

            var parameters = new [] {
                Expression.Parameter(query.ElementType, "query") 
            };

            var queryExpr = query.Expression;
            queryExpr = Expression.Call(typeof(Queryable), "OrderByDescending", new[] { query.ElementType, targetProp.PropertyType }, queryExpr, Expression.Lambda(Expression.Property(parameters[0], targetProp), parameters[0]));
            return query.Provider.CreateQuery<T>(queryExpr);
        }

        public static IQueryable<T> ThenBy<T>(this IQueryable<T> query, string propertyName)
        {
            if (!(new OrderByVistor(query.Expression)).HasOrderBy)
                throw new InvalidOperationException("You can't call ThenBy on a query that isnt already ordered. First call OrderBy.");

            var targetProp = GetProperty<T>(propertyName);
            if (targetProp == null)
                throw new ArgumentException("There is no property with that name");

            var parameters = new [] {
                Expression.Parameter(query.ElementType, "query")
            };

            var queryExpr = query.Expression;
            queryExpr = Expression.Call(typeof(Queryable), "ThenBy", new [] { query.ElementType, targetProp.PropertyType }, queryExpr, Expression.Lambda(Expression.Property(parameters[0], targetProp), parameters[0]));
            return query.Provider.CreateQuery<T>(queryExpr);
        }

        public static IQueryable<T> ThenByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            if (!(new OrderByVistor(query.Expression)).HasOrderBy)
                throw new InvalidOperationException("You can't call ThenByDescending on a query that isnt already ordered. First call OrderByDescending.");

            var targetProp = GetProperty<T>(propertyName);
            if (targetProp == null)
                throw new ArgumentException("There is no property with that name");

            var parameters = new [] {
                Expression.Parameter(query.ElementType, "query")
            };

            var queryExpr = query.Expression;
            queryExpr = Expression.Call(typeof(Queryable), "ThenByDescending", new [] { query.ElementType, targetProp.PropertyType }, queryExpr, Expression.Lambda(Expression.Property(parameters[0], targetProp), parameters[0]));
            return query.Provider.CreateQuery<T>(queryExpr);
        }

        internal sealed class OrderByVistor : ExpressionVisitor
        {
            public bool HasOrderBy { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            public OrderByVistor(Expression queryExpr)
            {
                Visit(queryExpr);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.Name == "OrderByDescending" || node.Method.Name == "OrderBy")
                    HasOrderBy = true;

                return node.CanReduce ? base.VisitMethodCall(node) : node;
            }
        }

    }
}
