using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
namespace punycode转换
{
    /// <summary>
    /// Punycode IDN编码操作
    /// </summary>
    /// 示例
    /// 解码punycode
    /// string wenben = punycode转换.编码项目(“中国.香港”);
    /// 输入后返回转换的代码
    /// console.writelin(wenben);
    /// string.wenben2;
    /// console.writelin(punycode转换.解码项目(wenben2));
    public class Punycode转换
    {
        /* Punycode parameters */
        static int TMIN = 1;
        static int TMAX = 26;
        static int BASE = 36;
        static int INITIAL_N = 128;
        static int INITIAL_BIAS = 72;
        static int DAMP = 700;
        static int SKEW = 38;
        static char DELIMITER = '-';
        public static string 编码项目(string 项目)
        {
            项目 = 转换字符(项目);
            string[] 项目列 = 项目.Split(new string[] { "." }, StringSplitOptions.None);
            string 结果 = "";
            foreach (string 临时 in 项目列)//遍历
            {
                if (临时 == "")
                {
                    结果 += ".";
                    continue;
                }
                结果 += "xn--" + 转换码(临时) + ".";
            }
            if (结果[结果.Length - 1] == '.') 结果 = 结果.Substring(0, 结果.Length - 1);
            return 结果;
        }

        private static string 转换字符(string 返回项目)
        {
            返回项目 = 返回项目.Replace("。", ".");//转换“。·，·.//
            return 返回项目;//返回domains
        }

        public static string 解码项目(string 项目)
        {
            项目 = 转换字符(项目);
            string[] 项目列 = 项目.Split(new string[] { "." }, StringSplitOptions.None);
            string result = "";
            foreach (string 临时 in 项目列)
            {
                if (临时 == "")
                {
                    result += ".";
                    continue;
                }
                string 临时2 = 临时;
                if (临时2.Length > 4 && 临时2.Substring(0, 4) == "xn--")
                {
                    result += 解码(临时2.Substring(4, 临时2.Length - 4)) + ".";
                }
            }
            try
            {
                if (result[result.Length - 1] == '.') result = result.Substring(0, result.Length - 1);
            }
            catch (IndexOutOfRangeException)
            {
                result = "错误的输入";
                return result;
            }
            return result;
        }
        public static string 转换码(string 输入)
        {
            int n = INITIAL_N;
            int delta = 0;
            int bias = INITIAL_BIAS;
            StringBuilder 输出 = new StringBuilder();
            // Copy all basic code points to the output
            int b = 0;
            for (int i = 0; i < 输入.Length; i++)
            {
                char c = 输入[i];
                if (IsBasic(c))
                {
                    输出.Append(c);
                    b++;
                }
            }
            // Append delimiter
            if (b > 0)
            {
                输出.Append(DELIMITER);
            }
            int h = b;
            while (h < 输入.Length)
            {
                int m = int.MaxValue;
                // Find the minimum code point >= n
                for (int i = 0; i < 输入.Length; i++)
                {
                    int c = 输入[i];
                    if (c >= n && c < m)
                    {
                        m = c;
                    }
                }
                if (m - n > (int.MaxValue - delta) / (h + 1))
                {
                    throw new Exception("OVERFLOW");
                }
                delta = delta + (m - n) * (h + 1);
                n = m;
                for (int j = 0; j < 输入.Length; j++)
                {
                    int c = 输入[j];
                    if (c < n)
                    {
                        delta++;
                        if (0 == delta)
                        {
                            throw new Exception("OVERFLOW");
                        }
                    }
                    if (c == n)
                    {
                        int q = delta;
                        for (int k = BASE; ; k += BASE)
                        {
                            int t;
                            if (k <= bias)
                            {
                                t = TMIN;
                            }
                            else if (k >= bias + TMAX)
                            {
                                t = TMAX;
                            }
                            else
                            {
                                t = k - bias;
                            }
                            if (q < t)
                            {
                                break;
                            }
                            输出.Append((char)Digit2codepoint(t + (q - t) % (BASE - t)));
                            q = (q - t) / (BASE - t);
                        }
                        输出.Append((char)Digit2codepoint(q));
                        bias = Adapt(delta, h + 1, h == b);
                        delta = 0;
                        h++;
                    }
                }
                delta++;
                n++;
            }
            return 输出.ToString();
        }
        public static string 解码(string input)
        {
            int n = INITIAL_N;
            int i = 0;
            int bias = INITIAL_BIAS;
            StringBuilder output = new StringBuilder();
            int d = input.LastIndexOf(DELIMITER);
            if (d > 0)
            {
                for (int j = 0; j < d; j++)
                {
                    char c = input[j];
                    if (!IsBasic(c))
                    {
                        throw new Exception("BAD_INPUT");
                    }
                    output.Append(c);
                }
                d++;
            }
            else
            {
                d = 0;
            }
            while (d < input.Length)
            {
                int oldi = i;
                int w = 1;
                for (int k = BASE; ; k += BASE)
                {
                    if (d == input.Length)
                    {
                        throw new Exception("BAD_INPUT");
                    }
                    int c = input[d++];
                    int digit = Codepoint2digit(c);
                    if (digit > (int.MaxValue - i) / w)
                    {
                        throw new Exception("OVERFLOW");
                    }
                    i = i + digit * w;
                    int t;
                    if (k <= bias)
                    {
                        t = TMIN;
                    }
                    else if (k >= bias + TMAX)
                    {
                        t = TMAX;
                    }
                    else
                    {
                        t = k - bias;
                    }
                    if (digit < t)
                    {
                        break;
                    }
                    w = w * (BASE - t);
                }
                bias = Adapt(i - oldi, output.Length + 1, oldi == 0);
                if (i / (output.Length + 1) > int.MaxValue - n)
                {
                    throw new Exception("OVERFLOW");
                }
                n = n + i / (output.Length + 1);
                i = i % (output.Length + 1);
                output.Insert(i, (char)n);
                i++;
            }
            return output.ToString();
        }
        private static int Adapt(int delta, int numpoints, bool first)
        {
            if (first)
            {
                delta = delta / DAMP;
            }
            else
            {
                delta = delta / 2;
            }
            delta = delta + (delta / numpoints);
            int k = 0;
            while (delta > ((BASE - TMIN) * TMAX) / 2)
            {
                delta = delta / (BASE - TMIN);
                k = k + BASE;
            }
            return k + ((BASE - TMIN + 1) * delta) / (delta + SKEW);
        }
        private static bool IsBasic(char c)
        {
            return c < 0x80;
        }
        private static int Digit2codepoint(int d)
        {
            if (d < 26)
            {
                // 0..25 : 'a'..'z'
                return d + 'a';
            }
            else if (d < 36)
            {
                // 26..35 : '0'..'9';
                return d - 26 + '0';
            }
            else
            {
                throw new Exception("BAD_INPUT");
            }
        }
        private static int Codepoint2digit(int c)
        {
            if (c - '0' < 10)
            {
                // '0'..'9' : 26..35
                return c - '0' + 26;
            }
            else if (c - 'a' < 26)
            {
                // 'a'..'z' : 0..25
                return c - 'a';
            }
            else
            {
                throw new Exception("BAD_INPUT");
            }
        }
    }
}