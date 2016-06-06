using System.Collections.ObjectModel;
using System.Linq;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {

        /// <summary>
        /// Iterates through all sequence and performs specified action on each
        /// element
        /// </summary>
        /// <typeparam name="T">Sequence element type</typeparam>
        /// <param name="enumerable">Target enumeration</param>
        /// <param name="action">Action</param>
        /// <exception cref="System.ArgumentNullException">One of the input agruments is null</exception>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T element in enumerable)
            {
                action(element);
            }
        }

        public static void ForLoop<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            var enumerableCount = enumerable.Count();
            for (var i = 0; i < enumerableCount; i++ )
            {
                action(enumerable.ElementAt(i));
            }
        }

        public static Boolean IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || enumerable.Count() == 0;
        }

        public static Boolean IsEmpty<T>(this T[] enumerable)
        {
            return enumerable == null || enumerable.Length == 0;
        }

        /// <summary>
        /// Return true if enumerable is Null or is empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || enumerable.Count() == 0;
        }

        public static Boolean IsNotNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.IsNullOrEmpty();
        }
        
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }
            return new ObservableCollection<T>(enumerable);
        }

        /// <summary>
        /// Executes an async ToArray and return EndInvoke method to call to get result/exception.
        /// Async does not work on arrays/lists/collections, only on true enumerables/queryables.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static Func<T[]> Async<T>(this IEnumerable<T> enumerable)
        {
            return Async(enumerable, e => e.ToArray());
        }

        /// <summary>
        /// Executes an async ToArray and return EndInvoke method to call to get result/exception.
        /// Async does not work on arrays/lists/collections, only on true enumerables/queryables.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="asyncSelector"></param>
        /// <returns></returns>
        public static Func<TResult> Async<T, TResult>(this IEnumerable<T> enumerable, Func<IEnumerable<T>, TResult> asyncSelector)
        {
            // Create delegate to exec async
            // ReSharper disable ConvertClosureToMethodGroup
            Func<IEnumerable<T>, TResult> work = (e => asyncSelector(e));
            // ReSharper restore ConvertClosureToMethodGroup

            // Launch it
            var r = work.BeginInvoke(enumerable, null, null);

            // Return method that will block until completed and rethrow exceptions if any
            return () => work.EndInvoke(r);
        }

        public static IEnumerable<T> Delete<T>(this IEnumerable<T> target, IEnumerable<T> elements)
        {
            if (target != null)
            {
                foreach (T item in target)
                {
                    var cont=false;

                    foreach (var element in elements)
                    {
                        if (item.IsNull() && element.IsNull())
                        {
                            cont=true;
                        }

                        if (item.IsNull() || item.Equals(element))
                        {
                            cont=true;
                        }    
                    }

                    if (cont) continue;
                    
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> Delete<T>(this IEnumerable<T> target, T element)
        {
            if (target != null)
            {
                foreach (T item in target)
                {
                    if (item.IsNull() && element.IsNull())
                    {
                        continue;
                    }

                    if (item.IsNull() || item.Equals(element))
                    {
                        continue;
                    }

                    yield return item;
                }
            }
        }
    }
}
