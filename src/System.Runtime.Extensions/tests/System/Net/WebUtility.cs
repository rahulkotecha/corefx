// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Net.Tests
{
    public class WebUtilityTests
    {
        [Fact]
        public static void HtmlDecodeWithoutTextWriter()
        {
            string input = "Hello! &apos;&quot;&lt;&amp;&gt;\u2665&hearts;\u00E7&#xe7;&#231;";
            string expected = "Hello! '\"<&>\u2665\u2665\u00E7\u00E7\u00E7";

            string returned = WebUtility.HtmlDecode(input);

            Assert.Equal(expected, returned);
        }

        [Fact]
        public static void HtmlDecodeWithoutTextWriterReturnsNullIfInputIsNull()
        {
            string returned = WebUtility.HtmlDecode(null);
            Assert.Null(returned);
        }

        [Fact]
        public static void HtmlDecodeNoTextWriterOriginalStringNoSpecialCharacters()
        {
            string input = "Hello, world! \"<>\u2665\u00E7";

            string returned = WebUtility.HtmlDecode(input);

            Assert.Equal(input, returned);
        }

        [Fact]
        public static void HtmlEncodeSingleQuote()
        {
            // Single quotes need to be encoded as &#39; rather than &apos; since &#39; is valid both for
            // HTML and XHTML, but &apos; is valid only for XHTML.
            // For more info: http://fishbowl.pastiche.org/2003/07/01/the_curse_of_apos/

            string input = "'";
            string expected = "&#39;";

            string returned = WebUtility.HtmlEncode(input);

            Assert.Equal(expected, returned);
        }

        [Fact]
        public static void HtmlEncodeWithoutTextWriter()
        {
            string input = "Hello! '\"<&>\u2665\u00E7 World";
            string expected = "Hello! &#39;&quot;&lt;&amp;&gt;\u2665&#231; World";

            string returned = WebUtility.HtmlEncode(input);

            Assert.Equal(expected, returned);
        }

        [Fact]
        public static void HtmlEncodeWithoutTextWriterReturnsNullIfInputIsNull()
        {
            string returned = WebUtility.HtmlEncode((string)null);

            Assert.Null(returned);
        }

        [Fact]
        public static void HtmlEncodeNoTextWriterOriginalStringNoSpecialCharacters()
        {
            string input = "Hello, world!";

            string returned = WebUtility.HtmlEncode(input);

            Assert.Equal(input, returned);
        }

        [Fact]
        public static void UrlDecodeFromStringNoEncodingReturnsNullIfInputIsNull()
        {
            string returned = WebUtility.UrlDecode((string)null);

            Assert.Null(returned);
        }

        [Fact]
        public static void UrlEncodeFromStringNoEncoding()
        {
            // Recent change brings function inline with RFC 3986 to return hex-encoded chars in uppercase
            string input = "/\\\"\tHello! \u2665?/\\\"\tWorld! \u2665?\u2665";
            string expected = "%2F%5C%22%09Hello!+%E2%99%A5%3F%2F%5C%22%09World!+%E2%99%A5%3F%E2%99%A5";

            string returned = WebUtility.UrlEncode(input);

            Assert.Equal(expected, returned);
        }

        [Fact]
        public static void UrlEncodeFromStringNoEncodingReturnsNullIfInputIsNull()
        {
            string returned = WebUtility.UrlEncode((string)null);
            Assert.Null(returned);
        }

        [Fact]
        public static void UrlEncodeToBytes_ArgumentValidation()
        {
            Assert.Null(WebUtility.UrlEncodeToBytes(null, 0, 0));
            Assert.Throws<ArgumentNullException>("bytes", () => WebUtility.UrlEncodeToBytes(null, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => WebUtility.UrlEncodeToBytes(new byte[1], -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => WebUtility.UrlEncodeToBytes(new byte[1], 2, 1));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => WebUtility.UrlEncodeToBytes(new byte[1], 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => WebUtility.UrlEncodeToBytes(new byte[1], 0, 3));
        }

        [Fact]
        public static void UrlDecodeToBytes_ArgumentValidation()
        {
            Assert.Null(WebUtility.UrlDecodeToBytes(null, 0, 0));
            Assert.Throws<ArgumentNullException>("bytes", () => WebUtility.UrlDecodeToBytes(null, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => WebUtility.UrlDecodeToBytes(new byte[1], -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => WebUtility.UrlDecodeToBytes(new byte[1], 2, 1));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => WebUtility.UrlDecodeToBytes(new byte[1], 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => WebUtility.UrlDecodeToBytes(new byte[1], 0, 3));
        }

        [Fact]
        public static void UrlDecodeToBytesNullIfInputIsNull()
        {
            Assert.Null(WebUtility.UrlDecodeToBytes(null, 0, 0));
        }

        [Fact]
        public static void UrlEncodeSingleQuote()
        {
            Assert.Equal("%27", WebUtility.UrlEncode("'"));
        }

        [Fact]
        public static void HtmlDefaultStrictSettingEncode()
        {
            Assert.Equal(WebUtility.HtmlEncode(Char.ConvertFromUtf32(144308)), "&#144308;");
        }

        [Fact]
        public static void HtmlDefaultStrictSettingDecode()
        {
            Assert.Equal(Char.ConvertFromUtf32(144308), WebUtility.HtmlDecode("&#144308;"));
        }

        [Theory]
        [InlineData("'")]
        [InlineData("http://www.microsoft.com")]
        [InlineData("/\\\"\tHello! \u2665?/\\\"\tWorld! \u2665?\u2665")]
        public static void UrlEncodeDecode_Roundtrip(string url)
        {
            string encoded = WebUtility.UrlEncode(url);
            Assert.Equal(url, WebUtility.UrlDecode(encoded));
        }

        [Theory]
        [InlineData("'")]
        [InlineData("http://www.microsoft.com")]
        [InlineData("/\\\"\tHello! \u2665?/\\\"\tWorld! \u2665?\u2665")]
        public static void UrlEncodeDecodeToBytes_Roundtrip(string url)
        {
            byte[] input = System.Text.Encoding.UTF8.GetBytes(url);
            byte[] encoded = WebUtility.UrlEncodeToBytes(input, 0, input.Length);
            Assert.Equal(input, WebUtility.UrlDecodeToBytes(encoded, 0, encoded.Length));
        }
    }
}
