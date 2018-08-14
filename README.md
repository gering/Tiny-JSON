# Tiny-JSON
C# JSON serializer, that works fine with Unity3D

# encoding

```javascript
var dict = new Dictionary<string, int> {
	{"three", 3},
	{"five", 5},
	{"ten", 10}
};
	
string json = Json.Encode(dict);
```
	
# decoding
  
```javascript
// Decoding a dictionary from a json string
var dict = Json.Decode<Dictionary<string, int>>(json);

// Decoding a vector2
var vector = Json.Decode<Vector2>("{\"x\":4, \"y\":2}");

// Decoding a list of vector3
var vectors = Json.Decode<IList<Vector3>>("[{\"x\":4, \"y\":3, \"z\":-1}, {\"x\":1, \"y\":1, \"z\":1}, {}]");
```

