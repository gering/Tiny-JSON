# Tiny-JSON
C# JSON serializer, that works fine with Unity3D

# encoding

```csharp
// Encode a Dictionary
var dict = new Dictionary<string, int> {
	{"three", 3},
	{"five", 5},
	{"ten", 10}
};	
string json = Json.Encode(dict);

// Encode a Unity3D type (or any type)
var v2 = new Vector2(3, 5);
string json = v2.Encode();
```

# decoding

  
```csharp
// Decoding a Dictionary from a json string
var dict = Json.Decode<Dictionary<string, int>>(json);

// Decoding a Vector2
var vector = Json.Decode<Vector2>("{\"x\":4, \"y\":2}");

// Decoding a list of Vector3
var vectors = Json.Decode<IList<Vector3>>("[{\"x\":4, \"y\":3, \"z\":-1}, {\"x\":1, \"y\":1, \"z\":1}, {}]");
```

And support for custom fields, very handy if you connect to an external service and don't want to match your naming style:

```csharp
[Serializable]
public class Session {
	[JsonProperty("token_type")]
	public string tokenType;
	[JsonProperty("access_token")] 
	public string accessToken;
	[JsonProperty("refresh_token")]
	public string refreshToken;
	[JsonProperty("expires_in")]
	public long accessTokenExpire;
}

// ... decode with custom attributed fields
byte[] result = request.downloadHandler.data;
string json = System.Text.Encoding.Default.GetString(result);
return json.Decode<Session>();
```

Or even simpler, automatically match snake case to camel case:

```csharp
[MatchSnakeCase]
public class Session {
	public string tokenType;
	public string accessToken;
	public string refreshToken;
	[JsonProperty("expires_in")]
	public long accessTokenExpire;
}

// ... decode with custom attributed fields
byte[] result = request.downloadHandler.data;
string json = System.Text.Encoding.Default.GetString(result);
return json.Decode<Session>();
```

