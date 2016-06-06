using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;


namespace System
{
    public static partial class StringExtensions
    {
        private const string Ellipsis = "...";

        public static T Deserialize<T>(this string value)
        {
            return (T)(value.IsNullOrEmpty()
                       ? default(T)
                       : new XmlSerializer(typeof (T)).Deserialize(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(value)), Encoding.UTF8)));
        }
        /// <summary>
        /// Extension alias of the String.Format, used for brevity.
        /// </summary>
        /// <param name="target">The string to format.</param>
        /// <param name="args">The arguments to substitute in the string.</param>
        /// <returns>The formatted string.</returns>
        public static string F(this string target, params object[] args)
        {
            // #Format
            return String.Format(target, args);
        }

        public static bool IsNotNullOrWhiteSpace(this String value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }

        public static bool IsNullOrWhiteSpace(this String value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        public static bool IsValidEmail(this string email)
        {
            var emailRegex = new Regex(@"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            return email.IsNotNullOrWhiteSpace() && emailRegex.IsMatch(email);
        }

        /// <summary>
        /// Converts the string to Guid or Guid.Empty is it fails
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string guid)
        {
            Guid value;
            Guid.TryParse(guid, out value);
            return value;
        }

        public static string StringJoin<T>(this IEnumerable<T> enumerable, string valueEnclosure, string separator)
        {
            var stringBuilder = new StringBuilder();
            if (enumerable.IsNotNull())
                enumerable.ForEach(
                    element => stringBuilder.Append(stringBuilder.Length == 0
                                                        ? string.Format("{0}{1}{0}", valueEnclosure, element)
                                                        : string.Format("{2}{0}{1}{0}", valueEnclosure, element, separator)));
            return stringBuilder.ToString();
        }


        public static String NullIfEmpty(this String value)
        {
            return value.IsNotNullOrWhiteSpace() ? value : null;
        }

        public static String EmptyIfNull(this String value)
        {
            return value ?? string.Empty;
        }

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns>Input string if its lengh is OK; otherwise, truncated input string</returns>
        public static string EnsureMaximumLength(this string str, int maxLength)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
                return str.Substring(0, maxLength);
            else
                return str;
        }

        public static StringBuilder Append(this StringBuilder stringBuilder, params object[] values)
        {
            if (!values.IsNullOrEmpty())
                values.ForEach(value => stringBuilder.Append(value));
            return stringBuilder;
        }

        public static String ToTitleCase(this String thePhrase)
        {
            var newString = new StringBuilder();
            var nextString = new StringBuilder();
            string theWord;
            var phraseArray = thePhrase.Split(null);
            foreach (var t in phraseArray)
            {
                theWord = t.ToLower();
                if (theWord.Length > 1)
                {
                    if (theWord.Substring(1, 1) == "'")
                    {
                        //Process word with apostrophe at position 1 in 0 based string.
                        if (nextString.Length > 0)
                            nextString.Replace(nextString.ToString(), null);
                        nextString.Append(theWord.Substring(0, 1).ToUpper());
                        nextString.Append("'");
                        nextString.Append(theWord.Substring(2, 1).ToUpper());
                        nextString.Append(theWord.Substring(3).ToLower());
                        nextString.Append(" ");
                    }
                    else
                    {
                        //Process normal word (possible apostrophe near end of word.
                        if (nextString.Length > 0)
                            nextString.Replace(nextString.ToString(), null);
                        nextString.Append(theWord.Substring(0, 1).ToUpper());
                        nextString.Append(theWord.Substring(1).ToLower());
                        nextString.Append(" ");
                    }
                }
                else
                {
                    //Process normal single character length word.
                    if (nextString.Length > 0)
                        nextString.Replace(nextString.ToString(), null);
                    nextString.Append(theWord.ToUpper());
                    nextString.Append(" ");
                }
                newString.Append(nextString);
            }
            return newString.ToString().Trim();
        }

        public static String RemoveInvalidPathChars(this String value)
        {
            if (value == null) return value;

            Path
                .GetInvalidPathChars()
                .ForEach(c =>
                {
                    value = value.Replace(c.ToString(), "");
                });
            return value;
        }


        private static readonly Regex s_StripHtmlRegex = new Regex("<[^>]*>", RegexOptions.Compiled);


        public static string StripHtml(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            html = s_StripHtmlRegex.Replace(html, string.Empty);
            html = html.Replace("&nbsp;", " ");

            return html.Replace("&#160;", string.Empty);
        }

        public static bool ContainsHtml(this string input)
        {
            return !string.IsNullOrEmpty(input) && s_StripHtmlRegex.IsMatch(input);
        }

        public static String HtmlDecode(this string value)
        {
            return value.IsNull() ? null : HttpUtility.HtmlDecode(value);
        }

        public static String HtmlEncode(this string value)
        {
            return value.IsNull() ? null : HttpUtility.HtmlEncode(value);
        }

        public static String UrlDecode(this string value)
        {
            return value.IsNull() ? null : HttpUtility.UrlDecode(value);
        }

        public static String UrlEncode(this string value)
        {
            return value.IsNull() ? null : HttpUtility.UrlEncode(value);
        }

        public static string ReplaceLineBreaksWithBr(this string lines)
        {
            const string replacement = "<br/>";
            return lines.IsNullOrWhiteSpace() ? "" :  
                    lines.Replace("\r\n", replacement)
                        .Replace("\r", replacement)
                        .Replace("\n", replacement);
        }

        public static string HttpUrl(this string url)
        {
            if (url.IsNullOrWhiteSpace()) return "#";
            return url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ? url : ("http://" + url);
        }

        public static string URLFriendly(this string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            var len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }

        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Substring but OK if shorter
        /// </summary>
        public static string Truncate(this string str, int characterCount)
        {
            if (str.IsNullOrWhiteSpace()) return string.Empty;
            return str.Length <= characterCount ? str : str.Substring(0, characterCount).TrimEnd(' ');
        }

        /// <summary>
        /// Substring with elipses but OK if shorter, will take 3 characters off character count if necessary
        /// </summary>
        public static string TruncateWithElipses(this string str, int characterCount)
        {
            if (str.IsNullOrWhiteSpace()) return string.Empty;
            if (characterCount < 5) return str.Truncate(characterCount);       // Can’t do much with such a short limit
            if (str.Length <= characterCount - 3) return str;
            return str.Substring(0, characterCount - 3).TrimEnd(' ') + Ellipsis;
        }

        /// <summary>
        /// Truncates the supplied content to the max length on a word boundary and adds an ellipsis if longer.
        /// </summary>
        /// <param name="content">The string to truncate</param>
        /// <param name="maxLength">Max number of characters to show, not including continuation content</param>
        /// <returns>The truncated string</returns>
        public static string TruncateOnWordBoundaryWithElipses(this string content, int maxLength)
        {
            if (content.IsNullOrWhiteSpace()) return string.Empty;
            return content.TruncateOnWordBoundaryWithElipses(maxLength, Ellipsis);
        }

        /// <summary>
        /// Truncates the supplied content to the max length on a word boundary and adds the suffix if longer.
        /// </summary>
        /// <param name="content">The string to truncate</param>
        /// <param name="maxLength">Max number of characters to show, not including the suffix</param>
        /// <param name="suffix">Suffix to append if content is truncated</param>
        /// <returns>The truncated string</returns>
        public static string TruncateOnWordBoundaryWithElipses(this string content, int maxLength, string suffix)
        {
            // No content? Return an empty string.
            if (String.IsNullOrEmpty(content))
                return String.Empty;

            // Content is shorter than the max length? Return the whole string.
            if (content.Length < maxLength)
                return content;

            // Find the word boundary.
            int i = maxLength;
            while (i > 0)
            {
                if (Char.IsWhiteSpace(content[i]))
                    break;
                i--;
            }

            // Can't truncate on a word boundary? Just return the suffix, e.g. "...".
            if (i <= 0)
                return (suffix ?? Ellipsis); // Just in case a null suffix was supplied.

            return content.Substring(0, i) + (suffix ?? Ellipsis);
        }

        /// <summary>
        /// Strips tags
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string StripTags(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = Regex.Replace(text, @"(>)(\r|\n)*(<)", "><");
            text = Regex.Replace(text, "(<[^>]*>)([^<]*)", "$2");
            text = Regex.Replace(text, "(&#x?[0-9]{2,4};|&quot;|&amp;|&nbsp;|&lt;|&gt;|&euro;|&copy;|&reg;|&permil;|&Dagger;|&dagger;|&lsaquo;|&rsaquo;|&bdquo;|&rdquo;|&ldquo;|&sbquo;|&rsquo;|&lsquo;|&mdash;|&ndash;|&rlm;|&lrm;|&zwj;|&zwnj;|&thinsp;|&emsp;|&ensp;|&tilde;|&circ;|&Yuml;|&scaron;|&Scaron;)", "@");

            return text;
        }

        /// <summary>
        /// Converts plain text to HTML
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string ConvertPlainTextToHtml(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = text.Replace("\r\n", "<br />");
            text = text.Replace("\r", "<br />");
            text = text.Replace("\n", "<br />");
            text = text.Replace("\t", "&nbsp;&nbsp;");
            text = text.Replace("  ", "&nbsp;&nbsp;");

            return text;
        }

        /// <summary>
        /// Converts HTML to plain text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="decode">A value indicating whether to decode text</param>
        /// <returns>Formatted text</returns>
        public static string ConvertHtmlToPlainText(this string text, bool decode = false)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            if (decode)
                text = HttpUtility.HtmlDecode(text);

            text = text.Replace("<br>", "\n");
            text = text.Replace("<br >", "\n");
            text = text.Replace("<br />", "\n");
            text = text.Replace("&nbsp;&nbsp;", "\t");
            text = text.Replace("&nbsp;&nbsp;", "  ");

            return text;
        }

        private readonly static Regex s_ParagraphStartRegex = new Regex("<p>", RegexOptions.IgnoreCase);
        private readonly static Regex s_ParagraphEndRegex = new Regex("</p>", RegexOptions.IgnoreCase);

        /// <summary>
        /// Converts text to paragraph
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string ConvertPlainTextToParagraph(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = s_ParagraphStartRegex.Replace(text, string.Empty);
            text = s_ParagraphEndRegex.Replace(text, "\n");
            text = text.Replace("\r\n", "\n").Replace("\r", "\n");
            text = text + "\n\n";
            text = text.Replace("\n\n", "\n");
            var strArray = text.Split(new char[] { '\n' });
            var builder = new StringBuilder();
            foreach (string str in strArray)
            {
                if ((str != null) && (str.Trim().Length > 0))
                {
                    builder.AppendFormat("<p>{0}</p>\n", str);
                }
            }
            return builder.ToString();
        }

        public static string SafeHtmlInputValue(this string  value, char enclosure='"')
        {
            if (value.IsNullOrWhiteSpace()) return value;

            if(enclosure == '"')
                value = value.Replace("\"", "\\\"");
            else if(enclosure == '\'')
                value = value.Replace("'", "\\\'");
            return value;
        }

        private static readonly Regex s_Tags = new Regex("<[^>]*(>|$)",
    RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        private static readonly Regex s_Whitelist = new Regex(@"
    ^</?(b(lockquote)?|code|d(d|t|l|el)|em|h(1|2|3)|i|kbd|li|ol|p(re)?|s(ub|up|trong|trike)?|ul)>$|
    ^<(b|h)r\s?/?>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex s_WhitelistA = new Regex(@"
    ^<a\s
    href=""(\#\d+|(https?|ftp)://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+)""
    (\stitle=""[^""<>]+"")?\s?>$|
    ^</a>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex s_WhitelistImg = new Regex(@"
    ^<img\s
    src=""https?://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+""
    (\swidth=""\d{1,3}"")?
    (\sheight=""\d{1,3}"")?
    (\salt=""[^""<>]*"")?
    (\stitle=""[^""<>]*"")?
    \s?/?>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);


        /// <summary>
        /// sanitize any potentially dangerous tags from the provided raw HTML input using 
        /// a whitelist based approach, leaving the "safe" HTML tags
        /// CODESNIPPET:4100A61A-1711-4366-B0B0-144D1179A937
        /// </summary>
        public static string SafeHtml(this string html)
        {
            if (String.IsNullOrEmpty(html)) return html;

            string tagname;
            Match tag;

            // match every HTML tag in the input
            var tags = s_Tags.Matches(html);
            for (int i = tags.Count - 1; i > -1; i--)
            {
                tag = tags[i];
                tagname = tag.Value.ToLowerInvariant();

                if (!(s_Whitelist.IsMatch(tagname) || s_WhitelistA.IsMatch(tagname) || s_WhitelistImg.IsMatch(tagname)))
                {
                    html = html.Remove(tag.Index, tag.Length);
                }
            }

            return html;
        }

    }
}
