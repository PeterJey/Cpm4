using Optional;

namespace Cpm.Core.Extensions
{
    public static class OptionExtensions
    {
        public static T? ToNullable<T>(this Option<T?> optional) where T : struct
        {
            return optional.ValueOr((T?)null);
        }

        public static T? ToNullable<T>(this Option<T> optional) where T : struct
        {
            return optional.Map(x => (T?) x).ValueOr((T?)null);
        }

        public static T ToReferenceOrNull<T>(this Option<T> optional) where T : class
        {
            return optional.ValueOr((T)null);
        }
    }
}
