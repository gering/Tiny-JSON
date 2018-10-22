using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tiny {

	[TestFixture]
	public static class ExtensionsTest {

		[Test]
		public static void CamelCaseToSnakeCase() {
			string snakeCase = "ThisIsATestString".CamelCaseToSnakeCase();
			Console.WriteLine(snakeCase);

			Assert.AreEqual("this_is_a_test_string", snakeCase);
		}

		[Test]
		public static void SnakeCaseToCamelCase() {
			string camelCalse = "this_is_a_test_string".SnakeCaseToCamelCase();
			Console.WriteLine(camelCalse);

			Assert.AreEqual("ThisIsATestString", camelCalse);
		}
	}
}