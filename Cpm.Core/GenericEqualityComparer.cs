using System;
using System.Collections.Generic;

namespace Cpm.Core
{
    public class GenericEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        private Func<T, object> Expr { get; }

        public GenericEqualityComparer(Func<T, object> expr)
        {
            Expr = expr;
        }

        public bool Equals(T x, T y)
        {
            var first = Expr.Invoke(x);
            var sec = Expr.Invoke(y);

            return first != null && first.Equals(sec);
        }

        public int GetHashCode(T obj)
        {
            return obj != null 
                ? Expr.Invoke(obj).GetHashCode()
                : 0;
        }
    }
}