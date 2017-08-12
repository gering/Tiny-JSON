using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Tiny {

	using Encoder = Action<object, JsonBuilder>;
	using Decoder = Func<Type, object, object>;

	public static class JsonMapper {

		internal static Encoder genericEncoder;
		internal static Decoder genericDecoder;
		internal static Dictionary<Type, Encoder> encoders = new Dictionary<Type, Encoder>();
		internal static Dictionary<Type, Decoder> decoders = new Dictionary<Type, Decoder>();

		static JsonMapper() {
			RegisterDefaultEncoder();
			RegisterDefaultDecoder();
		}

		static void RegisterDefaultEncoder() {

			// register generic encoder
			RegisterEncoder<object>((obj, builder) => {
				//Console.WriteLine("using generic encoder");
				builder.AppendBeginObject();
				Type type = obj.GetType();
				bool first = true;
				while (type != null) {
					foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
						if (field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0) {
							if (first) first = false; else builder.AppendSeperator();
							EncodeNameValue(field.Name, field.GetValue(obj), builder);
						}
					}
					type = type.BaseType;
				}
				builder.AppendEndObject();
			});

			// register IDictionary encoder
			RegisterEncoder<IDictionary>((obj, builder) => {
				//Console.WriteLine("using IDictionary encoder");
				builder.AppendBeginObject();
				bool first = true;
				IDictionary dict = (IDictionary)obj;
				foreach (var key in dict.Keys) {
					if (first) first = false; else builder.AppendSeperator();
					EncodeNameValue(key.ToString(), dict[key], builder);
				}
				builder.AppendEndObject();
			});

			// register IEnumerable support for all list and array types
			RegisterEncoder<IEnumerable>((obj, builder) => {
				//Console.WriteLine("using IEnumerable encoder");
				builder.AppendBeginArray();
				bool first = true;
				foreach (var item in (IEnumerable)obj) {
					if (first) first = false; else builder.AppendSeperator();
					EncodeValue(item, builder);
				}
				builder.AppendEndArray();
			});

			// register zulu date support
			RegisterEncoder<DateTime>((obj, builder) => {
				DateTime date = (DateTime)obj;
				string zulu = date.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
				builder.AppendString(zulu);
			});
		}

		static void RegisterDefaultDecoder() {

			// register generic decoder
			RegisterDecoder<object>((type, jsonObj) => {
				object instance = Activator.CreateInstance(type, true);

				if (jsonObj is IDictionary) {
					foreach (DictionaryEntry item in (IDictionary)jsonObj) {
						string name = (string)item.Key;
						if (!DecodeValue(instance, name, item.Value)) {
							Console.WriteLine("couldn't decode field \"" + name + "\" of " + type);
						}
					}
				} else {
					Console.WriteLine("unsupported json type: " + (jsonObj != null ? jsonObj.GetType().ToString() : "null"));
				}

				return instance;
			});

			// register IList support 
			RegisterDecoder<IEnumerable>((type, jsonObj) => {
				if (typeof(IEnumerable).IsAssignableFrom(type)) {
					if (jsonObj is IList) {
						IList jsonList = (IList)jsonObj;
						if (type.IsArray) {                                                         // Arrays
							Type elementType = type.GetElementType();
							bool nullable = elementType.IsNullable();
							var array = Array.CreateInstance(elementType, jsonList.Count);
							for (int i = 0; i < jsonList.Count; i++) {
								object value = DecodeValue(jsonList[i], elementType);
								if (value != null || nullable) array.SetValue(value, i);
							}
							return array;
						} else if (type.GetGenericArguments().Length == 1) {                        // Generic List
							Type genericType = type.GetGenericArguments()[0];
							if (type.HasGenericInterface(typeof(IList<>))) {                        // IList
								IList instance = null;
								bool nullable = genericType.IsNullable();
								if (type != typeof(IList) && typeof(IList).IsAssignableFrom(type)) {
									instance = Activator.CreateInstance(type, true) as IList;
								} else {
									Type genericListType = typeof(List<>).MakeGenericType(genericType);
									instance = Activator.CreateInstance(genericListType) as IList;
								}
								foreach (var item in jsonList) {
									object value = DecodeValue(item, genericType);
									if (value != null || nullable) instance.Add(value);
								}
								return instance;
							} else if (type.HasGenericInterface(typeof(ICollection<>))) {			// ICollection
								var listType = type.IsInstanceOfGenericType(typeof(HashSet<>)) ? typeof(HashSet<>) : typeof(List<>);
								var constructedListType = listType.MakeGenericType(genericType);
								var instance = Activator.CreateInstance(constructedListType, true);
								bool nullable = genericType.IsNullable();
								MethodInfo addMethodInfo = type.GetMethod("Add");
								if (addMethodInfo != null) {
									foreach (var item in jsonList) {
										object value = DecodeValue(item, genericType);
										if (value != null || nullable) addMethodInfo.Invoke(instance, new object[] { value });
									}
									return instance;
								} 
							} 
							Console.WriteLine("IEnumerable type not supported " + type);
						}
					}
					if (jsonObj is Dictionary<string, object>) {            // Dictionary
						Dictionary<string, object> jsonDict = (Dictionary<string, object>)jsonObj;
						if (type.GetGenericArguments().Length == 2) {
							IDictionary instance = null;
							Type keyType = type.GetGenericArguments()[0];
							Type genericType = type.GetGenericArguments()[1];
							bool nullable = genericType.IsNullable();
							if (type != typeof(IDictionary) && typeof(IDictionary).IsAssignableFrom(type)) {
								instance = Activator.CreateInstance(type, true) as IDictionary;
							} else {
								Type genericDictType = typeof(Dictionary<,>).MakeGenericType(keyType, genericType);
								instance = Activator.CreateInstance(genericDictType) as IDictionary;
							}
							foreach (KeyValuePair<string, object> item in jsonDict) {
								Console.WriteLine(item.Key + " = " + JsonMapper.DecodeValue(item.Value, genericType));
								object value = DecodeValue(item.Value, genericType);
								object key = item.Key;
								if (keyType == typeof(int)) key = Int32.Parse(item.Key);
								if (value != null || nullable) instance.Add(key, value);
							}
							return instance;
						} else {
							Console.WriteLine("unexpected type arguemtns");
						}
					}
					if (jsonObj is Dictionary<int, object>) {           // Dictionary
						// convert int to string key
						Dictionary<string, object> jsonDict = new Dictionary<string, object>();
						foreach (KeyValuePair<int, object> keyValuePair in (Dictionary<int, object>)jsonObj) {
							jsonDict.Add(keyValuePair.Key.ToString(), keyValuePair.Value);
						}
						if (type.GetGenericArguments().Length == 2) {
							IDictionary instance = null;
							Type keyType = type.GetGenericArguments()[0];
							Type genericType = type.GetGenericArguments()[1];
							bool nullable = genericType.IsNullable();
							if (type != typeof(IDictionary) && typeof(IDictionary).IsAssignableFrom(type)) {
								instance = Activator.CreateInstance(type, true) as IDictionary;
							} else {
								Type genericDictType = typeof(Dictionary<,>).MakeGenericType(keyType, genericType);
								instance = Activator.CreateInstance(genericDictType) as IDictionary;
							}
							foreach (KeyValuePair<string, object> item in jsonDict) {
								Console.WriteLine(item.Key + " = " + DecodeValue(item.Value, genericType));
								object value = DecodeValue(item.Value, genericType);
								if (value != null || nullable) instance.Add(Convert.ToInt32(item.Key), value);
							}
							return instance;
						} else {
							Console.WriteLine("unexpected type arguemtns");
						}
					}
				}
				Console.WriteLine("couldn't decode: " + type);
				return null;
			});
		}

		public static void RegisterDecoder<T>(Decoder decoder) {
			if (typeof(T) == typeof(object)) {
				genericDecoder = decoder;
			} else {
				decoders[typeof(T)] = decoder;
			}
		}

		public static void RegisterEncoder<T>(Encoder encoder) {
			if (typeof(T) == typeof(object)) {
				genericEncoder = encoder;
			} else {
				encoders[typeof(T)] = encoder;
			}
		}

		public static Decoder GetDecoder(Type type) {
			if (decoders.ContainsKey(type)) {
				return decoders[type];
			} 
			foreach (var entry in decoders) {
				Type baseType = entry.Key;
				if (baseType.IsAssignableFrom(type)) {
					return entry.Value;
				}
			}
			return genericDecoder;
		}

		public static Encoder GetEncoder(Type type) {
			if (encoders.ContainsKey(type)) {
				return encoders[type];
			} 
			foreach (var entry in encoders) {
				Type baseType = entry.Key;
				if (baseType.IsAssignableFrom(type)) {
					return entry.Value;
				}
			}
			return genericEncoder;
		}

		public static T DecodeJsonObject<T>(object jsonObj) {
			Decoder decoder = GetDecoder(typeof(T));
			return (T)decoder(typeof(T), jsonObj);
		}

		public static void EncodeValue(object value, JsonBuilder builder) {
			if (JsonBuilder.IsSupported(value)) {
				builder.AppendValue(value);
			} else {
				Encoder encoder = GetEncoder(value.GetType()); 
				if (encoder != null) {
					encoder(value, builder);
				} else {
					Console.WriteLine("encoder for " + value.GetType() + " not found");
				}
			}
		}

		public static void EncodeNameValue(string name, object value, JsonBuilder builder) {
			builder.AppendName(UnwrapName(name));
			EncodeValue(value, builder);
		}

		public static string UnwrapName(string name) {
            if (name.StartsWith("<", StringComparison.InvariantCulture) && name.Contains(">")) {
				return name.Substring(name.IndexOf("<", StringComparison.InvariantCulture) + 1, name.IndexOf(">", StringComparison.InvariantCulture) - 1);
			}
			return name;
		}

		static object ConvertValue(object value, Type type) {
			if (value != null) {
				Type safeType = Nullable.GetUnderlyingType(type) ?? type;
                if (!type.IsEnum) {
                    return Convert.ChangeType(value, safeType);
                } else {
                    if (value is string) {
                        return Enum.Parse(type, (string)value);
                    } else {
                        return Enum.ToObject(type, value);
                    }
                }
			}
			return value;
		}

		static object DecodeValue(object value, Type targetType) {
			if (value == null) return null;

			if (JsonBuilder.IsSupported(value)) {
				value = ConvertValue(value, targetType);
			}

			// use a registered decoder
			if (value != null && !targetType.IsAssignableFrom(value.GetType())) {
				Decoder decoder = GetDecoder(targetType);
				value = decoder(targetType, value);
			}

			if (value != null && targetType.IsAssignableFrom(value.GetType())) {
				return value;
			} else {
				Console.WriteLine("couldn't decode: " + targetType);
				return null;
			}
		}

		public static bool DecodeValue(object target, string name, object value) {
			Type type = target.GetType();
			while (type != null) {
				foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
					if (field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0) {
						if (name == UnwrapName(field.Name)) {
							if (value != null) {
								Type targetType = field.FieldType;
								object decodedValue = DecodeValue(value, targetType);

								if (decodedValue != null && targetType.IsAssignableFrom(decodedValue.GetType())) {
									field.SetValue(target, decodedValue);
									return true;
								} else {
									return false;
								}
							} else {
								field.SetValue(target, null);
								return true;
							}
						}
					}
				}
				type = type.BaseType;
			}
			return false;
		}
	}
}

