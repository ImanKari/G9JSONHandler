[![G9TM](https://raw.githubusercontent.com/ImanKari/G9JSONHandler/main/G9JSONHandler/Asset/G9JSONHandler.png)](http://www.g9tm.com/) **G9JSONHandler**

[![NuGet version (G9JSONHandler)](https://img.shields.io/nuget/v/G9JSONHandler.svg?style=flat-square)](https://www.nuget.org/packages/G9JSONHandler/)
[![Azure DevOps Pipeline Build Status](https://raw.githubusercontent.com/ImanKari/G9JSONHandler/main/G9JSONHandler/Asset/AzureDevOpsPipelineBuildStatus.png?raw=true)](https://g9tm.visualstudio.com/G9JSONHandler/_apis/build/status/G9JSONHandler?branchName=main)
[![Github Repository](https://raw.githubusercontent.com/ImanKari/G9JSONHandler/main/G9JSONHandler/Asset/GitHub.png?raw=true)](https://github.com/ImanKari/G9JSONHandler)

## G9JSONHandler is a pretty small library for working with JSON.
### Sample Object
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

### Object To JSON
```csharp
TestObject testObject = new TestObject();
// Unformatted JSON
string unformattedJson = testObject.G9ObjectToJson();
// Formatted JSON
string formattedJson = testObject.G9ObjectToJson(true);
```
#### Result
```json
// Unformatted JSON Result
{"#__Comment0__#":"Custom JSON Comment","Name":".NET","Color":5,"Color2":"DarkGreen","MyRegisterDateTime":"09/01/1990 00:00:00","Time":"09:09:09","Array":["Item 1","Item 2","Item 3"],"Dictionary":{"Key 1":"Value 1","Key 2":"Value 2","Key 3":"Value 3"}}

// Formatted JSON Result
{
	"#__Comment0__#": "Custom JSON Comment",
	"Name": ".NET",
	"Color": 5,
	"Color2": "DarkGreen",
	"MyRegisterDateTime": "09/01/1990 00:00:00",
	"Time": "09:09:09",
	"Array": 
	[
		"Item 1",
		"Item 2",
		"Item 3"
	],
	"Dictionary": 
	{
		"Key 1": "Value 1",
		"Key 2": "Value 2",
		"Key 3": "Value 3"
	}
}
```

### JSON To Object

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
### Attributes

- **G9AttrJsonComment**
  - This attribute enables you to write several comments (notes) for each member in JSON.
    - Note: Indeed, JSON has no option for comments (notes). But this library considers two ways for that.
      1. **Standard mode** considers a custom member that consists of a key and value like the usual member item ("__ #CommentN __#": "Comment Data") and saves the comment note there.
      ```csharp
      [G9AttrJsonComment("Custom JSON Comment")] public string Name = ".NET";
      // {
      //  "#__Comment0__#": "Custom JSON Comment",
      //  "Name": ".NET",
      // ...
      ``` 
      2. **Nonstandard mode** saves comments notes between two signs ("/* Comment Data  /*") like JavaScript.
      ```csharp
      [G9AttrJsonComment("Custom JSON Comment", true)] public string Name = ".NET";
      // {
      //  /* Custom JSON Comment */
      //  "Name": ".NET",
      // ...
      ``` 
    - Note: This attribute can use several times for a member.
- **G9AttrJsonStoreEnumAsString**
  - This attribute enables you to store an Enum object as a string value in JSON (By default, an Enum object storing as a number).
- **G9AttrJsonCustomMemberName**
  - This attribute enables you to choose a custom name for a member for storing in JSON.
    - Note: At parsing time (JSON to object), the parser can recognize and pair the member automatically.
- **G9AttrJsonIgnoreMember**
  - This attribute enables you to ignore a member for storing in JSON.