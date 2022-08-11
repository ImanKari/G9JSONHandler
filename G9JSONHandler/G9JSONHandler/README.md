[<img alt="G9TM" src="https://github.com/ImanKari/G9JSONHandler/blob/main/G9JSONHandler/G9JSONHandler/G9-Icon.png?raw=true" width="50" />](https://www.nuget.org/profiles/ImanKari)**G9JSONHandler**

[![NuGet version (G9JSONHandler)](https://img.shields.io/nuget/v/G9JSONHandler.svg?style=flat-square)](https://www.nuget.org/packages/G9JSONHandler/)
[![Build Status](https://g9tm.visualstudio.com/G9JSONHandler/_apis/build/status/G9JSONHandler?branchName=main)](https://g9tm.visualstudio.com/G9JSONHandler/_build/latest?definitionId=14&branchName=main)

## G9JSONHandler is a pretty small library for working with JSON.
## Sample Object
```csharp
using G9JSONHandler;
using G9JSONHandler.Attributes;

class TestObject
{
    [G9AttrJsonComment("Custom JSON Comment")]
    public string Name = ".NET";
    public ConsoleColor Color = ConsoleColor.DarkMagenta;
    [G9AttrJsonStoreEnumAsString]
    public ConsoleColor Color2 = ConsoleColor.DarkGreen;
    [G9AttrJsonCustomMemberName("MyRegisterDateTime")]
    public DateTime RegisterDateTime = new(1990, 9, 1);
    public TimeSpan Time = new(9, 9, 9);
    public string[] Array = { "Item 1", "Item 2", "Item 3" };
    public Dictionary<string, string> Dictionary = new()
        { { "Key 1", "Value 1" }, { "Key 2", "Value 2" }, { "Key 3", "Value 3" } };
    [G9AttrJsonIgnoreMember] 
    public string Address = "...";
}
```

## Object To JSON
```csharp
TestObject testObject = new TestObject();
// Unformatted JSON
string unformattedJson = testObject.G9ObjectToJson();
// Formatted JSON
string formattedJson = testObject.G9ObjectToJson(true);
```
### Result
```json
// Unformatted JSON Result

{/* Custom JSON Comment */"Name":".NET","Color":5,"Color2":"DarkGreen","MyRegisterDateTime":"09/01/1990 00:00:00","Time":"09:09:09","Array":["Item 1","Item 2","Item 3"],"Dictionary":{"Key 1":"Value 1","Key 2":"Value 2","Key 3":"Value 3"}}

// Formatted JSON Result

{
	/* Custom JSON Comment */
	"Name":".NET",
	"Color":5,
	"Color2":"DarkGreen",
	"MyRegisterDateTime":"09/01/1990 00:00:00",
	"Time":"09:09:09",
	"Array":
	[
		"Item 1",
		"Item 2",
		"Item 3"
	],
	"Dictionary":
	{
		"Key 1":"Value 1",
		"Key 2":"Value 2",
		"Key 3":"Value 3"
	}
}
```

## JSON To Object

```csharp
// Object To JSON
// string formattedJson = testObject.G9ObjectToJson(true);

// JSON To Object
TestObject newObject = formattedJson.G9JsonToObject<TestObject>();
Console.WriteLine(newObject.Name); // .NET
Console.WriteLine(newObject.Color); // DarkMagenta
Console.WriteLine(newObject.Array.Length); // 3
Console.WriteLine(newObject.Dictionary.Count); // 3
Console.WriteLine(newObject.Dictionary["Key 2"]); // Value 2
```
## Attributes

- **G9AttrJsonComment**
  - This attribute enables you to write several comments (notes) for each item member in JSON.
    - This attribute can use several times for an item member.
- **G9AttrJsonStoreEnumAsString**
  - This attribute enables you to store an Enum object as a string value in JSON (By default, an Enum object storing as a number).
- **G9AttrJsonCustomMemberName**
  - This attribute enables you to choose a custom name for a member item for storing in JSON (At parsing time, the parser can recognize and pair the member item automatically).
- **G9AttrJsonIgnoreMember**
  - This attribute enables you to ignore an item member for storing in JSON.