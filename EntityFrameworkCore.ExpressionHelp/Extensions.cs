using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFrameworkCore.ExpressionHelp
{
    /// <summary>
    /// HelperException
    /// </summary>
    public class ExpressionHelpException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ExpressionHelpException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// 合并表达式
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TBaseTarget"></typeparam>
        /// <param name="sourceExp">基础表达式</param>
        /// <param name="targetExp">目标表达式</param>
        /// <returns></returns>
        public static Expression<Func<TSource, TTarget>> Merge<TSource, TTarget, TBaseTarget>(
            this Expression<Func<TSource, TBaseTarget>> sourceExp,
            Expression<Func<TSource, TTarget>> targetExp
            )
            where TSource : new() where TBaseTarget : new() where TTarget : TBaseTarget
        {
            if (sourceExp.Body is MemberInitExpression sourceBody && targetExp.Body is MemberInitExpression targetBody)
            {
                return Merge<TSource, TTarget, TBaseTarget>(targetExp.Parameters[0], sourceBody, targetBody);
            }
            else
            {
                throw new ExpressionHelpException("不能合并复杂表达式!");
            }
        }

        private static Expression<Func<TSource, TTarget>> Merge<TSource, TTarget, TBaseTarget>(ParameterExpression param, MemberInitExpression sourceBody, MemberInitExpression targetBody)
            where TSource : new()
            where TTarget : TBaseTarget
            where TBaseTarget : new()
        {
            List<MemberAssignment> binds = PushSourceMenbers(param, sourceBody, GetTargetBindList(targetBody));
            var body = Expression.MemberInit(targetBody.NewExpression, binds);
            return Expression.Lambda<Func<TSource, TTarget>>(body, param);
        }

        private static List<MemberAssignment> GetTargetBindList(MemberInitExpression targetBody)
        {
            List<MemberAssignment> binds = new List<MemberAssignment>();
            foreach (var item in targetBody.Bindings)
            {
                if (item is MemberAssignment memberAssign)
                    binds.Add(memberAssign);
            }
            return binds;
        }

        private static List<MemberAssignment> PushSourceMenbers(ParameterExpression param, MemberInitExpression sourceBody, List<MemberAssignment> binds)
        {
            foreach (var item in sourceBody.Bindings)
            {
                if (item is MemberAssignment memAssign && memAssign.Expression is MemberExpression exp)
                    PushMember(param, binds, memAssign.Member, exp.Member.Name);
            };
            return binds;
        }

        private static void PushMember(ParameterExpression param, List<MemberAssignment> binds, MemberInfo memberInfo, string propertyName)
        {
            var sourceProperty = Expression.Property(param, propertyName);
            var memberAssign = Expression.Bind(memberInfo, sourceProperty);
            binds.Add(memberAssign);
        }

    }
}
