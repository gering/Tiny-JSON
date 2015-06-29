using System;
using System.IO;
using System.Text;
using System.Collections;

namespace Tiny {

	public class JsonBuilder {

		StringBuilder builder;

		public JsonBuilder() {
			builder = new StringBuilder();
		}

		public void AppendBeginObject() {
			builder.Append("{");
		}

		public void AppendEndObject() {
			builder.Append("}");
		}

		public void AppendBeginArray() {
			builder.Append("[");
		}
		
		public void AppendEndArray() {
			builder.Append("]");
		}

		public void AppendSeperator() {
			builder.Append(",");
		}

		public void AppendNull() {
			builder.Append("null");
		}

		public void AppendBool(bool b) {
			builder.Append(b ? "true" : "false");
		}
	
		public void AppendNumber(object number) {
			if (number != null) {
				builder.Append(number.ToString());
			} else {
				AppendNull();
			}
		}

		public void AppendString(string str) {
			if (str != null) {
				builder.Append('\"');
				foreach (var c in str.ToCharArray()) {
					switch (c) {
					case '"':
						builder.Append("\\\"");
						break;
					case '\\':
						builder.Append("\\\\");
						break;
					case '\b':
						builder.Append("\\b");
						break;
					case '\f':
						builder.Append("\\f");
						break;
					case '\n':
						builder.Append("\\n");
						break;
					case '\r':
						builder.Append("\\r");
						break;
					case '\t':
						builder.Append("\\t");
						break;
					default:
						int codepoint = Convert.ToInt32(c);
						if ((codepoint >= 32) && (codepoint <= 126)) {
							builder.Append(c);
						} else {
							builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
						}
						break;
					}
				}
				builder.Append('\"');
			} else {
				AppendNull();
			}
		}

		public void AppendArray(IEnumerable enumerable) {
			if (enumerable != null) {
				AppendBeginArray();
				bool first = true;
				foreach (var item in enumerable) {
					if (first) first = false; else AppendSeperator();
					AppendValue(item);
				}
				AppendEndArray();
			} else {
				AppendNull();
			}
		}

		public void AppendDictionary(IDictionary dict) {
			if (dict != null) {
				AppendBeginObject();
				bool first = true;
				foreach (DictionaryEntry entry in dict) {
					if (first) first = false; else AppendSeperator();
					AppendString(entry.Key.ToString());
					builder.Append(":");
					AppendValue(entry.Value);
				}
				AppendEndObject();
			} else {
				AppendNull();
			}
		}

		public void AppendValue(object value) {
			if (value == null) {
				AppendNull();
			} else if (value is bool) {
				AppendBool((bool)value);
			} else if (value is string) {
				AppendString((string)value);
			} else if (IsNumber(value)) {
				AppendNumber(value);
			} else {
				Console.WriteLine("type " + value.GetType() + " not supported");
			}
		}

		public void AppendName(string name) {
			AppendString(name);
			builder.Append(":");
		}

		internal static bool IsNumber(object value) {
			return value != null && value.GetType().IsNumeric();
		}

		internal static bool IsSupported(object obj) {
			if (obj == null) return true;
			if (obj is bool) return true;
			if (obj is string) return true;
			if (IsNumber(obj)) return true;
			return false;
		}

		public override string ToString() {
			return builder.ToString();
		}
	}
}

