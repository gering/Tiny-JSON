using NUnit.Framework;
using System.Collections;

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
			Assert.AreEqual("{\"a\":0,\"b\":\"Test\"}", builder.ToString());
		}

		[Test]
		public static void AppendUTF8String1() {
			JsonBuilder builder = new JsonBuilder();
			builder.AppendString("€ ö Ü é");
			Assert.AreEqual("\"\\u20ac \\u00f6 \\u00dc \\u00e9\"", builder.ToString());
		}
	}
}

