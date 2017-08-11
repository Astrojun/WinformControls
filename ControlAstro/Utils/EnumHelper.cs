using System;
using System.ComponentModel;

namespace ControlAstro.Utils
{
    public class EnumHelper
    {
        /// <summary>
        /// 获取某个Enum值对应的DescriptionAttribute字符串，不存在则返回string.Empty
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum en)
        {
            var fieldInfo = en.GetType().GetField(en.ToString());
            object[] objs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs.Length < 1)
            {
                return string.Empty;
            }
            foreach (var obj in objs)
            {
                if (obj is DescriptionAttribute)
                {
                    DescriptionAttribute attr = obj as DescriptionAttribute;
                    return attr.Description;
                }
            }
            return string.Empty;
        }
    }
}
