﻿//#define ENABLE_MULTI_TEST

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Web;

namespace RestSharp.Portable.Test
{
    [TestClass]
    public class UrlEncodeVariant2Tests
    {
        private readonly UrlEscapeUtility _utility = new UrlEscapeUtility(false);

        [TestMethod]
        public void TestEscapeDataStringComplianceASCII()
        {
            const UrlEscapeFlags flags = UrlEscapeFlags.BuilderVariantListByteArray;
            var chars = new string(Enumerable.Range(32, 95).Select(x => (char)x).ToArray());
            var expected = Uri.EscapeDataString(chars);
            var test = _utility.Escape(chars, flags);
            Assert.AreEqual(expected, test);
            Assert.AreEqual(expected.Length, _utility.ComputeLength(chars, flags));
        }

        [TestMethod]
        public void TestEscapeDataStringComplianceUmlaut()
        {
            const UrlEscapeFlags flags = UrlEscapeFlags.BuilderVariantListByteArray;
            const string chars = "äöüßÄÖÜ\u007F";
            var expected = Uri.EscapeDataString(chars);
            var test = _utility.Escape(chars, flags);
            Assert.AreEqual(expected, test);
            Assert.AreEqual(expected.Length, _utility.ComputeLength(chars, flags));
        }

        [TestMethod]
        public void TestUrlEncodeComplianceASCII()
        {
            const UrlEscapeFlags flags = UrlEscapeFlags.LikeUrlEncode | UrlEscapeFlags.BuilderVariantListByteArray;
            var chars = new string(Enumerable.Range(32, 95).Select(x => (char)x).ToArray());
            var expected = HttpUtility.UrlEncode(chars);
            Assert.IsNotNull(expected);
            var test = _utility.Escape(chars, flags);
            Assert.AreEqual(expected, test);
            Assert.AreEqual(expected.Length, _utility.ComputeLength(chars, flags));
        }

        [TestMethod]
        public void TestUrlEncodeComplianceUmlaut()
        {
            const UrlEscapeFlags flags = UrlEscapeFlags.LikeUrlEncode | UrlEscapeFlags.BuilderVariantListByteArray;
            const string chars = "äöüßÄÖÜ\u007F";
            var expected = HttpUtility.UrlEncode(chars);
            Assert.IsNotNull(expected);
            var test = _utility.Escape(chars, flags);
            Assert.AreEqual(expected, test);
            Assert.AreEqual(expected.Length, _utility.ComputeLength(chars, flags));
        }

#if ENABLE_MULTI_TEST
        [TestMethod]
        public void TestEscapeDataStringComplianceASCII100000()
        {
            for (int i = 0; i != 100000; ++i)
                TestEscapeDataStringComplianceASCII();
        }

        [TestMethod]
        public void TestUrlEncodeComplianceASCII100000()
        {
            for (int i = 0; i != 100000; ++i)
                TestUrlEncodeComplianceASCII();
        }
#endif
    }
}
