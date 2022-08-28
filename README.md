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
var jsonData = "{\"Name\":32,\"Age\":\"Iman\",\"Color\":5}";

// Default process:
// var newObject = formattedJson.G9JsonToObject<G9DtSmallSampleClass>();
// Note: Above process throws an exception like this:
// ### An exception occurred when the parser tried to parse the value 'Iman' for member 'Age' in type '....G9DtSmallSampleClass'.###
// But by setting the parameter 'ignoreMismatching' to 'true,' it can specify that if in the parsing process a mismatch occurs, the exception (mismatch) is ignored.
var newObject = jsonData
  // The 'ignoreMismatching' parameter is set to 'true.'
  .G9JsonToObject<G9DtSmallSampleClass>(true);
// Expected values
newObject.Name; // 32
newObject.Color; // DarkMagenta
newObject.Age; // 0 - default(int) - ignored in the mismatching process.
```

### Attributes

- **G9AttrJsonComment**
  - This attribute enables you to write several comments (notes) for each member in JSON.
    - Note: Indeed, JSON has no option for comments (notes). But this library considers two ways for that.
      1. **Standard mode** considers a custom member that consists of a key and value like the usual member item ("__ #CommentN __#": "Comment Data") and saves the comment note there.
      ```csharp
      [G9AttrJsonComment("Custom JSON Comment")] public string Name = ".NET";
      // Result:
      // {
      //  "#__Comment0__#": "Custom JSON Comment",
      //  "Name": ".NET",
      // ...
      ``` 
      2. **Nonstandard mode** saves comments notes between two signs ("/* Comment Data  /*") like JavaScript.
      ```csharp
      [G9AttrJsonComment("Custom JSON Comment", true)] public string Name = ".NET";
      // Result:
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
    - Note: The second parameter, 'G9IMemberGetter' in both methods, consists of helpful information about a member (field or property) in an object.
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
## Advanced
### Defining the advanced parser for a specified type
The abstract class '**G9ACustomTypeParser<>**' enables you to define a custom parser for a specified type (Any type like a built-in .NET type or custom definition type).\
This abstract class is a generic one where the generic parameter type specifies the type for parsing.\
In addition, this abstract class has two abstract methods for parsing the string to object and wise versa that must implement by the programmer.\
Furthermore, each class inherits by this abstract class is automatically used by JSON core (like a dependency injection process).
```csharp
// Sample Class
public class ClassA
{
    public string A = "G9";
    public int B = 9;
}

// Custom parser structure for ClassA
// The target type must be specified in generic parameter 'G9ACustomTypeParser<ClassA>'
public class CustomParserStructureForClassA : G9ACustomTypeParser<ClassA>
{
  // Method to parse specified object (ClassA) to string.
  public override string ObjectToString(ClassA objectForParsing, G9IMemberGetter accessToObjectMember)
  {
      return objectForParsing.A + "TM-" + (objectForParsing.B - 3);
  }
  // Method to parse string to specified object (ClassA).
  public override ClassA StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember)
  {
      var data = stringForParsing.Split("-");
      return new ClassA()
      {
          A = data[0],
          B = int.Parse(data[1])
      };
  }
}

// Usage
var object = new ClassA();
var jsonData = object.G9ObjectToJson(); // "{\"G9TM-6\"}"
var objectData = jsonData.G9JsonToObject<ClassA>();
objectData.A; // "G9TM"
objectData.B; // 6
```
- Note: The JSON core creates an instance from 'CustomParserStructureForClassA' automatically. So, this class must not have a constructor with a parameter; otherwise, an exception is thrown.
- Note: Each type can have just one parser. An exception is thrown if you define a parser more than one for a type.
- Note: The second parameter, '**G9IMemberGetter**' in both methods, consists of helpful information about a member (field or property) in an object. If the object wasn't a member of another object (like the above example), these parameters have a null value.
- **Notice: This parser type uses a created instance for all members with the specified type in an object. Its meaning is if you use some things in the body of the class (out of methods) like fields and properties, those things are used for all members with the specified type, and maybe a conflict occurs during parse time. To prevent this type of conflict, you must use another abstract class called 'G9ACustomTypeParserUnique<>'. For this type, per each member, a new instance is created and, after use, deleted (don't use it unless in mandatory condition because it has a bad performance in terms of memory usage and speed).**

### Defining the advanced parser for a specified (**generic**) type
The abstract class '**G9ACustomGenericTypeParser**' enables you to define a custom parser for a specified **generic** type. Many parts of this structure are like the previous structure, with this difference that the target type for reacting (that is generic type) specified by inherited abstract class constructor.
```csharp
// Sample Class
public class ClassB<TType>
{
    public string A = "G9";
    public TType B;
}

// Custom parser structure for generic ClassB<>
public class CustomParserStructureForClassB : G9ACustomGenericTypeParser
{
  public CustomParserStructureForClassB() 
    // The target type in this case must be specified in inherited constructor like this
    : base(typeof(ClassB<>))
  {
  }
  // Method to parse specified generic object (ClassB<>) to string.
  // The second parameter 'genericTypes', Specifies the type of generic parameters for target type.
  public override string ObjectToString(object objectForParsing, Type[] genericTypes, G9IMemberGetter accessToObjectMember)
  {
      var fields = G9Assembly.ObjectAndReflectionTools
        .GetFieldsOfObject(objectForParsing).ToDictionary(s => s.Name);
      return fields[nameof(G9CClassD<object>.A)].GetValue<string>() + "-" +
      fields[nameof(G9CClassD<object>.B)].GetValue();
  }
  // Method to parse string to specified generic object (ClassB<>).
  // The second parameter 'genericTypes', Specifies the type of generic parameters for target type.
  public override ClassB StringToObject(string stringForParsing, Type[] genericTypes, G9IMemberGetter accessToObjectMember)
  {
      var data = stringForParsing.Split("-");
      return new ClassB<string>()
      {
          A = data[0],
          B = data[1]
      };
  }
}

// Usage
var object = new ClassB<string>();
object.B = "None";
var jsonData = object.G9ObjectToJson(); // "{\"G9-None\"}"
var objectData = jsonData.G9JsonToObject<ClassB<string>>();
objectData.A; // "G9TM"
objectData.B; // "None"
```
- Note: The JSON core creates an instance from 'CustomParserStructureForClassB' automatically. So, this class must not have a constructor with a parameter; otherwise, an exception is thrown.
- Note: Each type can have just one parser. An exception is thrown if you define a parser more than one for a type.
- Note: The second parameter, '**G9IMemberGetter**' in both methods, consists of helpful information about a member (field or property) in an object. If the object wasn't a member of another object (like the above example), these parameters have a null value.
- **Notice: This parser type uses a created instance for all members with the specified type in an object. Its meaning is if you use some things in the body of the class (out of methods) like fields and properties, those things are used for all members with the specified type, and maybe a conflict occurs during parse time. To prevent this type of conflict, you must use another abstract class called 'G9ACustomGenericTypeParserUnique'. For this type, per each member, a new instance is created and, after use, deleted (don't use it unless in mandatory condition because it has a bad performance in terms of memory usage and speed).**
