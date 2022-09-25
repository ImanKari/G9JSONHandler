[![G9TM](https://raw.githubusercontent.com/ImanKari/G9JSONHandler/main/G9JSONHandler/Asset/G9JSONHandler.png)](http://www.g9tm.com/) **G9JSONHandler**

[![NuGet version (G9JSONHandler)](https://img.shields.io/nuget/v/G9JSONHandler.svg?style=flat-square)](https://www.nuget.org/packages/G9JSONHandler/)
[![Azure DevOps Pipeline Build Status](https://raw.githubusercontent.com/ImanKari/G9JSONHandler/main/G9JSONHandler/Asset/AzureDevOpsPipelineBuildStatus.png?raw=true)](https://g9tm.visualstudio.com/G9JSONHandler/_apis/build/status/G9JSONHandler?branchName=main)
[![Github Repository](https://raw.githubusercontent.com/ImanKari/G9JSONHandler/main/G9JSONHandler/Asset/GitHub.png?raw=true)](https://github.com/ImanKari/G9JSONHandler)

# G9JSONHandler
### A pretty small .NET library has been developed for working with JSON. This library provides many helpful attributes for members like [Comment](#g9attrcomment), [Encryption](#g9attrencryption), [CustomName](#g9attrcustomname), [Ordering](#g9attrorder), [Ignoring](#g9attrignore), [CustomParser](#g9attrcustomname), etc. On the other hand, with the [custom parser structure](#advanced), you can define your desired parsing process for specific types, or with a [preferred config](#object-to-json), you can [customize the parsing](#json-to-object-with-custom-config) process, which leads to more flexibility.

# ❇️Guide
## Implementation
In the first step, needs to consider a custom structure:
```csharp
using G9JSONHandler;
using G9JSONHandler.Attributes;

class TestObject
{
    [G9AttrComment("My Custom Comment")]
    public string Name = ".NET";
    public ConsoleColor Color = ConsoleColor.DarkMagenta;
    [G9AttrStoreEnumAsString]
    public ConsoleColor Color2 = ConsoleColor.DarkGreen;
    [G9AttrCustomName("MyRegisterDateTime")]
    public DateTime RegisterDateTime = new DateTime(1990, 9, 1);
    public TimeSpan Time = new TimeSpan(9, 9, 9);
    public string[] Array = { "Item 1", "Item 2", "Item 3" };
    public Dictionary<string, string> Dictionary = new Dictionary<string, string>()
        { { "Key 1", "Value 1" }, { "Key 2", "Value 2" }, { "Key 3", "Value 3" } };
    [G9AttrIgnore]
    public string Address = "...";
}
```
- Note: If you use properties in your structure, you must pay attention to their access modifiers. If they have a private setter, they can't receive their data from a JSON; on the other hand, if they have a private getter, they can't convert to JSON.
  ```csharp
  // public int Age { get; }
  // public int Age { set; }
  public int Age { set; get; }
  ```

## Object To JSON
The process of parsing an Object into a JSON string can be done with default configs and custom configs that are shown below:
```csharp
private static void Main()
{
    // Sample instance
    TestObject testObject = new TestObject();
    
    // JSON with default configs
    string unformattedJson = G9JSON.ObjectToJson(testObject);
    Console.WriteLine(unformattedJson);

    // JSON with custom configs + Formatted + Standard comments
    string formattedStandardJson = G9JSON.ObjectToJson(testObject,
        // Custom config structure for writer
        new G9DtJsonWriterConfig(
            // Specifies which access modifiers will include in the searching on members of an object.
            // By default, it's set to "BindingFlags.Instance | BindingFlags.Public," which means all members with public access and non-static.
            // On the other hand, you can use the shorter form of access modifiers implemented by Enum 'G9EAccessModifier'. 
            // accessibleModifiers: G9EAccessModifier.Public
            accessibleModifiers: BindingFlags.Instance | BindingFlags.Public,
            // Specifies that JSON result must be formatted or not.
            // By default, it's set to 'false.'
            isFormatted: true,
            // Specifies the type of comments in JSON structure.
            // Note: Indeed, JSON has no option for comments(notes).
            // In standard mode, this JSON library considers a custom member that consists of a key and value like the usual
            // member item ("#__CommentN__#": "Comment Data") and saves the comment note there.
            // In nonstandard mode, this JSON library saves comments notes between two signs ("/* Comment Data  /*").
            commentMode: G9ECommentMode.StandardMode
        )
    );
    Console.WriteLine(formattedStandardJson);

    // JSON with custom configs + Formatted + Nonstandard comments
    string formattedNonstandardJson = G9JSON.ObjectToJson(testObject,
        new G9DtJsonWriterConfig(
            accessibleModifiers: BindingFlags.Instance | BindingFlags.Public,
            isFormatted: true,
            commentMode: G9ECommentMode.NonstandardMode
        )
    );
    Console.WriteLine(formattedNonstandardJson);
}
```
## Object To JSON File And Vice Versa
Also, you can save a parsed object to a JSON file, and vice versa; you can read a JSON file and parse its data to your desired object directly.
```csharp
private static void Main()
{
    // Sample instance
    TestObject testObject = new TestObject();
    
    // Write
    G9JSON.ObjectToJsonFile(testObject, "Test.json");

    // Read
    var jsonObject = G9JSON.JsonFileToObject<TestObject>("Test.json");
}
```
## Results
### In the following can be seen the results of the above methods.
Unformatted JSON Result:
```json
{"#__Comment0__#":"My Custom Comment","Name":".NET","Color":5,"Color2":"DarkGreen","MyRegisterDateTime":"09/01/1990 00:00:00","Time":"09:09:09","Array":["Item 1","Item 2","Item 3"],"Dictionary":{"Key 1":"Value 1","Key 2":"Value 2","Key 3":"Value 3"}}
```
Formatted JSON Result + Standard comments: \
The standard comment mode is under all standards of JSON (RFC 8259/RFC 7159/RFC 4627/ECMA-404).
```json
{
	"#__Comment0__#": "My Custom Comment",
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
Formatted JSON Result + Nostandard comments: \
Please note: the nonstandard comment mode isn't under any standards of JSON (RFC 8259/RFC 7159/RFC 4627/ECMA-404), and maybe it causes a problem in other stacks. But if you use this library only for both sides (JSON to String and vice versa), it can handle this, and there isn't any problem.
```json
{
	/* My Custom Comment */
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

## JSON To Object
Also, the process of parsing a JSON string into an Object can be done with default configs and custom configs that are shown below:
```csharp
private static void Main()
{
  // Considers there is a JSON string that was parsed before:
  // var formattedJson = G9JSON.ObjectToJson(testObject, new G9DtJsonWriterConfig(isFormatted: true));

  // JSON To Object
  TestObject newObject = G9JSON.JsonToObject<TestObject>(formattedJson);
  Console.WriteLine(newObject.Name); // .NET
  Console.WriteLine(newObject.Color); // DarkMagenta
  Console.WriteLine(newObject.Array.Length); // 3
  Console.WriteLine(newObject.Dictionary.Count); // 3
  Console.WriteLine(newObject.Dictionary["Key 2"]); // Value 2
}
```
## JSON To Object With Custom Config
This scenario assumes a situation where the JSON data has mismatches in the values with the primary data type:
```csharp
private static void Main()
{
  // Please consider that the primary data type is like below:
  public class G9DtSmallSampleClass
  {
      public string Name;
      public int Age;
      public ConsoleColor Color;
  }

  // In continuation, the JSON data has been considered as below:
  var jsonData = "{\"Name\":32,\"Age\":\"Iman\",\"Color\":5}";
  
  // Notes:
  // In default process like below::
  //    var newObject = G9JSON.JsonToObject<G9DtSmallSampleClass>(jsonData);
  // An exception is thrown like this:
  //    ### An exception occurred when the parser tried to parse the value 'Iman' for member 'Age' in type '....G9DtSmallSampleClass'.###
  
  // But by setting the parameter 'ignoreMismatching' to 'true,'
  // it can specify that if in the parsing process a mismatch occurs,
  // the exception (mismatch) is ignored.
  G9DtSmallSampleClass newObject = G9JSON.JsonToObject<G9DtSmallSampleClass>(jsonData,
      // Custom config structure for parser
      new G9DtJsonParserConfig(
          // Specifies which access modifiers will include in the searching on members of an object.
          // By default, it's set to "BindingFlags.Instance | BindingFlags.Public," which means all members with public access and non-static.
          // On the other hand, you can use the shorter form of access modifiers implemented by Enum 'G9EAccessModifier'. 
          // accessibleModifiers: G9EAccessModifier.Public
          accessibleModifiers: BindingFlags.Instance | BindingFlags.Public,
          ignoreMismatching: true
        )
      );

  // Expected values:
  newObject.Name; // 32
  newObject.Color; // DarkMagenta
  newObject.Age; // 0 - default(int) - ignored in the mismatching process.
}
```
## Object To JSON File And Vice Versa
Also, you can save a parsed object to a JSON file, and vice versa; you can read a JSON file and parse its data to your desired object directly.
```csharp
private static void Main()
{
    // Sample instance
    TestObject testObject = new TestObject();
    
    // Write
    G9JSON.ObjectToJsonFile(testObject, "Test.json");

    // Read
    var jsonObject = G9JSON.JsonFileToObject<TestObject>("Test.json");
}
```
## Attributes

- ### **G9AttrComment**
  - This attribute enables you to write several comments (notes) for each member in JSON.
    - Note: Indeed, JSON has no option for comments (notes). But this library considers two ways for that.
      1. **Standard mode** considers a custom member that consists of a key and value like the usual member item ("__ #CommentN __#": "Comment Data") and saves the comment note there.
          - The standard comment mode is under all standards of JSON (RFC 8259/RFC 7159/RFC 4627/ECMA-404).
      ```csharp
      G9JSON.ObjectToJson(myObject, 
        new G9DtJsonWriterConfig(commentMode: G9ECommentMode.StandardMode));
      // {
      //  "#__Comment0__#": "My Custom Comment",
      // ...
      ``` 
      2. **Nonstandard mode** saves comments notes between two signs ("/* Comment Data  /*") like JavaScript.
          - the nonstandard comment mode isn't under any standards of JSON (RFC 8259/RFC 7159/RFC 4627/ECMA-404), and maybe it causes a problem in other stacks. But if you use this library only for both sides (JSON to String and vice versa), it can handle this, and there isn't any problem.
      ```csharp
      G9JSON.ObjectToJson(myObject, 
        new G9DtJsonWriterConfig(commentMode: G9ECommentMode.NonstandardMode));
      // {
      //  /* My Custom Comment */
      // ...
      ``` 
    - Note: This attribute can use several times for a member.
- ### **G9AttrStoreEnumAsString**
  - This attribute enables you to store an Enum object as a string value in JSON (By default, an Enum object storing as a number).
- ### **G9AttrCustomName**
  - This attribute enables you to choose a custom name for a member for storing in JSON.
    - Note: At parsing time (JSON to object), the parser can recognize and pair the member automatically.
- ### **G9AttrOrder**
  - This attribute enables you to specify the order of members of an object when they want to be written to a JSON structure.
    ```csharp
    public class Sample
    {
        [G9AttrOrder(3)]
        public int C = 3;
        [G9AttrOrder(2)]
        public int B = 2;
        [G9AttrOrder(1)]
        public int A = 1;
    }
    // Expected result:
    // {
    //  "A": 1,
    //  "B": 2,
    //  "C": 3,
    // }
    ``` 
- ### **G9AttrIgnore**
  - This attribute enables you to ignore a member for storing in JSON.
- ### **G9AttrCustomParser**
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
      [G9AttrCustomParser(typeof(CustomParser), nameof(CustomParser.StringToObject),
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
    - Note: The second parameter, 'G9IMemberGetter' in both parser methods, consists of helpful information about a member (field or property) in an object.
- ### **G9AttrEncryption**
  - This attribute enables you to add automated encrypting and decrypting processes for a member value.
  - Note: The specified member must have a convertible value to the string type.
  - Note: The priority of executing this attribute is higher than the others.
  - Note: If your member data type is complex, or you need to implement the custom encryption process, you can implement a custom (encryption/decryption) process with the attribute 'G9AttrCustomParser'.
  ```csharp
  // A class that consists of members with the attribute 'G9AttrEncryption'.
  // This attribute has several overloads.
  public class G9DtSampleClassForEncryptionDecryption
  {
      // With standard keys and default config
      [G9AttrEncryption("G-JaNdRgUkXp2s5v", "3t6w9z$C&F)J@NcR")]
      public string User = "G9TM";

      // With custom nonstandard keys and custom config
      [G9AttrEncryption("MyCustomKey", "MyCustomIV", PaddingMode.ANSIX923, CipherMode.CFB, enableAutoFixKeySize: true)]
      public string Password = "1990";

      // With custom nonstandard keys and custom config
      [G9AttrEncryption("MyCustomKey", "MyCustomIV", PaddingMode.ISO10126, CipherMode.ECB, enableAutoFixKeySize: true)]
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
**Important note:** The difference between the below structure and the attribute "G9AttrCustomParser" is that the mentioned attribute must be used on the desired member in a structure. But, by using this structure, if the parser finds the specified type in this structure, it automatically uses it (like the dependency injection process).

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
  public override string ObjectToString(ClassA objectForParsing, G9IMemberGetter accessToObjectMember, Action<string> addCustomComment)
  {
      addCustomComment("My custom comment 1");
      addCustomComment("My custom comment 2");
      addCustomComment("My custom comment 3");
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
- Note: The JSON core creates an instance from "CustomParserStructureForClassA" automatically. So, this class must not have a constructor with a parameter; otherwise, an exception is thrown.
- Note: Each type can have just one parser. An exception is thrown if you define a parser more than one for a type.
- Note: The second parameter, "**G9IMemberGetter**" in both methods, consists of helpful information about a member (field or property) in an object. If the object wasn't a member of another object (like the above example), these parameters have a null value.
- Note: The third parameter, "**addCustomComment**", is a callback action that sets a comment for the specified member if needed. Using that leads to making a comment before this member in the string structure. Using that is optional; it can be used several times or not used at all.
- **Notice: This parser type uses a created instance for all members with the specified type in an object. Its meaning is if you use some things in the body of the class (out of methods) like fields and properties, those things are used for all members with the specified type, and maybe a conflict occurs during parse time. To prevent this type of conflict, you must use another abstract class called 'G9ACustomTypeParserUnique<>'. For this type, per each member, a new instance is created and, after use, deleted (don't use it unless in mandatory condition because it has a bad performance in terms of memory usage and speed).**

### Defining the advanced parser for a specified (**generic**) type
**Important note:** The difference between the below structure and the attribute "G9AttrCustomParser" is that the mentioned attribute must be used on the desired member in a structure. But, by using this structure, if the parser finds the specified type in this structure, it automatically uses it (like the dependency injection process).

The abstract class '**G9ACustomGenericTypeParser**' enables you to define a custom parser for a specified **generic** type. \
Many parts of this structure are like the previous structure, with this difference that the target type for reacting (that is generic type) specified by inherited abstract class constructor. \
In addition, in this case, the parser methods receive and return generic objects as the object type (not generic type) that, like the below example or in your own way (with the reflections), you can handle them.
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
  public override string ObjectToString(object objectForParsing, Type[] genericTypes, G9IMemberGetter accessToObjectMember, Action<string> addCustomComment)
  {
      addCustomComment("My custom comment 1");
      addCustomComment("My custom comment 2");
      addCustomComment("My custom comment 3");

      var fields = G9Assembly.ObjectAndReflectionTools
        .GetFieldsOfObject(objectForParsing).ToDictionary(s => s.Name);
      return fields[nameof(G9CClassD<object>.A)].GetValue<string>() + "-" +
      fields[nameof(G9CClassD<object>.B)].GetValue();
  }
  // Method to parse string to specified generic object (ClassB<>).
  // The second parameter 'genericTypes', Specifies the type of generic parameters for target type.
  public override object StringToObject(string stringForParsing, Type[] genericTypes, G9IMemberGetter accessToObjectMember)
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
- Note: The third parameter, '**G9IMemberGetter**' in both methods, consists of helpful information about a member (field or property) in an object. If the object wasn't a member of another object (like the above example), these parameters have a null value.
- Note: The fourth parameter, "**addCustomComment**", is a callback action that sets a comment for the specified member if needed. Using that leads to making a comment before this member in the string structure. Using that is optional; it can be used several times or not used at all.
- **Notice: This parser type uses a created instance for all members with the specified type in an object. Its meaning is if you use some things in the body of the class (out of methods) like fields and properties, those things are used for all members with the specified type, and maybe a conflict occurs during parse time. To prevent this type of conflict, you must use another abstract class called 'G9ACustomGenericTypeParserUnique'. For this type, per each member, a new instance is created and, after use, deleted (don't use it unless in mandatory condition because it has a bad performance in terms of memory usage and speed).**

# END
## Be the best you can be; the future depends on it. 🚀