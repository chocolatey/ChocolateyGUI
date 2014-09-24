// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LinqExtensions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class LinqExtensions
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>> CachedTypes =
            new ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>>();

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if ((new OrderByVistor(query.Expression)).HasOrderBy)
            {
                throw new InvalidOperationException(
                    "You can't call OrderBy on a query that is already ordered. Try using ThenBy.");
            }

            var targetProp = GetProperty<T>(propertyName);
            if (targetProp == null)
            {
                throw new ArgumentException("There is no property with that name");
            }

            var parameters = new[] { Expression.Parameter(query.ElementType, "query") };

            var queryExpr = query.Expression;
            queryExpr = Expression.Call(
                typeof(Queryable),
                "OrderBy",
                new[] { query.ElementType, targetProp.PropertyType },
                queryExpr,
                Expression.Lambda(Expression.Property(parameters[0], targetProp), parameters[0]));

            return query.Provider.CreateQuery<T>(queryExpr);
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if ((new OrderByVistor(query.Expression)).HasOrderBy)
            {
                throw new InvalidOperationException(
                    "You can't call OrderByDescending on a query that is already ordered. Try using ThenByDescending.");
            }

            var targetProp = GetProperty<T>(propertyName);
            if (targetProp == null)
            {
                throw new ArgumentException("There is no property with that name");
            }

            var parameters = new[] { Expression.Parameter(query.ElementType, "query") };

            var queryExpr = query.Expression;
            queryExpr = Expression.Call(
                typeof(Queryable),
                "OrderByDescending",
                new[] { query.ElementType, targetProp.PropertyType },
                queryExpr,
                Expression.Lambda(Expression.Property(parameters[0], targetProp), parameters[0]));
            return query.Provider.CreateQuery<T>(queryExpr);
        }

        public static IQueryable<T> ThenBy<T>(this IQueryable<T> query, string propertyName)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (!(new OrderByVistor(query.Expression)).HasOrderBy)
            {
                throw new InvalidOperationException(
                    "You can't call ThenBy on a query that isnt already ordered. First call OrderBy.");
            }

            var targetProp = GetProperty<T>(propertyName);
            if (targetProp == null)
            {
                throw new ArgumentException("There is no property with that name");
            }

            var parameters = new[] { Expression.Parameter(query.ElementType, "query") };

            var queryExpr = query.Expression;
            queryExpr = Expression.Call(
                typeof(Queryable),
                "ThenBy",
                new[] { query.ElementType, targetProp.PropertyType },
                queryExpr,
                Expression.Lambda(Expression.Property(parameters[0], targetProp), parameters[0]));
            return query.Provider.CreateQuery<T>(queryExpr);
        }

        public static IQueryable<T> ThenByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (!(new OrderByVistor(query.Expression)).HasOrderBy)
            {
                throw new InvalidOperationException(
                    "You can't call ThenByDescending on a query that isnt already ordered. First call OrderByDescending.");
            }

            var targetProp = GetProperty<T>(propertyName);
            if (targetProp == null)
            {
                throw new ArgumentException("There is no property with that name");
            }

            var parameters = new[] { Expression.Parameter(query.ElementType, "query") };

            var queryExpr = query.Expression;
            queryExpr = Expression.Call(
                typeof(Queryable),
                "ThenByDescending",
                new[] { query.ElementType, targetProp.PropertyType },
                queryExpr,
                Expression.Lambda(Expression.Property(parameters[0], targetProp), parameters[0]));
            return query.Provider.CreateQuery<T>(queryExpr);
        }

        // From http://stackoverflow.com/a/13503860 | Thanks sehe!
        internal static IList<TR> FullOuterJoin<TA, TB, TK, TR>(
            this IEnumerable<TA> a,
            IEnumerable<TB> b,
            Func<TA, TK> selectKeyA,
            Func<TB, TK> selectKeyB,
            Func<TA, TB, TK, TR> projection,
            TA defaultA = default(TA),
            TB defaultB = default(TB),
            IEqualityComparer<TK> cmp = null)
        {
            cmp = cmp ?? EqualityComparer<TK>.Default;
            var alookup = a.ToLookup(selectKeyA, cmp);
            var blookup = b.ToLookup(selectKeyB, cmp);

            var keys = new HashSet<TK>(alookup.Select(p => p.Key), cmp);
            keys.UnionWith(blookup.Select(p => p.Key));

            var join = from key in keys
                       from xa in alookup[key].DefaultIfEmpty(defaultA)
                       from xb in blookup[key].DefaultIfEmpty(defaultB)
                       select projection(xa, xb, key);

            return join.ToList();
        }

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
                {
                    return null;
                }

                propDic.TryAdd(propertyName, targetProp);
            }

            return targetProp;
        }

        internal sealed class OrderByVistor : ExpressionVisitor
        {
            public OrderByVistor(Expression queryExpr)
            {
                this.Visit(queryExpr);
            }

            public bool HasOrderBy { get; set; }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }

                if (node.Method.Name == "OrderByDescending" || node.Method.Name == "OrderBy")
                {
                    this.HasOrderBy = true;
                }

                return node.CanReduce ? base.VisitMethodCall(node) : node;
            }
        }
    }
}