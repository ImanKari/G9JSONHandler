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
    [G9AttrJsonMemberCustomName("MyRegisterDateTime")]
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
### JSON To Object (with mismatching)

```csharp
// Considered a situation where the JSON data has some mismatching value with the primary data type.
// For example, the primary data type is:
public class G9DtSmallSampleClass
{
    public string Name;
    public int Age;
    public ConsoleColor Color;
}

// The json data is:
var json = "{\"Name\":32,\"Age\":\"Iman\",\"Color\":5}";

// Default process:
// var newObject = formattedJson.G9JsonToObject<G9DtSmallSampleClass>();
// Note: Above process throws an exception like this:
// ### An exception occurred when the parser tried to parse the value 'Iman' for member 'Age' in type '....G9DtSmallSampleClass'.###
// But by setting the parameter 'ignoreMismatching,' it can specify that if in the parsing process a mismatch occurs, the exception (mismatch) is ignored.
var newObject = formattedJson.G9JsonToObject<G9DtSmallSampleClass>(true);
newObject.Name; // 32
newObject.Color; // DarkMagenta
newObject.Age; // 0 - default(int)
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
- **G9AttrJsonMemberCustomName**
  - This attribute enables you to choose a custom name for a member for storing in JSON.
    - Note: At parsing time (JSON to object), the parser can recognize and pair the member automatically.
- **G9AttrJsonIgnoreMember**
  - This attribute enables you to ignore a member for storing in JSON.
- **G9AttrJsonMemberCustomParser**
  - This attribute enables you to implement the custom parsing process for (String to Json, Json to String, Both of them).
  ```csharp
  // A class that consists of custom parser methods.
  public class CustomParser
  {
      // Custom parser for parsing the object to string.
      // Note: The specified method must have two parameters (the first parameter is 'string' and the second one is 'G9IMemberGetter');
      // in continuation, it must return an object value that is parsed from the string value.
      public string ObjectToString(object objectForParsing, G9IMemberGetter accessToObjectMember)
      {
          if (accessToObjectMember.MemberType == typeof(CustomChildObject))
              return ((CustomChildObject)objectForParsing).Number1 + "-" +
                    ((CustomChildObject)objectForParsing).Number2 +
                    "-" + ((CustomChildObject)objectForParsing).Number3;
          return default;
      }

      // Custom parser for parsing the string to object.
      // Note: The specified method must have two parameters (the first parameter is 'object' and the second one is 'G9IMemberGetter');
      // in continuation, it must return a string value that is parsed from the object value.
      public object StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
      {
          if (accessToObjectMember.MemberType != typeof(CustomChildObject))
              return default;
          var numberData = stringForParsing.Split('-').Select(int.Parse).ToArray();
          return new CustomChildObject
          {
              Number1 = numberData[0],
              Number2 = numberData[1],
              Number3 = numberData[2]
          };
      }
  }

  // Custom object
  public class CustomObject
  {
      // This attribute has several overloads.
      // The popular way (used below), for use, must specify the type of custom parse class in the first parameter,
      // the second parameter specifies the string to object method name, and the last one specifies the object to string method name.
      [G9AttrJsonMemberCustomParser(typeof(CustomParser), nameof(CustomParser.StringToObject),
          nameof(CustomParser.ObjectToString))]
      public CustomChildObject CustomChild = new();
  }

  // Custom child object
  public class CustomChildObject
  {
      public int Number1 = 9;
      public int Number2 = 8;
      public int Number3 = 7;
  }
    ``` 
- **G9AttrJsonMemberEncryption**
  - This attribute enables you to add automated encrypting and decrypting processes for a member value.
  - Note: The specified member must have a convertible value to the string type.
  - Note: The priority of executing this attribute is higher than the others.
  - Note: If your member data type is complex, or you need to implement the custom encryption process, you can implement a custom (encryption/decryption) process with the attribute 'G9AttrJsonMemberCustomParser'.
  ```csharp
  // A class that consists of members with the attribute 'G9AttrJsonMemberEncryption'.
  // This attribute has several overloads.
  public class G9DtSampleClassForEncryptionDecryption
  {
      // With standard keys and default config
      [G9AttrJsonMemberEncryption("G-JaNdRgUkXp2s5v", "3t6w9z$C&F)J@NcR")]
      public string User = "G9TM";

      // With custom nonstandard keys and custom config
      [G9AttrJsonMemberEncryption("MyCustomKey", "MyCustomIV", PaddingMode.ANSIX923, CipherMode.CFB, enableAutoFixKeySize: true)]
      public string Password = "1990";

      // With custom nonstandard keys and custom config
      [G9AttrJsonMemberEncryption("MyCustomKey", "MyCustomIV", PaddingMode.ISO10126, CipherMode.ECB, enableAutoFixKeySize: true)]
      public DateTime Expire = DateTime.Now;
  }

  var objectWithEncryptionAttr = new G9DtSampleClassForEncryptionDecryption();

  // Encryption value
  var jsonData = objectWithEncryptionAttr.G9ObjectToJson();
  // Result
  //{
  //  "User": "fESJe1TvMr00Q7BKTwVadg==",
  //  "Password": "sWNkdxQ=",
  //  "Expire": "C/WjS9oA+FRLw3myST4EowLiM22tTidXoG7hgJy3ZHo="
  //}
  var objectData = jsonData.G9JsonToObject<G9DtSampleClassForEncryptionDecryption>();
  objectData.User; // "G9TM"
  objectData.Password; // "1990"
  ```
