using System;
using System.Linq;

namespace Tiny {
	public static class TypeExtensions {
		public static bool IsInstanceOfGenericType(this Type type, Type genericType) {
			while (type != null) {
				if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType) return true;
				type = type.BaseType;
			}
			return false;
		}

		public static bool HasGenericInterface(this Type type, Type genericInterface) {
			if (genericInterface == null) throw new ArgumentNullException();
			var interfaceTest = new Predicate<Type>(i => i.IsGenericType && i.GetGenericTypeDefinition().IsAssignableFrom(genericInterface));
			return interfaceTest(type) || type.GetInterfaces().Any(i => interfaceTest(i));
		}
	}

	public static class JsonExtensions {
		public static bool IsNullable(this Type type) {
			return Nullable.GetUnderlyingType(type) != null || !type.IsPrimitive;
		}

		public static bool IsNumeric(this Type type) {
			if (type.IsEnum) return false;
			switch (Type.GetTypeCode(type)) {
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				case TypeCode.Object:
					Type underlyingType = Nullable.GetUnderlyingType(type);
					return underlyingType != null && underlyingType.IsNumeric();
				default:
					return false;
			}
		}

		public static bool IsFloatingPoint(this Type type) {
			if (type.IsEnum) return false;
			switch (Type.GetTypeCode(type)) {
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				case TypeCode.Object:
					Type underlyingType = Nullable.GetUnderlyingType(type);
					return underlyingType != null && underlyingType.IsFloatingPoint();
				default:
					return false;
			}
		}
	}

	public static class StringBuilderExtensions {
		public static void Clear(this System.Text.StringBuilder sb) {
			sb.Length = 0;
		}
	}
}
