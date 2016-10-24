using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace Tiny {
	
	[TestFixture]
	public static class UnityTest {

		[Test]
		public static void Vector2DecodeTest1() {
			Vector2 v = Json.Decode<Vector2>("{\"x\":-1.0, \"y\":1}");
			Assert.AreEqual(-1f, v.x);
			Assert.AreEqual(1f, v.y);
		}

		[Test]
		public static void Vector2DecodeTest2() {
			Vector2[] vs = Json.Decode<Vector2[]>("[{\"x\":0, \"y\":1, \"z\":2},{\"x\":-0, \"y\":-1.1}]");
			Assert.AreEqual(0f, vs[0].x);
			Assert.AreEqual(1f, vs[0].y);
			Assert.AreEqual(0f, vs[1].x);
			Assert.AreEqual(-1.1f, vs[1].y);
		}

		[Test]
		public static void Vector2DecodeTest3() {
			Vector2 v = Json.Decode<Vector2>("{\"x\":-1.0, \"y\":null}");
			Assert.AreEqual(-1f, v.x);
			Assert.AreEqual(0f, v.y);
		}

		[Test]
		public static void Vector3DecodeTest1() {
			Vector3 v = Json.Decode<Vector3>("{\"x\":0, \"y\":1, \"z\":2}");
			Assert.AreEqual(0f, v.x);
			Assert.AreEqual(1f, v.y);
			Assert.AreEqual(2f, v.z);
		}
		
		[Test]
		public static void Vector3DecodeTest2() {
			List<Vector3> vs = Json.Decode<List<Vector3>>("[{\"x\":0, \"y\":1, \"z\":2},{\"x\":-0, \"y\":-1.1, \"z\":-2.0}]");
			Assert.AreEqual(0f, vs[0].x);
			Assert.AreEqual(1f, vs[0].y);
			Assert.AreEqual(2f, vs[0].z);
			Assert.AreEqual(0f, vs[1].x);
			Assert.AreEqual(-1.1f, vs[1].y);
			Assert.AreEqual(-2f, vs[1].z);
		}

		[Test]
		public static void QuaternionDecodeTest1() {
			Quaternion q = Json.Decode<Quaternion>("{\"x\":-1.0, \"y\":1, \"z\":0.1, \"w\":1.1}");
			Assert.AreEqual(-1f, q.x);
			Assert.AreEqual(1f, q.y);
			Assert.AreEqual(0.1f, q.z);
			Assert.AreEqual(1.1f, q.w);
		}

		[Test]
		public static void Vector2EncodeTest1() {
			Vector2 v = new Vector2(1f, -2.2f);
			string json = Json.Encode(v);
			Assert.AreEqual("{\"x\":1.0,\"y\":-2.2}", json);
		}
		
		[Test]
		public static void Vector2EncodeTest2() {
			string json = Json.Encode(new Vector2[] {new Vector2(3f, 0f), new Vector2(-1f, 0f)});
			Assert.AreEqual("[{\"x\":3.0,\"y\":0.0},{\"x\":-1.0,\"y\":0.0}]", json);
		}
	}
}