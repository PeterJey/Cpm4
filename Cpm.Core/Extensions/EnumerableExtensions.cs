using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

namespace Cpm.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> AppendOptional<TSource>(this IEnumerable<TSource> source, Option<TSource> optionalElement)
        {
            // ReSharper disable PossibleMultipleEnumeration
            return optionalElement.Match(source.Append, () => source);
            // ReSharper restore PossibleMultipleEnumeration
        }

        public static IEnumerable<T> ForWindow<T>(this IEnumerable<T> sequence, int startOffset, int length, T padding)
        {
            return Enumerable.Repeat(padding, Math.Max(0, -startOffset))
                .Concat(sequence.Skip(Math.Max(0, startOffset)))
                .LimitOrRightPad(length, padding);
        }

        public static IEnumerable<T> IgnoreNulls<T>(this IEnumerable<T> sequence)
        {
            return sequence.Where(x => x != null);
        }

        public static IEnumerable<T> SkipFromEnd<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        {
            return sequence
                .Reverse()
                .SkipWhile(predicate)
                .Reverse();
        }

        public static IEnumerable<T> SkipOrLeftPad<T>(this IEnumerable<T> sequence, int length, Func<T> padding)
        {
            return Enumerable.Range(0, Math.Max(0, -length))
                .Select(x => padding())
                .Concat(sequence.Skip(Math.Max(0, length)));
        }

        public static IEnumerable<T> SkipNullsFromEnd<T>(this IEnumerable<T> sequence)
        {
            return sequence.SkipFromEnd(x => x == null);
        }

        public static IEnumerable<T> SkipNulls<T>(this IEnumerable<T> sequence)
        {
            return sequence.SkipWhile(x => x == null);
        }

        public static TResult MinOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var materialized = source.ToArray();
            return materialized.Any()
                ? materialized.Min(selector)
                : default(TResult);
        }

        public static TResult MaxOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var materialized = source.ToArray();
            return materialized.Any()
                ? materialized.Max(selector)
                : default(TResult);
        }

        public static double? AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            var materialized = source.ToArray();
            return materialized.Any()
                ? materialized.Average(selector)
                : null;
        }

        public static decimal? NullableSum(this IEnumerable<decimal?> source)
        {
            return source
                .Select(x => x.ToOption())
                .Aggregate(
                    0m.None(), 
                    (accumulator, current) =>  current
                        .Map(curr => accumulator.ValueOr(0) + curr)
                        .Else(accumulator)
                    )
                .ToNullable();
        }

        public static IEnumerable<TSource> LeftPad<TSource>(
            this IEnumerable<TSource> source, 
            int length,
            TSource padding
            )
        {
            return Enumerable.Repeat(padding, length)
                .Concat(source);
        }

        public static IEnumerable<TSource> RightPad<TSource>(
            this IEnumerable<TSource> source, 
            int length,
            TSource padding
            )
        {
            return source
                .Concat(Enumerable.Repeat(padding, length));
        }

        public static IEnumerable<TSource> LimitOrRightPad<TSource>(
            this IEnumerable<TSource> source, 
            int count,
            TSource padding
            )
        {
            return source
                .RightPadIndefinetely(padding)
                .Take(count);
        }

        public static IEnumerable<TSource> RightPadIndefinetely<TSource>(
            this IEnumerable<TSource> source, 
            TSource padding
            )
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }

            while (true)
            {
                yield return padding;
            }

            // ReSharper disable once IteratorNeverReturns
            // duh!
        }

        public static void ForEach<TSource>(
            this IEnumerable<TSource> source, 
            Action<TSource> action
            )
        {
            foreach (var element in source)
            {
                action?.Invoke(element);
            }
        }

        public static void ForEach<TSource>(
            this IEnumerable<TSource> source, 
            Action<TSource, int> action
            )
        {
            foreach (var element in source.Select((x, i) => (x, i)))
            {
                action?.Invoke(element.Item1, element.Item2);
            }
        }

        public static IEnumerable<TResult> ZipWithOffset<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            int offset,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            var firstOffset = Math.Max(0, -offset);
            var secondOffset = Math.Max(0, offset);

            return first
                .Skip(secondOffset)
                .Zip(
                    second.Skip(firstOffset),
                    resultSelector
                );
        }

        public static long GetSpan<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long> offsetSelector)
        {
            var result = source
                .Aggregate(
                    (Tuple<long, long>)null,
                    (acc, curr) =>
                    {
                        var offset = offsetSelector(curr);

                        return new Tuple<long, long>(
                            Math.Min(acc?.Item1 ?? offset, offset),
                            Math.Max(acc?.Item2 ?? offset, offset)
                        );
                    });

            return result
                .SomeNotNull()
                .Map(x => x.Item2 - x.Item1 + 1)
                .ValueOr(0);
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second, Func<TFirst, TSecond, int, TResult> resultSelector)
        {
            return first
                .Zip(second, (f, s) => (f, s))
                .Select((x, i) => resultSelector(x.Item1, x.Item2, i));
        }

        public static IEnumerable<TOuterOutput> Pivot<TOuterInput, TInnerInput, TOuterOutput, TInnerOutput>(
            this IEnumerable<TOuterInput> series, 
            Func<TOuterInput, IEnumerable<TInnerInput>> getInners, 
            Func<TInnerInput, int> getInnerIndex,
            Func<int, IEnumerable<TInnerInput>, IEnumerable<TInnerOutput>, TOuterOutput> createOuter,
            Func<int, TInnerInput, TInnerOutput> createInner,
            uint paddingBefore = 0,
            uint paddingAfter = 0
            )
        {
            var localSeries = series
                .Select(x => getInners(x).ToArray())
                .ToArray();

            var firstIndex = localSeries
                .SelectMany(s => s.Select(getInnerIndex))
                .MinOrDefault(x => x) - (int)paddingBefore;

            var count = localSeries
                .SelectMany(s => s.Select(e => (int?)getInnerIndex(e)))
                .MaxOrDefault(x => x)
                .ToOption()
                .Map(li => li - firstIndex + 1 + (int)paddingBefore + (int)paddingAfter)
                .ValueOr(0);

            return Enumerable.Range(firstIndex, count)
                .Select(index => createOuter(
                    index,
                    localSeries
                        .Select(s => s.SingleOrDefault(x => getInnerIndex(x) == index)),
                    localSeries
                        .Select((s, i) => 
                            createInner(i, s.SingleOrDefault(x => getInnerIndex(x) == index)))
                        )
                );
        }
    }
}
