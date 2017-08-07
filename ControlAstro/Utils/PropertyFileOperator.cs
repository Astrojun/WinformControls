using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ControlAstro.Utils
{
    //Properties属性文件操作类
    /// <summary>
    /// 属性文件读取操作类
    /// </summary>
    public class PropertyFileOperator
    {
        private string filePath;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        public PropertyFileOperator(string strFilePath)
        {
            filePath = strFilePath;
        }

        /// <summary>
        /// 根据键获得值字符串
        /// </summary>
        /// <param name="strKey">键</param>
        /// <returns>值</returns>
        public string GetPropertiesText(string strKey)
        {
            string strResult = string.Empty;
            string str = string.Empty;
            using (StreamReader sr = new StreamReader(filePath))
            {
                sr.BaseStream.Seek(0, SeekOrigin.End);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                while ((str = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(str) && str[0] != '#' && str.Substring(0, str.IndexOf('=')).Trim().Equals(strKey))
                    {
                        strResult = str.Substring(str.IndexOf('=') + 1).Trim();
                        break;
                    }
                }
            }
            return strResult;
        }

        /// <summary>
        /// 根据键获得值数组
        /// </summary>
        /// <param name="strKey">键</param>
        /// <returns>值数组</returns>
        public string[] GetPropertiesArray(string strKey)
        {
            string strResult = string.Empty;
            string str = string.Empty;
            using (StreamReader sr = new StreamReader(filePath))
            {
                sr.BaseStream.Seek(0, SeekOrigin.End);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                while ((str = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(str) && str[0] != '#' && str.Substring(0, str.IndexOf('=')).Trim().Equals(strKey))
                    {
                        strResult = str.Substring(str.IndexOf('=') + 1).Trim();
                        break;
                    }
                }
            }
            return strResult.Split(',');
        }

        public void SetPropertiesText(string strKey, string value)
        {
            List<string> list = new List<string>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string str = string.Empty;
                while ((str = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(str) && str[0] != '#' && str.Substring(0, str.IndexOf('=')).Trim().Equals(strKey))
                    {
                        int index = str.IndexOf('=') + 1;
                        str = str.Remove(index);
                        str += value;
                    }
                    list.Add(str);
                }
            }
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    sw.WriteLine(list[i]);
                }
                sw.Flush();
            }
        }

    }

    /// <summary>
    /// 类名：FileProperties 
    /// 描述：操作*.Properties文件
    /// 作者：Michael
    /// 创建时间：2008-03-11
    /// </summary> 
    public class FileProperties : Hashtable
    {
        private ArrayList keys = new ArrayList();
        private string fileName = string.Empty; //要读写的Properties文件名

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">文件名</param>
        public FileProperties(string fileName)
        {
            this.fileName = fileName;
            load(fileName);
        }

        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="key">key</param>
        ///<param name="value">value</param>
        /// <returns></returns>
        public override void Add(object key, object value)
        {
            base.Add(key, value);
            keys.Add(key);
        }

        /// <summary>
        /// 删除指定键的元素
        /// </summary>
        /// <param name="key"></param>
        public override void Remove(object key)
        {
            base.Remove(key);
            keys.Remove(key);
        }

        public override ICollection Keys
        {
            get
            {
                return keys;
            }
        }

        /// <summary>
        /// 导入文件
        /// </summary>
        /// <param name="filePath">要导入的文件</param>
        /// <returns></returns>
        public void load(string filePath)
        {
            char[] convertBuf = new char[1024];
            int limit;
            int keyLen;
            int valueStart;
            char c;
            string bufLine = string.Empty;
            bool hasSep;
            bool precedingBackslash;

            using (StreamReader sr = new StreamReader(filePath))
            {
                while (sr.Peek() >= 0)
                {
                    bufLine = sr.ReadLine();
                    limit = bufLine.Length;
                    keyLen = 0;
                    valueStart = limit;
                    hasSep = false;

                    precedingBackslash = false;
                    if (bufLine.StartsWith("#"))
                        keyLen = bufLine.Length;

                    while (keyLen < limit)
                    {
                        c = bufLine[keyLen];
                        if ((c == '=' || c == ':') & !precedingBackslash)
                        {
                            valueStart = keyLen + 1;
                            hasSep = true;
                            break;
                        }
                        else if ((c == ' ' || c == '\t' || c == '\f') & !precedingBackslash)
                        {
                            valueStart = keyLen + 1;
                            break;
                        }
                        if (c == '\\')
                        {
                            precedingBackslash = !precedingBackslash;
                        }
                        else
                        {
                            precedingBackslash = false;
                        }
                        keyLen++;
                    }

                    while (valueStart < limit)
                    {
                        c = bufLine[valueStart];
                        if (c != ' ' && c != '\t' && c != '\f')
                        {
                            if (!hasSep && (c == '=' || c == ':'))
                            {
                                hasSep = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        valueStart++;
                    }

                    string key = bufLine.Substring(0, keyLen);

                    string values = bufLine.Substring(valueStart, limit - valueStart);

                    if (key == "")
                        key += "#";
                    while (key.StartsWith("#") & this.Contains(key))
                    {
                        key += "#";
                    }

                    this.Add(key, values);
                }
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">要保存的Properties文件</param>
        /// <returns></returns>
        public void save(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            FileStream fileStream = File.Create(filePath);
            StreamWriter sw = new StreamWriter(fileStream);
            foreach (object item in keys)
            {
                string key = (string)item;
                string val = (string)this[key];
                if (key.StartsWith("#"))
                {
                    if (val == "")
                    {
                        sw.WriteLine(key);
                    }
                    else
                    {
                        sw.WriteLine(val);
                    }
                }
                else
                {
                    sw.WriteLine(key + "=" + val);
                }
            }
            sw.Close();
            fileStream.Close();
        }
    }

}
