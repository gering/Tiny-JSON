using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Tiny {

	[TestFixture]
	public static class BuilderTest {

		[Test]
		public static void AppendArray1() {
			JsonBuilder builder = new JsonBuilder();
			builder.AppendArray(new []{0, 1, 2, 3});
			Assert.AreEqual("[0,1,2,3]", builder.ToString());
		}

		[Test]
		public static void AppendDict1() {
			JsonBuilder builder = new JsonBuilder();
			IDictionary dict = new Hashtable();
			dict.Add("a", 0);
			dict.Add("b", "Test");
			builder.AppendDictionary(dict);
			string json = builder.ToString();
            Assert.True(json.StartsWith("{", System.StringComparison.InvariantCultureIgnoreCase));
            Assert.True(json.EndsWith("}", System.StringComparison.InvariantCultureIgnoreCase));
			AssertHelper.Contains(@"""a"":0", json);
			AssertHelper.Contains(@"""b"":""Test""", json);
		}

		[Test]
		public static void AppendDict2() {
			JsonBuilder builder = new JsonBuilder();
            Dictionary<string, object> dict = new Dictionary<string, object>();
			dict.Add("a", 0);
			dict.Add("b", true);
			builder.AppendDictionary(dict);
			string json = builder.ToString();
            Assert.True(json.StartsWith("{", System.StringComparison.InvariantCultureIgnoreCase));
            Assert.True(json.EndsWith("}", System.StringComparison.InvariantCultureIgnoreCase));
			AssertHelper.Contains(@"""a"":0", json);
			AssertHelper.Contains(@"""b"":true", json);
		}

		[Test]
		public static void AppendUTF8String1() {
			JsonBuilder builder = new JsonBuilder();
			builder.AppendString("€ ö Ü é");
			Assert.AreEqual("\"\\u20ac \\u00f6 \\u00dc \\u00e9\"", builder.ToString());
		}
	}
}

