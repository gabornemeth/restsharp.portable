﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RestSharp.Portable
{
    /// <summary>
    /// URL utility functions
    /// </summary>
    internal static class UrlUtility
    {
        private static readonly Encoding s_defaultEncoding = new UTF8Encoding(false);

        private static readonly byte[] s_reserved = Encoding.UTF8.GetBytes(";/?:@&=+$,");
        private static readonly byte[] s_lowalpha = Encoding.UTF8.GetBytes("abcdefghijklmnopqrstuvwxyz");
        private static readonly byte[] s_upalpha = Encoding.UTF8.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        private static readonly byte[] s_alpha = s_lowalpha.Union(s_upalpha).ToArray();
        private static readonly byte[] s_digit = Encoding.UTF8.GetBytes("0123456789");
        private static readonly byte[] s_alphanum = s_alpha.Union(s_digit).ToArray();
        private static readonly byte[] s_mark = Encoding.UTF8.GetBytes("-_.!~*'()");
        private static readonly byte[] s_unreserved = s_alphanum.Union(s_mark).ToArray();
        private static readonly byte[] s_control = Encoding.UTF8.GetBytes("\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u0009\u000a\u000b\u000c\u000d\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f");
        private static readonly byte[] s_space = Encoding.UTF8.GetBytes(" ");
        private static readonly byte[] s_delims = Encoding.UTF8.GetBytes("<>#%\"");
        private static readonly byte[] s_unwise = Encoding.UTF8.GetBytes("{}|\\^[]`");

        private static readonly byte[] s_hexCharsLow = Encoding.UTF8.GetBytes("0123456789abcdef");
        private static readonly byte[] s_hexCharsUp = Encoding.UTF8.GetBytes("0123456789ABCDEF");

        private static readonly Dictionary<UrlEscapeFlags, ISet<byte>> s_allowedBytes = new Dictionary<UrlEscapeFlags, ISet<byte>>
        {
            { UrlEscapeFlags.AllowLikeEscapeDataString, new HashSet<byte>(s_alphanum.Union(Encoding.UTF8.GetBytes("-_.~"))) },
            { UrlEscapeFlags.AllowAllUnreserved, new HashSet<byte>(s_unreserved) },
            { UrlEscapeFlags.AllowLikeUrlEncode, new HashSet<byte>(s_alphanum.Union(Encoding.UTF8.GetBytes("-_.!*()"))) },
        };

        private static string ConvertEscapedBytesToString(byte[] data)
        {
            return s_defaultEncoding.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// URL escape
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Escape(string data)
        {
            return Escape(data, UrlEscapeFlags.Default);
        }

        /// <summary>
        /// URL escape
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Escape(string data, Encoding encoding)
        {
            return Escape(data, encoding, UrlEscapeFlags.Default);
        }

        /// <summary>
        /// URL escape for bytes
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Escape(byte[] data)
        {
            return Escape(data, UrlEscapeFlags.Default);
        }

        /// <summary>
        /// URL escape
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EscapeToBytes(string data)
        {
            return EscapeToBytes(data, UrlEscapeFlags.Default);
        }

        /// <summary>
        /// URL escape for bytes
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] EscapeToBytes(string data, Encoding encoding)
        {
            return EscapeToBytes(data, encoding, UrlEscapeFlags.Default);
        }

        /// <summary>
        /// URL escape for bytes
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EscapeToBytes(byte[] data)
        {
            return EscapeToBytes(data, UrlEscapeFlags.Default);
        }

        /// <summary>
        /// URL escape
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static string Escape(string data, UrlEscapeFlags flags)
        {
            return ConvertEscapedBytesToString(EscapeToBytes(data, flags));
        }

        /// <summary>
        /// URL escape
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static string Escape(string data, Encoding encoding, UrlEscapeFlags flags)
        {
            return ConvertEscapedBytesToString(EscapeToBytes(data, encoding, flags));
        }

        /// <summary>
        /// URL escape for bytes
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static string Escape(byte[] data, UrlEscapeFlags flags)
        {
            return ConvertEscapedBytesToString(EscapeToBytes(data, flags));
        }

        /// <summary>
        /// URL escape
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static byte[] EscapeToBytes(string data, UrlEscapeFlags flags)
        {
            return EscapeToBytes(data, s_defaultEncoding, flags);
        }

        /// <summary>
        /// URL escape
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static byte[] EscapeToBytes(string data, Encoding encoding, UrlEscapeFlags flags)
        {
            return EscapeToBytes(encoding.GetBytes(data), flags);
        }

        /// <summary>
        /// URL escape for bytes
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static byte[] EscapeToBytes(byte[] data, UrlEscapeFlags flags)
        {
            var hexChars = ((flags & UrlEscapeFlags.LowerCaseHexCharacters) != UrlEscapeFlags.Default) ? s_hexCharsLow : s_hexCharsUp;

            var encodedByte = new byte[3];
            encodedByte[0] = (byte)'%';

            var allowedBytesIndex = (flags & UrlEscapeFlags.AllowMask);
            ISet<byte> allowedBytes;
            if (!s_allowedBytes.TryGetValue(allowedBytesIndex, out allowedBytes))
                allowedBytes = s_allowedBytes[UrlEscapeFlags.Default];

            var output = new List<byte>(data.Length);
            var len = data.Length;
            var escapeByte = false;
            for (var i = 0; i != len; ++i)
            {
                var v = data[i];
                if (allowedBytes.Contains(v))
                {
                    output.Add(v);
                }
                else if (v == 0x20)
                {
                    if ((flags & UrlEscapeFlags.EscapeSpaceAsPlus) != UrlEscapeFlags.Default)
                    {
                        output.Add((byte)'+');
                    }
                    else
                    {
                        escapeByte = true;
                    }
                }
                else
                {
                    escapeByte = true;
                }
                if (escapeByte)
                {
                    escapeByte = false;
                    var hiByte = ((v >> 4) & 0x0F);
                    var loByte = (v & 0x0F);
                    encodedByte[1] = hexChars[hiByte];
                    encodedByte[2] = hexChars[loByte];
                    output.AddRange(encodedByte);
                }
            }
            return output.ToArray();
        }
    }
}