using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Collections.Specialized;
using System.Xml;

namespace LunarCalendar
{
    /// <summary>
    /// string 扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 移除实例中的字符。
        /// </summary>
        /// <param name="source"></param>
        public static void Clear(this StringBuilder source)
        {
            if (source == null) return;

            source.Remove(0, source.Length);
        }

        /// <summary>
        /// 移除特殊字符
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveErrorChar(this string source)
        {
            if (source.IsNullOrWhiteSpace()) return string.Empty;

            // \u007f报错字符
            Regex reg = new Regex(@"[^\[\u007f]+");

            Match m = reg.Match(@source);

            return m.Value;
        }

        /// <summary>
        /// 获取非空字符串，为null时返回string.Entity,否则返回原始值。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetNotNull(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            else
                return source;
        }

        /// <summary>
        /// 获取非空字符串，为null或者空字符串时返回后备的不为空且不为空字符串的参数值,如都不符合条件则返回空字符串。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="values"></param>
        /// <returns>返回的结果进行了Trim()处理</returns>
        public static string GetNotNullOrWhiteSpace(this string source, params string[] values)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                if (values != null)
                {
                    foreach (var item in values)
                    {
                        if (!string.IsNullOrWhiteSpace(item)) return item.Trim();
                    }
                }
                return string.Empty;
            }
            else
                return source.Trim();
        }

        /// <summary>
        /// 返回指定的 System.String 对象是 null 还是 System.String.Empty 字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        const char _charNil = (char)0x00;
        /// <summary>
        /// 指示指定的字符串是 null、空还是仅由空白字符组成。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return true;

            return string.IsNullOrWhiteSpace(source.Trim(new char[] { _charNil }));
        }

        /// <summary>
        /// 去掉字符串的所有空白字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimAll(this string str)
        {
            if (str.IsNullOrEmpty()) return str;

            bool isok = true;
            foreach (char c in str.ToCharArray())
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.SpaceSeparator
                    || c == _charNil)
                {
                    isok = false;
                    break;
                }
            }

            if (isok)
                return str;

            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.SpaceSeparator) continue;

                sb.Append(c);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 去掉字符串的前导及尾部空白字符(如果为null则返回string.Empty)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimEx(this string str)
        {
            if (str.IsNullOrEmpty()) return string.Empty;

            return str.Trim().Trim(new char[] { _charNil });
        }
        /// <summary>
        /// 括号格式化字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bracketType">括号类型</param>
        /// <returns></returns>
        public static string ToString(this string text, BracketType bracketType)
        {
            string prefix = string.Empty;
            string postfix = string.Empty;
            switch (bracketType)
            {
                case BracketType.BigQuotation:
                    prefix = "{";
                    postfix = "}";
                    break;
                case BracketType.DoubleQuotation:
                    prefix = "\"";
                    postfix = "\"";
                    break;
                case BracketType.DoubleQuotationChs:
                    prefix = "“";
                    postfix = "”";
                    break;
                case BracketType.FrenchQuotes:
                    prefix = "《";
                    postfix = "》";
                    break;
                case BracketType.Parentheses:
                    prefix = "(";
                    postfix = ")";
                    break;
                case BracketType.ParenthesesChs:
                    prefix = "（";
                    postfix = "）";
                    break;
                case BracketType.SingleQuotation:
                    prefix = "'";
                    postfix = "'";
                    break;
                case BracketType.SingleQuotationChs:
                    prefix = "‘";
                    postfix = "’";
                    break;
                case BracketType.SquareBrackets:
                    prefix = "[";
                    postfix = "]";
                    break;
                case BracketType.SolidSquareBrackets:
                    prefix = "【";
                    postfix = "】";
                    break;
                case BracketType.HollowSquareBrackets:
                    prefix = "〖";
                    postfix = "〗";
                    break;
            }
            return string.Format("{0}{1}{2}", prefix, text, postfix);
        }

        /// <summary>
        /// 将字符串格式化为SQL字符串。增加"'"并将其中的"'"替换为"''"(带外面的引号)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToSqlString(this string text)
        {
            return string.Format("'{0}'", text.Replace("'", "''"));
        }
        /// <summary>
        /// 将字符串格式化为SQL字符串。增加"'"并将其中的"'"替换为"''"(不带外面的引号)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToSQLField(this string text)
        {
            if (text == null) return string.Empty;
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("'", "''");
        }
        /// <summary>
        /// 获取字符串不区分大小写得哈希码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int GetHashCodeIngoreCase(this string text)
        {
            if (text == null) return 0;
            return StringComparer.OrdinalIgnoreCase.GetHashCode(text);
        }

        /// <summary>
        /// 确定此字符串忽略大小写是否与指定的 String 对象具有相同的值。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string source, string target)
        {
            if (source == null) return target == null;

            return source.Equals(target, StringComparison.CurrentCultureIgnoreCase);
        }
        /// <summary>
        /// 返回当前实例是否一个合法的标识符名称。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsValidIdentifierName(this string source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                UnicodeCategory unicodeCategory = char.GetUnicodeCategory(source[i]);
                bool flag = ((((unicodeCategory == UnicodeCategory.UppercaseLetter) || (unicodeCategory == UnicodeCategory.LowercaseLetter)) || ((unicodeCategory == UnicodeCategory.TitlecaseLetter) || (unicodeCategory == UnicodeCategory.OtherLetter))) || (unicodeCategory == UnicodeCategory.LetterNumber)) || (source[i] == '_');
                bool flag2 = (((unicodeCategory == UnicodeCategory.NonSpacingMark) || (unicodeCategory == UnicodeCategory.SpacingCombiningMark)) || (unicodeCategory == UnicodeCategory.ModifierLetter)) || (unicodeCategory == UnicodeCategory.DecimalDigitNumber);
                if (i == 0)
                {
                    if (!flag)
                    {
                        return false;
                    }
                }
                else if (!flag && !flag2)
                {
                    return false;
                }
            }
            return true;
        }

        #region 字串截取
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Cut(this string source, int start, int length)
        {
            return source.Left(start) + source.Right(start + length);
        }

        /// <summary>
        /// 截取指字字节数的字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="lengthB">字节数</param>
        /// <returns></returns>
        public static string CutB(this string source, int lengthB)
        {
            //格式化字符串长度，超出部分显示省略号,区分汉字跟字母。汉字2个字节，字母数字一个字节
            string temp = string.Empty;
            if (System.Text.Encoding.Default.GetByteCount(source) <= lengthB)//如果长度比需要的长度n小,返回原字符串
            {
                return source;
            }
            else
            {
                int t = 0;
                char[] q = source.ToCharArray();
                for (int i = 0; i < q.Length; i++)
                {
                    if (q[i].IsChineseCharacter())//是否汉字
                    {
                        temp += q[i];
                        t += 2;
                    }
                    else
                    {
                        temp += q[i];
                        t += 1;
                    }
                    if (t >= lengthB)
                    {
                        break;
                    }
                }
                return temp;
            }
        }

        /// <summary>
        /// 返回此实例中和分隔符匹配指定次数后的剩余片断
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public static string CutStart(this string source, string separator, int skip)
        {
            int pos = source.IndexOf(separator);
            for (int i = 0; i < skip && pos > -1; i++)
            {
                pos = source.IndexOf(separator, pos + separator.Length);
            }

            if (pos > -1)
                return source.Substring(pos + separator.Length);
            else
                return string.Empty;
        }

        /// <summary>
        /// 返回此实例中和分隔符由后向前匹配指定次数后的剩余片断
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public static string CutEnd(this string source, string separator, int skip)
        {
            int pos = source.LastIndexOf(separator);
            for (int i = 0; i < skip && pos > -1; i++)
            {
                pos = source.LastIndexOf(separator, pos - 1);
            }
            if (pos > -1)
                return source.Substring(0, pos);
            else
                return string.Empty;
        }

        /// <summary>
        /// 返回此实例切除尾部指定长度的后的剩余片段。超出范围时返回string.Empty。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CutEnd(this string source, int length)
        {
            int len = source.Length - length;
            if (len > 0)
                return source.Substring(0, len);
            else
                return string.Empty;
        }

        /// <summary>
        /// 返回当前实例的左部字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(this string source, int length)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            else if (source.Length <= length)
                return source;
            else if (length > 0)
                return source.Substring(0, length);
            else
                return string.Empty;
        }

        /// <summary>
        /// 返回当前实例的右部字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static string Right(this string source, int start)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            else if (start > -1 && start < source.Length)
                return source.Substring(start);
            else
                return string.Empty;
        }


        /// <summary>
        /// 从此实例检索子字符串。子字符串从指定的字符位置开始。
        /// <para>当指定的字符位置大于字符串长度是返回string.Empty</para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string SubstringEx(this string source, int startIndex)
        {
            if (startIndex >= source.Length)
                return string.Empty;

            return source.Substring(startIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Substring(this string source, int startIndex, int length, string defaultValue)
        {
            if (string.IsNullOrEmpty(source)) return defaultValue;

            if (startIndex >= source.Length || startIndex < 0)
                return defaultValue;
            else if (length == int.MaxValue || startIndex + length > source.Length)
                return source.Substring(startIndex);
            else
                return source.Substring(startIndex, length);
        }
        #endregion

        /// <summary>
        /// 返回字符串中指定字符的个数。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int Count(this string source, char c)
        {
            int count = 0;
            foreach (var item in source)
            {
                if (item == c) count++;
            }
            return count;
        }

        /// <summary>
        /// 指示指定的 String 对象是否包含双字节字符。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsUnicode(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return Regex.IsMatch(source, @"[^\x00-\xff]");
        }
        /// <summary>
        /// 指示指定的 String 对象是否为 ASCII
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsAscii(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return true;

            return Regex.IsMatch(source, @"^[\x21-\x47]+$");
        }

        /// <summary>
        /// 指示指定的 String 对象是否数值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return Regex.IsMatch(source, @"^[+-]?([0-9]+(\.[0-9]*)?|\.[0-9]+)$");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsUpperNumeric(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return Regex.IsMatch(source, @"^[零〇一二三四五六七八九十]+$");
        }
        /// <summary>
        /// 指示指定的 String 对象是否整数。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsInteger(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return Regex.IsMatch(source, @"^[+-]?[0-9]+$");
        }

        /// <summary>
        /// 指示指定的 String 对象是否字母
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsAlphabetic(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return Regex.IsMatch(source, @"^[a-zA-Z]*$");
        }

        /// <summary>
        /// 指示指定的 String 对象是否字母或数字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsAlphNum(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return Regex.IsMatch(source, @"^[a-zA-Z0-9.+\-]*$");
        }
        /// <summary>
        /// 指示指定的 String 对象是否标点符号
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsPunctuation(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            foreach (char c in source)
            {
                if (!char.IsPunctuation(c))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 指示指定的 String 对象是否十六进制数值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsHexNumber(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return Regex.IsMatch(source, @"^[0-9a-fA-F]*$");
        }

        /// <summary>
        /// 指示指定的 String 对象是否 Guid
        /// <para>{8位-4位-4位-4位-12位}</para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsGuid(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return Regex.IsMatch(source, @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$");
        }

        /// <summary>
        /// 是否IP4地址。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIpAddress(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return Regex.IsMatch(source, @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
        }

        /// <summary>
        /// 将指定的 String 对象转换为数字串。其中的字母将转换为数字，规则：A->1...Z->26。去除非字母和数字字符。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToNumString(this string source)
        {
            if (source.IsNumeric())
                return source;

            StringBuilder sb = new StringBuilder();
            foreach (var item in source)
            {
                if (char.IsLetter(item))
                {
                    sb.Append((int)(char.ToUpper(item) - 'A') + 1);
                }
                else if (char.IsDigit(item))
                {
                    sb.Append(item);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 去除头尾标点符号。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string TrimPunctuations(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            return source.Trim(_punctuations);
        }

        static char[] _punctuations = new char[] { ' ', ',', '，', '.', '。', '?', '？', ';', '；', ':', '：', '\t', '、' };
        /// <summary>
        /// 替换标点符号左侧部分。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ReplaceLeft(this string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return target;

            int x = source.IndexOfAny(_punctuations);

            if (x < 0) return target;

            return string.Concat(target, source.Right(x));
        }

        /// <summary>
        /// 替换标点符号右侧部分。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ReplaceRight(this string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return target;

            int x = source.LastIndexOfAny(_punctuations) + 1;

            if (x <= 0) return target;

            return string.Concat(source.Left(x), target);
        }

        /// <summary>
        /// 创建一个 StringWriter
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        public static StringWriter CreateWriter(this StringBuilder sb)
        {
            return new StringWriter(sb);
        }

        /// <summary>
        /// 将当前实例中的值转换为 String。
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string ToString(this StringBuilder sb, int startIndex)
        {
            if (startIndex >= sb.Length) return string.Empty;

            return sb.ToString(startIndex, sb.Length - startIndex);
        }

        /// <summary>
        /// 将长度为0的实例转换为null
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string EmptyAsNull(this string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return null;

            return source;
        }

        #region 连接
        /// <summary>
        /// 连接两个字符串。忽略空字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static string Coalesce(this string source, string target, string sp)
        {
            return source.Coalesce(target, sp, false);
        }

        /// <summary>
        /// 连接两个字符串。忽略空字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="sp"></param>
        /// <param name="ingoreExists">源字符串包含目标字符串时，忽略操作</param>
        /// <returns></returns>
        public static string Coalesce(this string source, string target, string sp, bool ingoreExists)
        {
            if (string.IsNullOrEmpty(source))
                return target ?? source;
            else if (string.IsNullOrEmpty(target))
                return source ?? target;
            else if (ingoreExists && source.Contains(target))
                return source;
            else
                return source + sp + target;
        }

        /// <summary>
        /// 保证字符以指定字符结尾。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        public static string CheckEndsWith(this string source, string tail)
        {
            if (string.IsNullOrEmpty(source))
                return tail;
            else if (source.EndsWith(tail))
                return source;
            else
                return source + tail;
        }
        #endregion

        #region 全角半角转换
        /// <summary>
        /// 将当前字符串实例转换为全角的形式(SBC case)。
        /// </summary>
        /// <param name="input"></param>
        /// <returns>全角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToSBC(this string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }


        /// <summary> 将当前字符串实例转换为半角的形式(DBC case) </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        #endregion

        #region 相似度比较
        /// <summary>
        /// 相似度比较（1－全匹配；返回0～1之间的double值，值越大越相似）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static double SimilarCompare(this string source, string target)
        {
            double max = Math.Max(source.Length, target.Length);
            double ld = LevenshteinDistance(source, target);

            return (max - ld) / max;
        }

        private static int Minimum(int a, int b, int c)
        {
            int mi = a;
            if (b < mi) mi = b;
            if (c < mi) mi = c;
            return mi;
        }

        private static int LevenshteinDistance(String s, String t)
        {
            int slen = s.Length;
            int tlen = t.Length;
            int[,] matrix = new int[slen + 1, tlen + 1];
            char ci, cj;
            int cost;

            if (slen == 0) return tlen;
            if (tlen == 0) return slen;

            for (int i = 0; i <= slen; i++) matrix[i, 0] = i;
            for (int j = 0; j <= tlen; j++) matrix[0, j] = j;

            for (int i = 1; i <= slen; i++)
            {
                ci = s[i - 1];
                for (int j = 1; j <= tlen; j++)
                {
                    cj = t[j - 1];
                    cost = (ci == cj) ? 0 : 1;
                    matrix[i, j] = Minimum(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1, matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[slen, tlen];
        }
        #endregion

        #region Xml缩进格式化
        /// <summary>
        /// 将Xml内容格式化成缩进4个空格
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string XmlFormatIndented(this string source)
        {
            return XmlFormatIndented(source, 4, ' ');
        }
        /// <summary>
        /// 将Xml内容格式化成缩进指定个数的字符
        /// </summary>
        /// <param name="source"></param>
        /// <param name="indentation">每个级别编写多少个indentChar</param>
        /// <param name="IndentChar">用于缩进的字符</param>
        /// <returns></returns>
        public static string XmlFormatIndented(this string source, int indentation, char indentChar)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(source);
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (XmlTextWriter xtw = new XmlTextWriter(sw))
                {
                    xtw.Formatting = Formatting.Indented;
                    xtw.Indentation = indentation;
                    xtw.IndentChar = indentChar;
                    xd.WriteTo(xtw);
                }
            }
            return sb.ToString();
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public static class CharExtensions
    {
        /// <summary>
        /// 是否是汉字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsChineseCharacter(this char source)
        {
            return (int)source >= 0x4E00 && (int)source <= 0x9FA5 || IsChineseSign(source);
        }

        private static bool IsChineseSign(char source)
        {
            char[] chrs = { source };
            byte[] b = System.Text.Encoding.Unicode.GetBytes(chrs, 0, 1);
            if (b.Length == 2 && b[1] == 255) return true;
            if (source.ToString().LenC() > 1) return true;
            //不管是否中文，只要占2位，就当作是中文
            //if (b.Length == 2) return true;
            return false;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "lstrlenA", CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        internal static extern int LenAPI(string ptr);

        /// <summary>
        /// 获取字符串的ANSI字符码长度。
        /// </summary>
        /// <param name="strInfo"></param>
        /// <returns></returns>
        public static int LenC(this string strInfo)
        {
            return LenAPI(strInfo);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class ByteExtensionsii
    {
        /// <summary>
        /// 转ASCII
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToChar(this byte source)
        {

            return Convert.ToChar(source).ToString();
        }
    }

    /// <summary>
    /// 文本括符类型
    /// </summary>
    public enum BracketType
    {
        /// <summary>
        /// 单引号''
        /// </summary>
        SingleQuotation,
        /// <summary>
        /// 双引号""
        /// </summary>
        DoubleQuotation,
        /// <summary>
        /// 大括号{}
        /// </summary>
        BigQuotation,
        /// <summary>
        ///  圆括号()
        /// </summary>
        Parentheses,
        /// <summary>
        ///  中文圆括号（）
        /// </summary>
        ParenthesesChs,
        /// <summary>
        /// 方括号[]
        /// </summary>
        SquareBrackets,
        /// <summary>
        /// 书名号《》
        /// </summary>
        FrenchQuotes,
        /// <summary>
        ///  中文单引号‘’
        /// </summary>
        SingleQuotationChs,
        /// <summary>
        /// 中文双引号“”
        /// </summary>
        DoubleQuotationChs,
        /// <summary>
        /// 实心方括号【】
        /// </summary>
        SolidSquareBrackets,
        /// <summary>
        /// 空心方括号〖〗
        /// </summary>
        HollowSquareBrackets
    }


    /// <summary>
    /// 表示汉字编码信息
    /// </summary>
    public interface ICharacterCode
    {
        /// <summary>
        /// 字符
        /// </summary>
        string Character { get; }
        /// <summary>
        /// 五笔码
        /// </summary>
        string WBCode { get; }
        /// <summary>
        /// 五笔首码
        /// </summary>
        string WBCode1 { get; }
        /// <summary>
        /// 拼音码
        /// </summary>
        string SpellCode { get; }
        /// <summary>
        /// 拼音首码
        /// </summary>
        string SpellCode1 { get; }
    }
}
