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
var dict = Json.Decode<Dictionary<string, int>>(json);
```

