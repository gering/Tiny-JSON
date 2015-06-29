using NUnit.Framework;
using System.Collections.Generic;

namespace Tiny {

	[TestFixture]
	public static class ParserTest {

		[Test]
		public static void ParseString1() {
			string str = JsonParser.ParseValue("\"test\"") as string;
			Assert.AreEqual("test", str);
		}

		[Test]
		public static void ParseString2() {
			string str = JsonParser.ParseValue(" \" \\\"Test\\\" \" ") as string;
			Assert.AreEqual(" \"Test\" ", str);
		}

		[Test]
		public static void ParseString3() {
			string str = JsonParser.ParseValue("\"Test") as string;
			Assert.IsNull(str);
		}

		[Test]
		public static void ParseString4() {
			string str = JsonParser.ParseValue("Test\"") as string;
			Assert.IsNull(str);
		}

		[Test]
		public static void ParseArray1() {
			List<object> array = JsonParser.ParseValue(@" [0, 1, 2.2, true, false, null, ""test""] ") as List<object>;
			Assert.IsNotNull(array);

			Assert.AreEqual(0, array[0]);
			Assert.AreEqual(1, array[1]);
			Assert.AreEqual(2.2, array[2]);
			Assert.AreEqual(true, array[3]);
			Assert.AreEqual(false, array[4]);
			Assert.AreEqual(null, array[5]);
			Assert.AreEqual("test", array[6]);
		}

		[Test]
		public static void ParseArray2() {
			List<object> array = JsonParser.ParseValue(@"[[0, 1], [true, false], [""a"", ""b"", ""c""]]") as List<object>;
			Assert.IsNotNull(array);

			List<object> sub1 = array[0] as List<object>;
			Assert.IsNotNull(sub1);
			Assert.AreEqual(0, sub1[0]);
			Assert.AreEqual(1, sub1[1]);

			List<object> sub2 = array[1] as List<object>;
			Assert.IsNotNull(sub2);
			Assert.AreEqual(true, sub2[0]);
			Assert.AreEqual(false, sub2[1]);

			List<object> sub3 = array[2] as List<object>;
			Assert.IsNotNull(sub2);
			Assert.AreEqual("a", sub3[0]);
			Assert.AreEqual("b", sub3[1]);
			Assert.AreEqual("c", sub3[2]);
		}

		[Test]
		public static void ParseObject1() {
			Dictionary<string, object> obj = JsonParser.ParseValue(@"{""a"":1, ""b"":null, ""c"":0.3}") as Dictionary<string, object>;
			Assert.IsNotNull(obj);
			
			Assert.AreEqual(1, obj["a"]);
			Assert.AreEqual(null, obj["b"]);
			Assert.AreEqual(0.3, obj["c"]);
		}

		[Test]
		public static void ParseObject2() {
			Dictionary<string, object> obj = JsonParser.ParseValue(@"{""a"" : {}, ""b"" : null, ""c"":[]}") as Dictionary<string, object>;
			Assert.IsNotNull(obj);
			
			Assert.AreEqual(new Dictionary<string, object>(), obj["a"]);
			Assert.AreEqual(null, obj["b"]);
			Assert.AreEqual(new List<object>(), obj["c"]);
		}

		[Test]
		public static void ParseUTF8String1() {
			string str = JsonParser.ParseValue("\"\\u20AC \\u00F6 \\u00DC \\u00E9\"") as string;
			Assert.AreEqual("€ ö Ü é", str);
		}
	}
}