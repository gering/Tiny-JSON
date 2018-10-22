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
			[JsonProperty("legs")]
			public int numberOfLegs;

			internal Kind Kind {get; set; }
		}

		class Bear : Animal {
			[JsonProperty("hungry")]
			public bool Hungry {get; set; }
			float weight;
			[NonSerialized]
			public string id = "unknown";
			public string name = "Baloo";

			Bear() {}

			public Bear(float weight) {
				this.weight = weight;
				this.Kind = Kind.Mammal;
			}
		}

		struct Mission {
			public string target;
			public DateTime start;
		}

		class Transporter<T> {
			public T[] cargo;
			public int? maxCargo;
			public List<string> Driver { get; private set; }

			[JsonProperty("mission")]
			Mission _mission;
			public Mission Mission {
				get { return _mission; }
				set { _mission = value; }
			}

			public Transporter() {
				Driver = new List<string>();
			}
		}

		[MatchSnakeCase]
		public class Session {
			public string tokenType;
			public string accessToken;
			public string refreshToken;
			[JsonProperty("expires_in")]
			public long accessTokenExpire;
		}

		[Test]
		public static void EncodeTest1() {
			Animal a = new Animal {
				numberOfLegs = 4
			};
			string json = Json.Encode(a);

			Assert.AreEqual("{\"legs\":4,\"Kind\":0}", json);
		}

		[Test]
		public static void EncodeTest2() {
			Animal a = new Bear(10) {
				numberOfLegs = 4
			};
			string json = Json.Encode(a);

			Assert.AreEqual("{\"hungry\":false,\"weight\":10.0,\"name\":\"Baloo\",\"legs\":4,\"Kind\":3}", json);
		}

		[Test]
		public static void EncodeTest3() {
			Animal a = new Animal {
				numberOfLegs = 4
			};

			Animal b = new Animal {
				numberOfLegs = 2
			};

			Mission m;
			m.target = "unknown";
			m.start = new DateTime(2020, 4, 1);

			Transporter<Animal> t = new Transporter<Animal> {
				Mission = m,
				cargo = new Animal[] { a, b }
			};
			t.Driver.Add("Joe");

			string json = Json.Encode(t);

			Assert.AreEqual("{\"cargo\":[{\"legs\":4,\"Kind\":0},{\"legs\":2,\"Kind\":0}],\"maxCargo\":null,\"Driver\":[\"Joe\"],\"mission\":{\"target\":\"unknown\",\"start\":\"2020-03-31T22:00:00.000Z\"}}", json);
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
			Animal a = new Bear(10.5f) {
				numberOfLegs = 4
			};
			string json = Json.Encode(a, true);

			Assert.AreEqual("{\n\t\"hungry\" : false,\n\t\"weight\" : 10.5,\n\t\"name\" : \"Baloo\",\n\t\"legs\" : 4,\n\t\"Kind\" : 3\n}\n", json);
		}

		[Test]
		public static void PrettyEncodeTest2() {
			string[] array = {"ä", "\"", "", null};
			string json = array.Encode(true);

			Assert.AreEqual("[\n\t\"ä\",\n\t\"\\\"\",\n\t\"\",\n\tnull\n]\n", json);
		}

		[Test]
		public static void EncodeSnakeCaseTest1() {
			var session = new Session {
				tokenType = "bearer",
				accessToken = "19786C58",
				refreshToken = "0EBEFBC2",
				accessTokenExpire = 1800
			};
			var json = session.Encode();

			Assert.AreEqual(@"{""token_type"":""bearer"",""access_token"":""19786C58"",""refresh_token"":""0EBEFBC2"",""expires_in"":1800}", json);
		}

		[Test]
		public static void DecodeTest0() {
			var a = Json.Decode<Animal>("");

			Assert.AreEqual(null, a);
		}

		[Test]
		public static void DecodeTest1() {
			var json = "{\"legs\":4,\"a\":false}";
			var a = json.Decode<Animal>();

			Assert.AreEqual(4, a.numberOfLegs);
		}

		[Test]
		public static void DecodeTest2() {
			var json = "{\"legs\":null}";
			var a = json.Decode<Animal>();

			Assert.AreEqual(0, a.numberOfLegs);
		}

		[Test]
		public static void DecodeTest3() {
			var json = "{\"Legs\":4,\"hungry\":true,\"kind\":3,\"name\":null,\"id\":\"a\"}";
			var a = json.Decode<Bear>();
			
			Assert.AreEqual(4, a.numberOfLegs);
			Assert.AreEqual(true, a.Hungry);
			Assert.AreEqual(Kind.Mammal, a.Kind);
			Assert.AreEqual(null, a.name);
			Assert.AreEqual("unknown", a.id);
		}

        [Test]
        public static void DecodeTest4() {
			var json = "{\"legs\":4,\"hungry\":true,\"kind\":\"Mammal\",\"name\":null}";
			var a = json.Decode<Bear>();

            Assert.AreEqual(4, a.numberOfLegs);
            Assert.AreEqual(true, a.Hungry);
			Assert.AreEqual(Kind.Mammal, a.Kind);
            Assert.AreEqual(null, a.name);
        }

		[Test]
		public static void DecodeTest5() {
			var json = "{\"mission\":{\"target\":\"secret\",\"start\":\"2020-03-31T22:00:00.000Z\"},\"cargo\":[{\"legs\":2,\"kind\":2},{\"legs\":4,\"kind\":1}],\"maxCargo\":5,\"driver\":[\"John\", \"Homer\", null]}";
			var a = json.Decode<Transporter<Animal>>();
			
			Assert.AreEqual(5, a.maxCargo);
			Assert.AreEqual("secret", a.Mission.target);
			Assert.AreEqual(new DateTime(2020, 4, 1), a.Mission.start);
			Assert.AreEqual(2, a.cargo[0].numberOfLegs);
			Assert.AreEqual(Kind.Bird, a.cargo[0].Kind);
			Assert.AreEqual(4, a.cargo[1].numberOfLegs);
			Assert.AreEqual(Kind.Reptile, a.cargo[1].Kind);
			Assert.AreEqual(new List<string>(new string[]{"John", "Homer", null}), a.Driver);
		}

		[Test]
		public static void DecodeVector2Test1() {
			var json = "{\"x\":4, \"y\":2}";
			var vector = json.Decode<Vector2>();

			Assert.AreEqual(new Vector2(4, 2), vector);
		}

		[Test]
		public static void DecodeVector2Test2() {
			var json = "{\"x\":4, \"y\":2, \"z\":0}";
			var vector = json.Decode<Vector2>();

			Assert.AreEqual(new Vector2(4, 2), vector);
		}

		[Test]
		public static void DecodeQuaternionTest1() {
			var json = "{\"x\":3, \"y\":4, \"z\":5, \"w\":1}";
			var quaternion = json.Decode<Quaternion>();

			Assert.AreEqual(new Quaternion(3, 4, 5, 1), quaternion);
		}

		[Test]
		public static void DecodeVector3ListTest1() {
			var json = "[{\"x\":4, \"y\":3, \"z\":-1}, {\"x\":1, \"y\":1, \"z\":1}, {}]";
			var vectors = json.Decode<IList<Vector3>>();

			Assert.AreEqual(new Vector3(4, 3, -1), vectors[0]);
			Assert.AreEqual(new Vector3(1, 1, 1), vectors[1]);
			Assert.AreEqual(new Vector3(0, 0, 0), vectors[2]);
		}

		[Test]
		public static void DecodeListTest1() {
			var json = "[{\"legs\":4}, {\"legs\":2}]";
			var list = json.Decode<IList<Animal>>();
			
			Assert.AreEqual(4, list[0].numberOfLegs);
			Assert.AreEqual(2, list[1].numberOfLegs);
		}

		[Test]
		public static void DecodeListTest2() {
			var json = "[{\"legs\":4}, {\"legs\":2}]";
			var array = json.Decode<Animal[]>();
			
			Assert.AreEqual(4, array[0].numberOfLegs);
			Assert.AreEqual(2, array[1].numberOfLegs);
		}

		[Test]
		public static void DecodeListTest3() {
			var json = "[true,false,null,true]";
			var array = json.Decode<bool?[]>();
			
			Assert.AreEqual(true, array[0]);
			Assert.AreEqual(false, array[1]);
			Assert.AreEqual(null, array[2]);
			Assert.AreEqual(true, array[3]);
		}

		[Test]
		public static void DecodeListTest4() {
			var json = " [ true , false , null , true ] ";
			var array = json.Decode<Boolean?[]>();
			
			Assert.AreEqual(true, array[0]);
			Assert.AreEqual(false, array[1]);
			Assert.AreEqual(null, array[2]);
			Assert.AreEqual(true, array[3]);
		}

		[Test]
		public static void DecodeListTest5() {
			var json = "[true,false,null,true]";
			var array = json.Decode<object[]>();
			
			Assert.AreEqual(true, array[0]);
			Assert.AreEqual(false, array[1]);
			Assert.AreEqual(null, array[2]);
			Assert.AreEqual(true, array[3]);
		}

		[Test]
		public static void DecodeListTest6() {
			var json = "[true, false, null, true]";
			var list = json.Decode<List<Boolean?>>();
			
			Assert.AreEqual(true, list[0]);
			Assert.AreEqual(false, list[1]);
			Assert.AreEqual(null, list[2]);
			Assert.AreEqual(true, list[3]);
		}

		[Test]
		public static void DecodeListTest7() {
			var json = "[1.1, 2.2, -3.3, 0, -0, null]";
			var array = json.Decode<List<float?>>();
			
			Assert.AreEqual(1.1f, array[0]);
			Assert.AreEqual(2.2f, array[1]);
			Assert.AreEqual(-3.3f, array[2]);
			Assert.AreEqual(0f, array[3]);
			Assert.AreEqual(0f, array[4]);
			Assert.AreEqual(null, array[5]);
		}

		[Test]
		public static void DecodeListTest8() {
			var json = "[null, null, \"a\", \"b\", null]";
			var array = json.Decode<List<string>>();
			
			Assert.AreEqual(null, array[0]);
			Assert.AreEqual(null, array[1]);
			Assert.AreEqual("a", array[2]);
			Assert.AreEqual("b", array[3]);
			Assert.AreEqual(null, array[4]);
		}

		[Test]
		public static void DecodeListTest9() {
			var json = "[true, false, null, true]";
			var array = json.Decode<bool[]>();
			
			Assert.AreEqual(true, array[0]);
			Assert.AreEqual(false, array[1]);
			Assert.AreEqual(false, array[2]);	// the null value defaults to false
			Assert.AreEqual(true, array[3]);
		}

		[Test]
		public static void DecodeSetTest1() {
			var json = "[1, 2, 3, 4]";
			var hashSet = json.Decode<HashSet<int>>();

			Assert.IsTrue(hashSet.Contains(1));
			Assert.IsTrue(hashSet.Contains(2));
			Assert.IsTrue(hashSet.Contains(3));
			Assert.IsTrue(hashSet.Contains(4));
		}

		[Test]
		public static void DecodeSetTest2() {
			var json = "[1, 1, 3, 4]";
			var hashSet = json.Decode<HashSet<int>>();

			Assert.IsTrue(hashSet.Contains(1));
			Assert.IsFalse(hashSet.Contains(2));
			Assert.IsTrue(hashSet.Contains(3));
			Assert.IsTrue(hashSet.Contains(4));
		}

		[Test]
		public static void DecodeDictTest1() {
			var json = "{\"3\":\"three\", \"5\":\"five\"}";
			var dict = json.Decode<Dictionary<int, string>>();
			
			Assert.AreEqual("three", dict[3]);
			Assert.AreEqual("five", dict[5]);
		}

		[Test]
		public static void DecodeDictTest2() {
			var json = "{\"a\":true, \"b\":false, \"c\":null}";
			var dict = json.Decode<IDictionary<string, bool>>();
			
			Assert.AreEqual(true, dict["a"]);
			Assert.AreEqual(false, dict["b"]);
			Assert.IsFalse(dict.ContainsKey("c"));
		}

		[Test]
		public static void DecodeDictTest3() {
			var json = "{\"a\":true, \"b\":false, \"c\":null}";
			var dict = json.Decode<Dictionary<string, bool>>();
			
			Assert.AreEqual(true, dict["a"]);
			Assert.AreEqual(false, dict["b"]);
			Assert.IsFalse(dict.ContainsKey("c"));
		}

        [Test]
        public static void DecodeDictTest4() {
			var json = "{\"1\":\"a\", \"2\":33}";
			var dict = json.Decode<Dictionary<int, char>>();

            Assert.AreEqual('a', dict[1]);
            Assert.AreEqual((char)33, dict[2]);
        }

		[Test]
		public static void DecodeSnakeCaseTest1() {
			var json = @"{""token_type"":""bearer"", ""access_token"":""19786C58"", ""refresh_token"":""0EBEFBC2"", ""expires_in"":1800}";
			var session = json.Decode<Session>();

			Assert.AreEqual("bearer", session.tokenType);
			Assert.AreEqual("19786C58", session.accessToken);
			Assert.AreEqual("0EBEFBC2", session.refreshToken);
			Assert.AreEqual(1800, session.accessTokenExpire);
		}
	}
}

