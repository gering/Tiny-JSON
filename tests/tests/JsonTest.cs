using NUnit.Framework;
using System.Collections.Generic;
using System;

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
			private float weight;
			[NonSerialized]
			public string id = "unknown";
			public string name = "Baloo";

			private Bear() {}

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
			private Mission mission;
			public T[] cargo = null;
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
			Animal a = new Bear(10.5f);
			a.legs = 4;

			string json = Json.Encode(a);
			Assert.AreEqual("{\"weight\":10.5,\"name\":\"Baloo\",\"hungry\":false,\"legs\":4,\"kind\":3}", json);
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
			string[] list = new string[]{"a", "b", null, null, "e"};
			string json = Json.Encode(list);

			Assert.AreEqual("[\"a\",\"b\",null,null,\"e\"]", json);
		}

		[Test]
		public static void EncodeListTest2() {
			List<string> list = new List<string>(new string[]{"a", "b", null, null, "e"});
			string json = Json.Encode(list);
			
			Assert.AreEqual("[\"a\",\"b\",null,null,\"e\"]", json);
		}

		[Test]
		public static void EncodeListTest3() {
			bool?[] list = new bool?[]{true, false, null};
			string json = Json.Encode(list);
			
			Assert.AreEqual("[true,false,null]", json);
		}

		[Test]
		public static void EncodeListTest4() {
			bool[] list = new bool[]{true, false};
			string json = Json.Encode(list);
			
			Assert.AreEqual("[true,false]", json);
		}

		[Test]
		public static void DecodeTest0() {
			Animal a = Json.Decode<Animal>("");
			Assert.AreEqual(null, a);
		}


		[Test]
		public static void DecodeTest1() {
			Animal a = Json.Decode<Animal>("{\"legs\":4,\"a\":false}");

			Assert.AreEqual(4, a.legs);
		}

		[Test]
		public static void DecodeTest2() {
			Animal a = Json.Decode<Animal>("{\"legs\":null}");
			
			Assert.AreEqual(0, a.legs);
		}

		[Test]
		public static void DecodeTest3() {
			Bear a = Json.Decode<Bear>("{\"legs\":4,\"hungry\":true,\"kind\":3,\"name\":null,\"id\":\"a\"}");
			
			Assert.AreEqual(4, a.legs);
			Assert.AreEqual(true, a.hungry);
			Assert.AreEqual(Kind.Mammal, a.kind);
			Assert.AreEqual(null, a.name);
			Assert.AreEqual("unknown", a.id);
		}

		[Test]
		public static void DecodeTest4() {
			Transporter<Animal> a = Json.Decode<Transporter<Animal>>("{\"mission\":{\"target\":\"secret\",\"start\":\"2020-03-31T22:00:00.000Z\"},\"cargo\":[{\"legs\":2,\"kind\":2},{\"legs\":4,\"kind\":1}],\"maxCargo\":5,\"driver\":[\"John\", \"Homer\", null]}");
			
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
		public static void DecodeListTest1() {
			IList<Animal> a = Json.Decode<IList<Animal>>("[{\"legs\":4}, {\"legs\":3}]");
			
			Assert.AreEqual(4, a[0].legs);
			Assert.AreEqual(3, a[1].legs);
		}

		[Test]
		public static void DecodeListTest2() {
			Animal[] a = Json.Decode<Animal[]>("[{\"legs\":4}, {\"legs\":3}]");
			
			Assert.AreEqual(4, a[0].legs);
			Assert.AreEqual(3, a[1].legs);
		}

		[Test]
		public static void DecodeListTest3() {
			bool?[] a = Json.Decode<bool?[]>("[true,false,null,true]");
			
			Assert.AreEqual(true, a[0]);
			Assert.AreEqual(false, a[1]);
			Assert.AreEqual(null, a[2]);
			Assert.AreEqual(true, a[3]);
		}

		[Test]
		public static void DecodeListTest4() {
			Boolean?[] a = Json.Decode<Boolean?[]>(" [ true , false , null , true ] ");
			
			Assert.AreEqual(true, a[0]);
			Assert.AreEqual(false, a[1]);
			Assert.AreEqual(null, a[2]);
			Assert.AreEqual(true, a[3]);
		}

		[Test]
		public static void DecodeListTest5() {
			object[] a = Json.Decode<object[]>("[true,false,null,true]");
			
			Assert.AreEqual(true, a[0]);
			Assert.AreEqual(false, a[1]);
			Assert.AreEqual(null, a[2]);
			Assert.AreEqual(true, a[3]);
		}

		[Test]
		public static void DecodeListTest6() {
			List<Boolean?> a = Json.Decode<List<Boolean?>>("[true, false, null, true]");
			
			Assert.AreEqual(true, a[0]);
			Assert.AreEqual(false, a[1]);
			Assert.AreEqual(null, a[2]);
			Assert.AreEqual(true, a[3]);
		}

		[Test]
		public static void DecodeListTest7() {
			List<float?> a = Json.Decode<List<float?>>("[1.1, 2.2, -3.3, 0, -0, null]");
			
			Assert.AreEqual(1.1f, a[0]);
			Assert.AreEqual(2.2f, a[1]);
			Assert.AreEqual(-3.3f, a[2]);
			Assert.AreEqual(0f, a[3]);
			Assert.AreEqual(0f, a[4]);
			Assert.AreEqual(null, a[5]);
		}

		[Test]
		public static void DecodeListTest8() {
			List<string> a = Json.Decode<List<string>>("[null, null, \"a\", \"b\", null]");
			
			Assert.AreEqual(null, a[0]);
			Assert.AreEqual(null, a[1]);
			Assert.AreEqual("a", a[2]);
			Assert.AreEqual("b", a[3]);
			Assert.AreEqual(null, a[4]);
		}

		[Test]
		public static void DecodeListTest9() {
			bool[] a = Json.Decode<bool[]>("[true, false, null, true]");
			
			Assert.AreEqual(true, a[0]);
			Assert.AreEqual(false, a[1]);
			Assert.AreEqual(false, a[2]);	// the null value defaults to false
			Assert.AreEqual(true, a[3]);
		}

		[Test]
		public static void DecodeDictTest1() {
			IDictionary<string, bool> dict = Json.Decode<IDictionary<string, bool>>("{\"a\":true, \"b\":false, \"c\":null}");
			
			Assert.AreEqual(true, dict["a"]);
			Assert.AreEqual(false, dict["b"]);
			Assert.IsFalse(dict.ContainsKey("c"));
		}
	}
}

