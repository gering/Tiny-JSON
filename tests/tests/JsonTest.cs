using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Tiny {

	[TestFixture]
	public static class JsonTest {

		enum Kind {
			Unknown, Reptile, Bird, Mammal
		}

		class Animal {
			public int legs;
			internal Kind kind {get; set; }
		}

		class Bear : Animal {
			public bool hungry {get; set; }
			float weight;
			[NonSerialized]
			public string id = "unknown";
			public string name = "Baloo";

			Bear() {}

			public Bear(float weight) {
				this.weight = weight;
				this.kind = Kind.Mammal;
			}
		}

		struct Mission {
			public string target;
			public DateTime start;
		}

		class Transporter<T> {
			Mission mission;
			public T[] cargo;
			public int? maxCargo;
			public List<string> driver { get; private set; }

			public Mission Mission {
				get { return mission; }
				set { mission = value; }
			}

			public Transporter() {
				driver = new List<string>();
			}
		}

		[Test]
		public static void EncodeTest1() {
			Animal a = new Animal();
			a.legs = 4;

			string json = Json.Encode(a);
			Assert.AreEqual("{\"legs\":4,\"kind\":0}", json);
		}

		[Test]
		public static void EncodeTest2() {
			Animal a = new Bear(10);
			a.legs = 4;

			string json = Json.Encode(a);
			Assert.AreEqual("{\"hungry\":false,\"weight\":10.0,\"name\":\"Baloo\",\"legs\":4,\"kind\":3}", json);
		}

		[Test]
		public static void EncodeTest3() {
			Animal a = new Animal();
			a.legs = 4;

			Animal b = new Animal();
			a.legs = 2;

			Mission m;
			m.target = "unknown";
			m.start = new DateTime(2020, 4, 1);

			Transporter<Animal> t = new Transporter<Animal>();
			t.Mission = m;
			t.cargo = new Animal[] { a, b };
			t.driver.Add("Joe");

			string json = Json.Encode(t);
			Console.WriteLine("json = " + json);
			Assert.AreEqual("{\"mission\":{\"target\":\"unknown\",\"start\":\"2020-03-31T22:00:00.000Z\"},\"cargo\":[{\"legs\":2,\"kind\":0},{\"legs\":0,\"kind\":0}],\"maxCargo\":null,\"driver\":[\"Joe\"]}", json);
		}

		[Test]
		public static void EncodeListTest1() {
			string[] list = {"a", "b", null, null, "e"};
			string json = Json.Encode(list);

			Assert.AreEqual("[\"a\",\"b\",null,null,\"e\"]", json);
		}

		[Test]
		public static void EncodeListTest2() {
			List<string> list = new List<string>(new string[]{"ä", "b", null, null, "e"});
			string json = Json.Encode(list);
			
			Assert.AreEqual("[\"\\u00e4\",\"b\",null,null,\"e\"]", json);
		}

		[Test]
		public static void EncodeListTest3() {
			bool?[] list = {true, false, null};
			string json = Json.Encode(list);
			
			Assert.AreEqual("[true,false,null]", json);
		}

		[Test]
		public static void EncodeListTest4() {
			bool[] list = {true, false};
			string json = Json.Encode(list);
			
			Assert.AreEqual("[true,false]", json);
		}

		[Test]
		public static void EncodeSetTest1() {
			var hashSet = new HashSet<int> { 1, 2, 3, 4 };
			string json = Json.Encode(hashSet);

			Assert.AreEqual("[1,2,3,4]", json);
		}

		[Test]
		public static void EncodeDictTest1() {
			var dict = new Dictionary<string, int> {
				{"three", 3},
				{"five", 5},
				{"ten", 10}
			};
			
			string json = Json.Encode(dict);
			
			Assert.AreEqual("{\"three\":3,\"five\":5,\"ten\":10}", json);
		}

		[Test]
		public static void EncodeDictTest2() {
			var dict = new Dictionary<int, string> {
				{3, "three"},
				{5, "five"},
				{10, "ten"}
			};
			string json = Json.Encode(dict);
			Assert.AreEqual("{\"3\":\"three\",\"5\":\"five\",\"10\":\"ten\"}", json);
		}

        [Test]
        public static void EncodeDictTest3() {
            var dict = new Dictionary<string, object> {
                {"a", 'a'},
                {"b", 1},
                {"c", true},
                {"d", null}
            };
            string json = Json.Encode(dict);
            Assert.AreEqual(@"{""a"":""a"",""b"":1,""c"":true,""d"":null}", json);
        }

		[Test]
		public static void PrettyEncodeTest1() {
			Animal a = new Bear(10.5f);
			a.legs = 4;
			
			string json = Json.Encode(a, true);
			Assert.AreEqual("{\n\t\"hungry\" : false,\n\t\"weight\" : 10.5,\n\t\"name\" : \"Baloo\",\n\t\"legs\" : 4,\n\t\"kind\" : 3\n}\n", json);
		}

		[Test]
		public static void PrettyEncodeTest2() {
			string[] array = {"ä", "\"", "", null};
			string json = Json.Encode(array, true);
			
			Assert.AreEqual("[\n\t\"ä\",\n\t\"\\\"\",\n\t\"\",\n\tnull\n]\n", json);
		}

		[Test]
		public static void DecodeTest0() {
			var a = Json.Decode<Animal>("");
			Assert.AreEqual(null, a);
		}

		[Test]
		public static void DecodeTest1() {
			var a = Json.Decode<Animal>("{\"legs\":4,\"a\":false}");
			Assert.AreEqual(4, a.legs);
		}

		[Test]
		public static void DecodeTest2() {
			var a = Json.Decode<Animal>("{\"legs\":null}");
			Assert.AreEqual(0, a.legs);
		}

		[Test]
		public static void DecodeTest3() {
			var a = Json.Decode<Bear>("{\"legs\":4,\"hungry\":true,\"kind\":3,\"name\":null,\"id\":\"a\"}");
			
			Assert.AreEqual(4, a.legs);
			Assert.AreEqual(true, a.hungry);
			Assert.AreEqual(Kind.Mammal, a.kind);
			Assert.AreEqual(null, a.name);
			Assert.AreEqual("unknown", a.id);
		}

        [Test]
        public static void DecodeTest4() {
            var a = Json.Decode<Bear>("{\"legs\":4,\"hungry\":true,\"kind\":\"Mammal\",\"name\":null}");

            Assert.AreEqual(4, a.legs);
            Assert.AreEqual(true, a.hungry);
            Assert.AreEqual(Kind.Mammal, a.kind);
            Assert.AreEqual(null, a.name);
        }

		[Test]
		public static void DecodeTest5() {
			var a = Json.Decode<Transporter<Animal>>("{\"mission\":{\"target\":\"secret\",\"start\":\"2020-03-31T22:00:00.000Z\"},\"cargo\":[{\"legs\":2,\"kind\":2},{\"legs\":4,\"kind\":1}],\"maxCargo\":5,\"driver\":[\"John\", \"Homer\", null]}");
			
			Assert.AreEqual(5, a.maxCargo);
			Assert.AreEqual("secret", a.Mission.target);
			Assert.AreEqual(new DateTime(2020, 4, 1), a.Mission.start);
			Assert.AreEqual(2, a.cargo[0].legs);
			Assert.AreEqual(Kind.Bird, a.cargo[0].kind);
			Assert.AreEqual(4, a.cargo[1].legs);
			Assert.AreEqual(Kind.Reptile, a.cargo[1].kind);
			Assert.AreEqual(new List<string>(new string[]{"John", "Homer", null}), a.driver);
		}

		[Test]
		public static void DecodeVector2Test1() {
			var vector = Json.Decode<Vector2>("{\"x\":4, \"y\":2}");
			Assert.AreEqual(new Vector2(4, 2), vector);
		}

		[Test]
		public static void DecodeVector2Test2() {
			var vector = Json.Decode<Vector2>("{\"x\":4, \"y\":2, \"z\":0}");
			Assert.AreEqual(new Vector2(4, 2), vector);
		}

		[Test]
		public static void DecodeQuaternionTest1() {
			var quaternion = Json.Decode<Quaternion>("{\"x\":3, \"y\":4, \"z\":5, \"w\":1}");
			Assert.AreEqual(new Quaternion(3, 4, 5, 1), quaternion);
		}

		[Test]
		public static void DecodeVector3ListTest1() {
			var vectors = Json.Decode<IList<Vector3>>("[{\"x\":4, \"y\":3, \"z\":-1}, {\"x\":1, \"y\":1, \"z\":1}, {}]");

			Assert.AreEqual(new Vector3(4, 3, -1), vectors[0]);
			Assert.AreEqual(new Vector3(1, 1, 1), vectors[1]);
			Assert.AreEqual(new Vector3(0, 0, 0), vectors[2]);
		}

		[Test]
		public static void DecodeListTest1() {
			var list = Json.Decode<IList<Animal>>("[{\"legs\":4}, {\"legs\":2}]");
			
			Assert.AreEqual(4, list[0].legs);
			Assert.AreEqual(2, list[1].legs);
		}

		[Test]
		public static void DecodeListTest2() {
			var array = Json.Decode<Animal[]>("[{\"legs\":4}, {\"legs\":2}]");
			
			Assert.AreEqual(4, array[0].legs);
			Assert.AreEqual(2, array[1].legs);
		}

		[Test]
		public static void DecodeListTest3() {
			var array = Json.Decode<bool?[]>("[true,false,null,true]");
			
			Assert.AreEqual(true, array[0]);
			Assert.AreEqual(false, array[1]);
			Assert.AreEqual(null, array[2]);
			Assert.AreEqual(true, array[3]);
		}

		[Test]
		public static void DecodeListTest4() {
			var array = Json.Decode<Boolean?[]>(" [ true , false , null , true ] ");
			
			Assert.AreEqual(true, array[0]);
			Assert.AreEqual(false, array[1]);
			Assert.AreEqual(null, array[2]);
			Assert.AreEqual(true, array[3]);
		}

		[Test]
		public static void DecodeListTest5() {
			var array = Json.Decode<object[]>("[true,false,null,true]");
			
			Assert.AreEqual(true, array[0]);
			Assert.AreEqual(false, array[1]);
			Assert.AreEqual(null, array[2]);
			Assert.AreEqual(true, array[3]);
		}

		[Test]
		public static void DecodeListTest6() {
			var list = Json.Decode<List<Boolean?>>("[true, false, null, true]");
			
			Assert.AreEqual(true, list[0]);
			Assert.AreEqual(false, list[1]);
			Assert.AreEqual(null, list[2]);
			Assert.AreEqual(true, list[3]);
		}

		[Test]
		public static void DecodeListTest7() {
			var array = Json.Decode<List<float?>>("[1.1, 2.2, -3.3, 0, -0, null]");
			
			Assert.AreEqual(1.1f, array[0]);
			Assert.AreEqual(2.2f, array[1]);
			Assert.AreEqual(-3.3f, array[2]);
			Assert.AreEqual(0f, array[3]);
			Assert.AreEqual(0f, array[4]);
			Assert.AreEqual(null, array[5]);
		}

		[Test]
		public static void DecodeListTest8() {
			var array = Json.Decode<List<string>>("[null, null, \"a\", \"b\", null]");
			
			Assert.AreEqual(null, array[0]);
			Assert.AreEqual(null, array[1]);
			Assert.AreEqual("a", array[2]);
			Assert.AreEqual("b", array[3]);
			Assert.AreEqual(null, array[4]);
		}

		[Test]
		public static void DecodeListTest9() {
			var array = Json.Decode<bool[]>("[true, false, null, true]");
			
			Assert.AreEqual(true, array[0]);
			Assert.AreEqual(false, array[1]);
			Assert.AreEqual(false, array[2]);	// the null value defaults to false
			Assert.AreEqual(true, array[3]);
		}

		[Test]
		public static void DecodeSetTest1() {
			var hashSet = Json.Decode<HashSet<int>>("[1, 2, 3, 4]");

			Assert.IsTrue(hashSet.Contains(1));
			Assert.IsTrue(hashSet.Contains(2));
			Assert.IsTrue(hashSet.Contains(3));
			Assert.IsTrue(hashSet.Contains(4));
		}

		[Test]
		public static void DecodeSetTest2() {
			var hashSet = Json.Decode<HashSet<int>>("[1, 1, 3, 4]");

			Assert.IsTrue(hashSet.Contains(1));
			Assert.IsFalse(hashSet.Contains(2));
			Assert.IsTrue(hashSet.Contains(3));
			Assert.IsTrue(hashSet.Contains(4));
		}

		[Test]
		public static void DecodeDictTest1() {
			var dict = Json.Decode<Dictionary<int, string>>("{\"3\":\"three\", \"5\":\"five\"}");
			
			Assert.AreEqual("three", dict[3]);
			Assert.AreEqual("five", dict[5]);
		}

		[Test]
		public static void DecodeDictTest2() {
			var dict = Json.Decode<IDictionary<string, bool>>("{\"a\":true, \"b\":false, \"c\":null}");
			
			Assert.AreEqual(true, dict["a"]);
			Assert.AreEqual(false, dict["b"]);
			Assert.IsFalse(dict.ContainsKey("c"));
		}

		[Test]
		public static void DecodeDictTest3() {
			var dict = Json.Decode<Dictionary<string, bool>>("{\"a\":true, \"b\":false, \"c\":null}");
			
			Assert.AreEqual(true, dict["a"]);
			Assert.AreEqual(false, dict["b"]);
			Assert.IsFalse(dict.ContainsKey("c"));
		}

        [Test]
        public static void DecodeDictTest4() {
            var dict = Json.Decode<Dictionary<int, char>>("{\"1\":\"a\", \"2\":33}");

            Assert.AreEqual('a', dict[1]);
            Assert.AreEqual((char)33, dict[2]);
        }
	}
}

