using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlAstro.Utils
{
    public static class EnumerableHelper
    {
        /// <summary>
        /// 将枚举集合中每个元素转为字符串并以","分隔
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToArrayString<T>(this IEnumerable<T> a)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < a.Count(); i++)
            {
                sb.Append(a.ElementAt(i));
            }
            return string.Join(",", sb);
        }

        /// <summary>
        /// 将枚举集合中每个元素转为字符串并以指定分隔符分隔
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string ToArrayString<T>(this IEnumerable<T> a, string separator)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < a.Count(); i++)
            {
                sb.Append(a.ElementAt(i));
            }
            return string.Join(separator, sb);
        }

        /// <summary>
        /// 比较两个数组是否各元素都相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ArrayEquals(Array a, Array b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a.GetValue(i).Equals(b.GetValue(i)))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 比较两个数组是否各元素都相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool EqualsArray(this Array a, Array b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (!a.GetValue(i).Equals(b.GetValue(i)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
