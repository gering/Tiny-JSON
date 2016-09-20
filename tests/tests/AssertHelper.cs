using NUnit.Framework;

namespace Tiny {

	public static class AssertHelper {
		public static void Contains(string expected, string actual) {
			Assert.True(actual.Contains(expected));
		}
	}
}
