using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace TFSideKicks.Helpers
{
    /// <summary>
    /// 命令行参数类
    /// 测试参数如下：-param0 -param1 value1 --param2 value2 /param3 value3 =param4 value4 :param5 value5 -param6=param6 -param7:param7 -param8.1 "1234" -param8.2 "1 2 3 4" -param9.1 '1234' -param9.2='1 2 3 4'
    /// 各参数项测试内容如下：
    /// -param0，只有参数项
    /// -param1 value1，有参数项，有参数值
    /// --param2 value2，用--标记参数项开头
    /// /param value3，用/标记参数项开头
    /// =param4 value4，用=标记参数项开头
    /// :param5 value5，用:标记参数项开头
    /// -param6=param6，用=标记参数项与参数值的关系
    /// -param7:param7，用:标记参数项与参数值的关系
    /// -param8.1 "1234"，用""指定参数
    /// -param8.2 "1 2 3 4"，用""指定参数（含空格）
    /// -param9.1 '1234'，用''指定参数
    /// -param9.2='1 2 3 4'，用''指定参数（含空格）
    /// </summary>
    public class CommandLineArguments
    {
        // Variables
        private StringDictionary _parameters;

        // Constructor
        public CommandLineArguments(string[] Args)
        {
            Parameters = new StringDictionary();
            Regex spliter = new Regex(@"^-{1,2}|^/|=|:",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Regex remover = new Regex(@"^['""]?(.*?)['""]?$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;
            string[] parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string txt in Args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                parts = spliter.Split(txt, 3);

                switch (parts.Length)
                {
                    // Found a value (for the last parameter 
                    // found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            if (!Parameters.ContainsKey(parameter))
                            {
                                parts[0] =
                                    remover.Replace(parts[0], "$1");

                                Parameters.Add(parameter, parts[0]);
                            }
                            parameter = null;
                        }
                        // else Error: no parameter waiting for a value (skipped)
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!Parameters.ContainsKey(parameter))
                                Parameters.Add(parameter, "true");
                        }
                        parameter = parts[1];
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!Parameters.ContainsKey(parameter))
                                Parameters.Add(parameter, "true");
                        }

                        parameter = parts[1];

                        // Remove possible enclosing characters (",')
                        if (!Parameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            Parameters.Add(parameter, parts[2]);
                        }

                        parameter = null;
                        break;
                }
            }
            // In case a parameter is still waiting
            if (parameter != null)
            {
                if (!Parameters.ContainsKey(parameter))
                    Parameters.Add(parameter, "true");
            }
        }

        // Retrieve a parameter value if it exists 
        // (overriding C# indexer property)
        public string this[string param]
        {
            get
            {
                return (Parameters[param]);
            }
        }

        public StringDictionary Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
    }
}