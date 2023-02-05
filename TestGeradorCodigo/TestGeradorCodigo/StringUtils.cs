using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestGeradorCodigo
{
    //
    // Summary:
    //     String utility class that provides a host of string related operations
    public static class StringUtils
    {
        private static Regex tokenizeRegex = new Regex("{{.*?}}");

        private static Random random = new Random((int)DateTime.Now.Ticks);

        private static char[] base36CharArray = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();

        private static string base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz";

        private static object _logLock = new object();

        //
        // Summary:
        //     Trims a sub string from a string.
        //
        // Parameters:
        //   text:
        //
        //   textToTrim:
        public static string TrimStart(string text, string textToTrim, bool caseInsensitive)
        {
            while (true)
            {
                string text2 = text.Substring(0, textToTrim.Length);
                if (!(text2 == textToTrim) && (!caseInsensitive || !(text2.ToLower() == textToTrim.ToLower())))
                {
                    break;
                }

                text = ((text.Length > text2.Length) ? text.Substring(textToTrim.Length) : "");
            }

            return text;
        }

        //
        // Summary:
        //     Trims a string to a specific number of max characters
        //
        // Parameters:
        //   value:
        //
        //   charCount:
        [Obsolete("Please use the StringUtils.Truncate() method instead.")]
        public static string TrimTo(string value, int charCount)
        {
            if (value == null)
            {
                return value;
            }

            if (value.Length > charCount)
            {
                return value.Substring(0, charCount);
            }

            return value;
        }

        //
        // Summary:
        //     Replicates an input string n number of times
        //
        // Parameters:
        //   input:
        //
        //   charCount:
        public static string Replicate(string input, int charCount)
        {
            StringBuilder stringBuilder = new StringBuilder(input.Length * charCount);
            for (int i = 0; i < charCount; i++)
            {
                stringBuilder.Append(input);
            }

            return stringBuilder.ToString();
        }

        //
        // Summary:
        //     Replicates a character n number of times and returns a string You can use `new
        //     string(char, count)` directly though.
        //
        // Parameters:
        //   charCount:
        //
        //   character:
        public static string Replicate(char character, int charCount)
        {
            return new string(character, charCount);
        }

        //
        // Summary:
        //     Finds the nth index of string in a string
        //
        // Parameters:
        //   source:
        //
        //   matchString:
        //
        //   stringInstance:
        public static int IndexOfNth(this string source, string matchString, int stringInstance, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (string.IsNullOrEmpty(source))
            {
                return -1;
            }

            int num = 0;
            int num2 = 0;
            while (num2 < stringInstance)
            {
                int count = source.Length - num;
                num = source.IndexOf(matchString, num, count, stringComparison);
                if (num == -1)
                {
                    break;
                }

                num2++;
                if (num2 == stringInstance)
                {
                    return num;
                }

                num += matchString.Length;
            }

            return -1;
        }

        //
        // Summary:
        //     Returns the nth Index of a character in a string
        //
        // Parameters:
        //   source:
        //
        //   matchChar:
        //
        //   charInstance:
        public static int IndexOfNth(this string source, char matchChar, int charInstance)
        {
            if (string.IsNullOrEmpty(source))
            {
                return -1;
            }

            if (charInstance < 1)
            {
                return -1;
            }

            int num = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == matchChar)
                {
                    num++;
                    if (num == charInstance)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        //
        // Summary:
        //     Finds the nth index of strting in a string
        //
        // Parameters:
        //   source:
        //
        //   matchString:
        //
        //   charInstance:
        public static int LastIndexOfNth(this string source, string matchString, int charInstance, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (string.IsNullOrEmpty(source))
            {
                return -1;
            }

            int num = source.Length;
            int num2 = 0;
            while (num2 < charInstance)
            {
                num = source.LastIndexOf(matchString, num, num, stringComparison);
                if (num == -1)
                {
                    break;
                }

                num2++;
                if (num2 == charInstance)
                {
                    return num;
                }
            }

            return -1;
        }

        //
        // Summary:
        //     Finds the nth index of in a string from the end.
        //
        // Parameters:
        //   source:
        //
        //   matchChar:
        //
        //   charInstance:
        public static int LastIndexOfNth(this string source, char matchChar, int charInstance)
        {
            if (string.IsNullOrEmpty(source))
            {
                return -1;
            }

            int num = 0;
            for (int num2 = source.Length - 1; num2 > -1; num2--)
            {
                if (source[num2] == matchChar)
                {
                    num++;
                    if (num == charInstance)
                    {
                        return num2;
                    }
                }
            }

            return -1;
        }

        //
        // Summary:
        //     Return a string in proper Case format
        //
        // Parameters:
        //   Input:
        public static string ProperCase(string Input)
        {
            if (Input == null)
            {
                return null;
            }

            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Input);
        }

        //
        // Summary:
        //     Takes a phrase and turns it into CamelCase text. White Space, punctuation and
        //     separators are stripped
        //
        // Parameters:
        //   phrase:
        //     Text to convert to CamelCase
        public static string ToCamelCase(string phrase)
        {
            if (phrase == null)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder(phrase.Length);
            bool flag = true;
            foreach (char c in phrase)
            {
                if (char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsSeparator(c) || (c > ' ' && c < '0'))
                {
                    flag = true;
                    continue;
                }

                if (char.IsDigit(c))
                {
                    stringBuilder.Append(c);
                    flag = true;
                    continue;
                }

                if (flag)
                {
                    stringBuilder.Append(char.ToUpper(c));
                }
                else
                {
                    stringBuilder.Append(char.ToLower(c));
                }

                flag = false;
            }

            return stringBuilder.ToString();
        }

        //
        // Summary:
        //     Tries to create a phrase string from CamelCase text into Proper Case text. Will
        //     place spaces before capitalized letters. Note that this method may not work for
        //     round tripping ToCamelCase calls, since ToCamelCase strips more characters than
        //     just spaces.
        //
        // Parameters:
        //   camelCase:
        //     Camel Case Text: firstName -> First Name
        public static string FromCamelCase(string camelCase)
        {
            if (string.IsNullOrEmpty(camelCase))
            {
                return camelCase;
            }

            StringBuilder stringBuilder = new StringBuilder(camelCase.Length + 10);
            bool flag = true;
            char c = '\0';
            foreach (char c2 in camelCase)
            {
                if (!flag && c != ' ' && !char.IsSymbol(c) && !char.IsPunctuation(c) && ((char.IsUpper(c2) && !char.IsUpper(c)) || (char.IsDigit(c2) && !char.IsDigit(c))))
                {
                    stringBuilder.Append(' ');
                }

                stringBuilder.Append(c2);
                flag = false;
                c = c2;
            }

            return stringBuilder.ToString();
        }

        //
        // Summary:
        //     Extracts a string from between a pair of delimiters. Only the first instance
        //     is found.
        //
        // Parameters:
        //   source:
        //     Input String to work on
        //
        //   beginDelim:
        //     Beginning delimiter
        //
        //   endDelim:
        //     ending delimiter
        //
        //   caseSensitive:
        //     Determines whether the search for delimiters is case sensitive
        //
        //   allowMissingEndDelimiter:
        //
        //   returnDelimiters:
        //
        // Returns:
        //     Extracted string or string.Empty on no match
        public static string ExtractString(this string source, string beginDelim, string endDelim, bool caseSensitive = false, bool allowMissingEndDelimiter = false, bool returnDelimiters = false)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            int num;
            int num2;
            if (caseSensitive)
            {
                num = source.IndexOf(beginDelim, StringComparison.CurrentCulture);
                if (num == -1)
                {
                    return string.Empty;
                }

                num2 = source.IndexOf(endDelim, num + beginDelim.Length, StringComparison.CurrentCulture);
            }
            else
            {
                num = source.IndexOf(beginDelim, 0, source.Length, StringComparison.OrdinalIgnoreCase);
                if (num == -1)
                {
                    return string.Empty;
                }

                num2 = source.IndexOf(endDelim, num + beginDelim.Length, StringComparison.OrdinalIgnoreCase);
            }

            if (allowMissingEndDelimiter && num2 < 0)
            {
                if (!returnDelimiters)
                {
                    return source.Substring(num + beginDelim.Length);
                }

                return source.Substring(num);
            }

            if (num > -1 && num2 > 1)
            {
                if (!returnDelimiters)
                {
                    return source.Substring(num + beginDelim.Length, num2 - num - beginDelim.Length);
                }

                return source.Substring(num, num2 - num + endDelim.Length);
            }

            return string.Empty;
        }

        //
        // Summary:
        //     String replace function that supports replacing a specific instance with case
        //     insensitivity
        //
        // Parameters:
        //   origString:
        //     Original input string
        //
        //   findString:
        //     The string that is to be replaced
        //
        //   replaceWith:
        //     The replacement string
        //
        //   instance:
        //     Instance of the FindString that is to be found. 1 based. If Instance = -1 all
        //     are replaced
        //
        //   caseInsensitive:
        //     Case insensitivity flag
        //
        // Returns:
        //     updated string or original string if no matches
        public static string ReplaceStringInstance(string origString, string findString, string replaceWith, int instance, bool caseInsensitive)
        {
            if (instance == -1)
            {
                return ReplaceString(origString, findString, replaceWith, caseInsensitive);
            }

            int num = 0;
            for (int i = 0; i < instance; i++)
            {
                num = ((!caseInsensitive) ? origString.IndexOf(findString, num) : origString.IndexOf(findString, num, origString.Length - num, StringComparison.OrdinalIgnoreCase));
                if (num == -1)
                {
                    return origString;
                }

                if (i < instance - 1)
                {
                    num += findString.Length;
                }
            }

            return origString.Substring(0, num) + replaceWith + origString.Substring(num + findString.Length);
        }

        //
        // Summary:
        //     Replaces a substring within a string with another substring with optional case
        //     sensitivity turned off.
        //
        // Parameters:
        //   origString:
        //     String to do replacements on
        //
        //   findString:
        //     The string to find
        //
        //   replaceString:
        //     The string to replace found string wiht
        //
        //   caseInsensitive:
        //     If true case insensitive search is performed
        //
        // Returns:
        //     updated string or original string if no matches
        public static string ReplaceString(string origString, string findString, string replaceString, bool caseInsensitive)
        {
            int num = 0;
            while (true)
            {
                num = ((!caseInsensitive) ? origString.IndexOf(findString, num) : origString.IndexOf(findString, num, origString.Length - num, StringComparison.OrdinalIgnoreCase));
                if (num == -1)
                {
                    break;
                }

                origString = origString.Substring(0, num) + replaceString + origString.Substring(num + findString.Length);
                num += replaceString.Length;
            }

            return origString;
        }

        //
        // Summary:
        //     Truncate a string to maximum length.
        //
        // Parameters:
        //   text:
        //     Text to truncate
        //
        //   maxLength:
        //     Maximum length
        //
        // Returns:
        //     Trimmed string
        public static string Truncate(this string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (text.Length > maxLength)
            {
                return text.Substring(0, maxLength);
            }

            return text;
        }

        //
        // Summary:
        //     Returns an abstract of the provided text by returning up to Length characters
        //     of a text string. If the text is truncated a ... is appended. Note: Linebreaks
        //     are converted into spaces.
        //
        // Parameters:
        //   text:
        //     Text to abstract
        //
        //   length:
        //     Number of characters to abstract to
        //
        // Returns:
        //     string
        public static string TextAbstract(string text, int length)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (text.Length > length)
            {
                text = text.Substring(0, length);
                text = text.Substring(0, text.LastIndexOf(" ")) + "...";
            }

            if (!text.Contains("\n"))
            {
                return text;
            }

            StringBuilder stringBuilder = new StringBuilder(text.Length);
            string[] lines = text.GetLines();
            foreach (string text2 in lines)
            {
                stringBuilder.Append(text2.Trim() + " ");
            }

            return stringBuilder.ToString().Trim();
        }

        //
        // Summary:
        //     Terminates a string with the given end string/character, but only if the text
        //     specified doesn't already exist and the string is not empty.
        //
        // Parameters:
        //   value:
        //     String to terminate
        //
        //   terminator:
        //     String to terminate the text string with
        public static string TerminateString(string value, string terminator)
        {
            if (string.IsNullOrEmpty(value))
            {
                return terminator;
            }

            if (value.EndsWith(terminator))
            {
                return value;
            }

            return value + terminator;
        }

        //
        // Summary:
        //     Returns the number or right characters specified
        //
        // Parameters:
        //   full:
        //     full string to work with
        //
        //   rightCharCount:
        //     number of right characters to return
        public static string Right(string full, int rightCharCount)
        {
            if (string.IsNullOrEmpty(full) || full.Length < rightCharCount || full.Length - rightCharCount < 0)
            {
                return full;
            }

            return full.Substring(full.Length - rightCharCount);
        }

        //
        // Summary:
        //     Determines if a string is contained in a list of other strings
        //
        // Parameters:
        //   s:
        //
        //   list:
        public static bool Inlist(string s, params string[] list)
        {
            return list.Contains(s);
        }

        //
        // Summary:
        //     String.Contains() extension method that allows to specify case
        //
        // Parameters:
        //   text:
        //     Input text
        //
        //   searchFor:
        //     text to search for
        //
        //   stringComparison:
        //     Case sensitivity options
        public static bool Contains(this string text, string searchFor, StringComparison stringComparison)
        {
            return text.IndexOf(searchFor, stringComparison) > -1;
        }

        //
        // Summary:
        //     Parses a string into an array of lines broken by \r\n or \n
        //
        // Parameters:
        //   s:
        //     String to check for lines
        //
        //   maxLines:
        //     Optional - max number of lines to return
        //
        // Returns:
        //     array of strings, or null if the string passed was a null
        public static string[] GetLines(this string s, int maxLines = 0)
        {
            if (s == null)
            {
                return new string[0];
            }

            s = s.Replace("\r\n", "\n");
            if (maxLines < 1)
            {
                return s.Split(new char[1] { '\n' });
            }

            return s.Split(new char[1] { '\n' }).Take(maxLines).ToArray();
        }

        //
        // Summary:
        //     Returns a line count for a string
        //
        // Parameters:
        //   s:
        //     string to count lines for
        public static int CountLines(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }

            return s.Split(new char[1] { '\n' }).Length;
        }

        //
        // Summary:
        //     Returns a string that has the max amount of characters.
        //
        // Parameters:
        //   s:
        //     string to work on
        //
        //   maxCharacters:
        //     Maximum number of characters
        //
        //   startPosition:
        //     Optional start position. If not specified uses entire string (0)
        public static string GetMaxCharacters(this string s, int maxCharacters, int startPosition = 0)
        {
            if (string.IsNullOrEmpty(s) || (startPosition == 0 && maxCharacters < s.Length))
            {
                return s;
            }

            if (startPosition > s.Length - 1)
            {
                return null;
            }

            int val = s.Length - startPosition;
            return s.Substring(startPosition, Math.Min(val, maxCharacters));
        }

        //
        // Summary:
        //     Strips all non digit values from a string and only returns the numeric string.
        //
        // Parameters:
        //   input:
        public static string StripNonNumber(string input)
        {
            char[] array = input.ToCharArray();
            StringBuilder stringBuilder = new StringBuilder();
            char[] array2 = array;
            foreach (char c in array2)
            {
                if (char.IsNumber(c) || char.IsSeparator(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        //
        // Summary:
        //     Checks to see if value is part of a delimited list of values. Example: IsStringInList("value1,value2,value3","value3");
        //
        // Parameters:
        //   stringList:
        //     A list of delimited strings (ie. value1, value2, value3) with or without spaces
        //     (values are trimmed)
        //
        //   valueToFind:
        //     value to match against the list
        //
        //   separator:
        //     Character that separates the list values
        //
        //   ignoreCase:
        //     If true ignores case for the list value matches
        public static bool IsStringInList(string stringList, string valueToFind, char separator = ',', bool ignoreCase = false)
        {
            string[] array = stringList.Split(new char[1] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length == 0)
            {
                return false;
            }

            StringComparison comparisonType = (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.InvariantCulture);
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                if (array2[i].Trim().Equals(valueToFind, comparisonType))
                {
                    return true;
                }
            }

            return false;
        }

        //
        // Summary:
        //     Tokenizes a string based on a start and end string. Replaces the values with
        //     a token text (#@#1#@# for example). You can use Detokenize to get the original
        //     values back
        //
        // Parameters:
        //   text:
        //
        //   start:
        //
        //   end:
        //
        //   replaceDelimiter:
        public static List<string> TokenizeString(ref string text, string start, string end, string replaceDelimiter = "#@#")
        {
            List<string> list = new List<string>();
            MatchCollection matchCollection = tokenizeRegex.Matches(text);
            int num = 0;
            foreach (Match item in matchCollection)
            {
                tokenizeRegex = new Regex(Regex.Escape(item.Value));
                text = tokenizeRegex.Replace(text, $"{replaceDelimiter}{num}{replaceDelimiter}", 1);
                list.Add(item.Value);
                num++;
            }

            return list;
        }

        //
        // Summary:
        //     Detokenizes a string tokenized with TokenizeString. Requires the collection created
        //     by detokenization
        //
        // Parameters:
        //   text:
        //
        //   tokens:
        //
        //   replaceDelimiter:
        public static string DetokenizeString(string text, List<string> tokens, string replaceDelimiter = "#@#")
        {
            int num = 0;
            foreach (string token in tokens)
            {
                text = text.Replace($"{replaceDelimiter}{num}{replaceDelimiter}", token);
                num++;
            }

            return text;
        }

        //
        // Summary:
        //     Parses an string into an integer. If the text can't be parsed a default text
        //     is returned instead
        //
        // Parameters:
        //   input:
        //     Input numeric string to be parsed
        //
        //   defaultValue:
        //     Optional default text if parsing fails
        //
        //   formatProvider:
        //     Optional NumberFormat provider. Defaults to current culture's number format
        public static int ParseInt(string input, int defaultValue = 0, IFormatProvider numberFormat = null)
        {
            if (numberFormat == null)
            {
                numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            }

            int result = defaultValue;
            if (input == null)
            {
                return defaultValue;
            }

            if (!int.TryParse(input, NumberStyles.Any, numberFormat, out result))
            {
                return defaultValue;
            }

            return result;
        }

        //
        // Summary:
        //     Parses an string into an decimal. If the text can't be parsed a default text
        //     is returned instead
        //
        // Parameters:
        //   input:
        //
        //   defaultValue:
        public static decimal ParseDecimal(string input, decimal defaultValue = 0m, IFormatProvider numberFormat = null)
        {
            numberFormat = numberFormat ?? CultureInfo.CurrentCulture.NumberFormat;
            decimal result = defaultValue;
            if (input == null)
            {
                return defaultValue;
            }

            if (!decimal.TryParse(input, NumberStyles.Any, numberFormat, out result))
            {
                return defaultValue;
            }

            return result;
        }

        //
        // Summary:
        //     Creates short string id based on a GUID hashcode. Not guaranteed to be unique
        //     across machines, but unlikely to duplicate in medium volume situations.
        public static string NewStringId()
        {
            return Guid.NewGuid().ToString().GetHashCode()
                .ToString("x");
        }

        //
        // Summary:
        //     Creates a new random string of upper, lower case letters and digits. Very useful
        //     for generating random data for storage in test data.
        //
        // Parameters:
        //   size:
        //     The number of characters of the string to generate
        //
        // Returns:
        //     randomized string
        public static string RandomString(int size, bool includeNumbers = false)
        {
            StringBuilder stringBuilder = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                int num = ((!includeNumbers) ? Convert.ToInt32(Math.Floor(52.0 * random.NextDouble())) : Convert.ToInt32(Math.Floor(62.0 * random.NextDouble())));
                char value = ((num >= 26) ? ((num <= 25 || num >= 52) ? Convert.ToChar(num - 52 + 48) : Convert.ToChar(num - 26 + 97)) : Convert.ToChar(num + 65));
                stringBuilder.Append(value);
            }

            return stringBuilder.ToString();
        }

        //
        // Summary:
        //     UrlEncodes a string without the requirement for System.Web
        //
        // Parameters:
        //   String:
        public static string UrlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return Uri.EscapeDataString(text);
        }

        //
        // Summary:
        //     Encodes a few additional characters for use in paths Encodes: . #
        //
        // Parameters:
        //   text:
        public static string UrlEncodePathSafe(string text)
        {
            return UrlEncode(text).Replace(".", "%2E").Replace("#", "%23");
        }

        //
        // Summary:
        //     UrlDecodes a string without requiring System.Web
        //
        // Parameters:
        //   text:
        //     String to decode.
        //
        // Returns:
        //     decoded string
        public static string UrlDecode(string text)
        {
            text = text.Replace("+", " ");
            return Uri.UnescapeDataString(text);
        }

        //
        // Summary:
        //     Retrieves a text by key from a UrlEncoded string.
        //
        // Parameters:
        //   urlEncoded:
        //     UrlEncoded String
        //
        //   key:
        //     Key to retrieve text for
        //
        // Returns:
        //     returns the text or "" if the key is not found or the text is blank
        public static string GetUrlEncodedKey(string urlEncoded, string key)
        {
            urlEncoded = "&" + urlEncoded + "&";
            int num = urlEncoded.IndexOf("&" + key + "=", StringComparison.OrdinalIgnoreCase);
            if (num < 0)
            {
                return string.Empty;
            }

            int num2 = num + 2 + key.Length;
            int num3 = urlEncoded.IndexOf("&", num2);
            if (num3 < 0)
            {
                return string.Empty;
            }

            return UrlDecode(urlEncoded.Substring(num2, num3 - num2));
        }

        //
        // Summary:
        //     Allows setting of a text in a UrlEncoded string. If the key doesn't exist a new
        //     one is set, if it exists it's replaced with the new text.
        //
        // Parameters:
        //   urlEncoded:
        //     A UrlEncoded string of key text pairs
        //
        //   key:
        //
        //   value:
        public static string SetUrlEncodedKey(string urlEncoded, string key, string value)
        {
            if (!urlEncoded.EndsWith("?") && !urlEncoded.EndsWith("&"))
            {
                urlEncoded += "&";
            }

            Match match = Regex.Match(urlEncoded, "[?|&]" + key + "=.*?&");
            urlEncoded = ((match != null && !string.IsNullOrEmpty(match.Value)) ? urlEncoded.Replace(match.Value, match.Value.Substring(0, 1) + key + "=" + UrlEncode(value) + "&") : (urlEncoded + key + "=" + UrlEncode(value) + "&"));
            return urlEncoded.TrimEnd(new char[1] { '&' });
        }

        //
        // Summary:
        //     Turns a BinHex string that contains raw byte values into a byte array
        //
        // Parameters:
        //   hex:
        //     BinHex string (just two byte hex digits strung together)
        //public static byte[] BinHexToBinary(string hex)
        //{
        //    int num = (hex.StartsWith("0x") ? 2 : 0);
        //    if (hex.Length % 2 != 0)
        //    {
        //        throw new ArgumentException(string.Format(Resources.InvalidHexStringLength, hex.Length));
        //    }

        //    byte[] array = new byte[(hex.Length - num) / 2];
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        array[i] = (byte)((ParseHexChar(hex[num]) << 4) | ParseHexChar(hex[num + 1]));
        //        num += 2;
        //    }

        //    return array;
        //}

        //
        // Summary:
        //     Converts a byte array into a BinHex string. BinHex is two digit hex byte values
        //     squished together into a string.
        //
        // Parameters:
        //   data:
        //     Raw data to send
        //
        // Returns:
        //     BinHex string or null if input is null
        public static string BinaryToBinHex(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }

            return stringBuilder.ToString();
        }

        //
        // Summary:
        //     Converts a string into bytes for storage in any byte[] types buffer or stream
        //     format (like MemoryStream).
        //
        // Parameters:
        //   text:
        //
        //   encoding:
        //     The character encoding to use. Defaults to Unicode
        public static byte[] StringToBytes(string text, Encoding encoding = null)
        {
            if (text == null)
            {
                return null;
            }

            if (encoding == null)
            {
                encoding = Encoding.Unicode;
            }

            return encoding.GetBytes(text);
        }

        //
        // Summary:
        //     Converts a byte array to a stringUtils
        //
        // Parameters:
        //   buffer:
        //     raw string byte data
        //
        //   encoding:
        //     Character encoding to use. Defaults to Unicode
        public static string BytesToString(byte[] buffer, Encoding encoding = null)
        {
            if (buffer == null)
            {
                return null;
            }

            if (encoding == null)
            {
                encoding = Encoding.Unicode;
            }

            return encoding.GetString(buffer);
        }

        //private static int ParseHexChar(char c)
        //{
        //    if (c >= '0' && c <= '9')
        //    {
        //        return c - 48;
        //    }

        //    if (c >= 'A' && c <= 'F')
        //    {
        //        return c - 65 + 10;
        //    }

        //    if (c >= 'a' && c <= 'f')
        //    {
        //        return c - 97 + 10;
        //    }

        //    throw new ArgumentException(Resources.InvalidHexDigit + c);
        //}

        //
        // Summary:
        //     Encodes an integer into a string by mapping to alpha and digits (36 chars) chars
        //     are embedded as lower case Example: 4zx12ss
        //
        // Parameters:
        //   value:
        public static string Base36Encode(long value)
        {
            string text = "";
            bool flag = value < 0;
            if (flag)
            {
                value *= -1;
            }

            do
            {
                text = base36CharArray[value % base36CharArray.Length] + text;
                value /= 36;
            }
            while (value != 0L);
            if (!flag)
            {
                return text;
            }

            return text + "-";
        }

        //
        // Summary:
        //     Decodes a base36 encoded string to an integer
        //
        // Parameters:
        //   input:
        public static long Base36Decode(string input)
        {
            bool flag = false;
            if (input.EndsWith("-"))
            {
                flag = true;
                input = input.Substring(0, input.Length - 1);
            }

            char[] array = input.ToCharArray();
            Array.Reverse((Array)array);
            long num = 0L;
            for (long num2 = 0L; num2 < array.Length; num2++)
            {
                long num3 = base36Chars.IndexOf(array[num2]);
                num += Convert.ToInt64((double)num3 * Math.Pow(36.0, num2));
            }

            if (!flag)
            {
                return num;
            }

            return num * -1;
        }

        //
        // Summary:
        //     Normalizes linefeeds to the appropriate
        //
        // Parameters:
        //   text:
        //     The text to fix up
        //
        //   type:
        //     Type of linefeed to fix up to
        //public static string NormalizeLineFeeds(string text, LineFeedTypes type = LineFeedTypes.Auto)
        //{
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        return text;
        //    }

        //    if (type == LineFeedTypes.Auto)
        //    {
        //        type = (Enumerable.Contains(Environment.NewLine, '\r') ? LineFeedTypes.CrLf : LineFeedTypes.Lf);
        //    }

        //    if (type == LineFeedTypes.Lf)
        //    {
        //        return text.Replace("\r\n", "\n");
        //    }

        //    return text.Replace("\r\n", "*@\r@*").Replace("\n", "\r\n").Replace("*@\r@*", "\r\n");
        //}

        //
        // Summary:
        //     Strips any common white space from all lines of text that have the same common
        //     white space text. Effectively removes common code indentation from code blocks
        //     for example so you can get a left aligned code snippet.
        //
        // Parameters:
        //   code:
        //     Text to normalize
        public static string NormalizeIndentation(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return string.Empty;
            }

            string[] array = code.Replace("\t", "   ").Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int num = 1000;
            string[] array2 = array;
            foreach (string text in array2)
            {
                if (text.Length == 0)
                {
                    continue;
                }

                int num2 = 0;
                string text2 = text;
                for (int j = 0; j < text2.Length && text2[j] == ' '; j++)
                {
                    if (num2 >= num)
                    {
                        break;
                    }

                    num2++;
                }

                if (num2 == 0)
                {
                    return code;
                }

                num = num2;
            }

            string findString = new string(' ', num);
            StringBuilder stringBuilder = new StringBuilder();
            array2 = array;
            foreach (string origString in array2)
            {
                stringBuilder.AppendLine(ReplaceStringInstance(origString, findString, "", 1, caseInsensitive: false));
            }

            return stringBuilder.ToString();
        }

        //
        // Summary:
        //     Simple Logging method that allows quickly writing a string to a file
        //
        // Parameters:
        //   output:
        //
        //   filename:
        //
        //   encoding:
        //     if not specified used UTF-8
        public static void LogString(string output, string filename, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            lock (_logLock)
            {
                StreamWriter streamWriter = new StreamWriter(filename, append: true, encoding);
                streamWriter.WriteLine(DateTime.Now.ToString() + " - " + output);
                streamWriter.Close();
            }
        }

        //
        // Summary:
        //     Creates a Stream from a string. Internally creates a memory stream and returns
        //     that.
        //
        // Parameters:
        //   text:
        //
        //   encoding:
        public static Stream StringToStream(string text, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }

            MemoryStream memoryStream = new MemoryStream(text.Length * 2);
            byte[] bytes = encoding.GetBytes(text);
            memoryStream.Write(bytes, 0, bytes.Length);
            memoryStream.Position = 0L;
            return memoryStream;
        }

        //
        // Summary:
        //     Retrieves a text from an XML-like string
        //
        // Parameters:
        //   propertyString:
        //
        //   key:
        public static string GetProperty(string propertyString, string key)
        {
            return propertyString.ExtractString("<" + key + ">", "</" + key + ">");
        }

        //
        // Parameters:
        //   propertyString:
        //
        //   key:
        //
        //   value:
        public static string SetProperty(string propertyString, string key, string value)
        {
            string text = propertyString.ExtractString("<" + key + ">", "</" + key + ">");
            if (string.IsNullOrEmpty(value) && text != string.Empty)
            {
                return propertyString.Replace(text, "");
            }

            string text2 = "<" + key + ">" + value + "</" + key + ">";
            if (text != string.Empty)
            {
                return propertyString.Replace(text, text2);
            }

            return propertyString + text2 + "\r\n";
        }
    }
}
