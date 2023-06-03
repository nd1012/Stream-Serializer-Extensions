# Stream-Serializer-Extensions

This .NET library extends any `Stream` object with serializing methods for 
writing almost any object binary serialized to a stream, and deserializing any 
binary stream sequence.

The built in serializer supports binary serialization of 

- booleans
- numbers ((U)Int8-64, Single, Double, Decimal, little endian)
- enumerations
- strings (UTF-8/16/32, little endian)
- arrays
- lists
- dictionaries
- byte arrays
- structures
- possibly any other objects with a parameterless public constructor
- streams

**NOTE**: Arrays, lists and dictionaries with nullable values aren't supported.

It's possible to override the build in serializers, and to add custom type 
serializers, too.

## Methods

The `Write` and `WriteAsync` methods will be extended with supported types, 
while serializing some types is being done with specialized methods:

| Type | Serialization method | Deserialization method |
| --- | --- | --- |
| `string` | `WriteString*` | `ReadString*` |
| `Enum` | `WriteEnum*` | `ReadEnum*` |
| `Array` | `Write*Array*` | `Read*Array*` |
| `List<T>` | `WriteList*` | `ReadList*` |
| `Dictionary<tKey, tValue>` | `WriteDict*` | `ReadDict*` |
| `IStreamSerializer` | `WriteSerialized*` | `ReadSerialized*` |
| `byte[]` | `WriteBytes*` | `ReadBytes*` |
| `Stream` | `WriteStream*` | `ReadStream*` |
| Structure | `WriteStruct*` | `ReadStruct*` |
| (any other) | `WriteAnyObject*` | `ReadAnyObject*` |

Using the `WriteObject*` and `ReadObject*` methods you can let the library 
decide which method to use for the given object type.

Using the `WriteAny*` and `ReadAny*` methods, you can write and read an object 
with a dynamic type.

Using the `WriteAnyObject*` and `ReadAnyObject*` methods you can also 
(de)serialize objects which have a constructor without parameters. The 
serializer will process public properties which have a public getter and 
setter. If in the future the object changes, it's not possible to deserialize 
an older binary sequence, unless you work with the `StreamSerializerAttribute` 
and set an object version to the type and its properties.

**NOTE**: Please use the `*Nullable*` methods for working with nullables. 
They'll add an extra byte for `null` value detection.

**NOTE**: In general you should use the opposite method for reading a binary 
sequence that you've used for writing it!

Most methods are designed specially for one type, while other methods work 
more generic. This is when to choose which method:

| Method | Condition |
| --- | --- |
| `*Serialized*` | The fixed type is a stream serializer base object |
| `*Object*` | The fixed type has a specialized serializer |
| `*Struct*` | The fixed type is a marshalable structure |
| `*AnyObject*` | The type uses attributes (or no serializer contract information at all) and doesn't have a specialized serializer |
| `*Any*` | The dynamic type is unknown when (de)serializing |

## Number serialization

The `WriteNumber*` and `ReadNumber*` methods will find the best matching 
serialization method for a given number. For example, if you give an Int32 
value which could be fit into an UInt16, the value will be converted for 
serialization, and you can save one byte (because the methods will store the 
used numeric type in an extra byte).

**NOTE**: All numbers will be serialized using little endian.

## Embedded streams

Using `WriteStream*` and `ReadStream*` you can embed a stream in serialized 
data. Seekable streams will just be copied, having their length as header, 
while non-seekable streams will be embedded chunked.

When deserializing an embedded stream using generic read-methods (like 
`ReadObject` or `ReadAny`), a temporary `FileStream` will be created, which 
will delete the temporary file when disposing. You can define a stream factory 
using `StreamSerializerAttribute.StreamFactoryType` and 
`StreamSerializerAttribute.StreamFactoryMethod`.

## Structure serialization

The `WriteStruct*` methods use `Marshal` for structure serialization, which 
produce little or big endian bits depending on the processors endianess. To 
ensure that a serialized structure can be deserialized on any system, 
endianess will be converted to little endian per default. For this it's 
required to add a `StreamSerializerAttribute` to the strcture type and all 
fields which require endianess conversion, which are

- numeric types (which include enumeration values)
- sub-structures which host numeric type fields

**NOTE**: The serializer uses reflection for this only once (for 
initialization) only on system with a big endian processor, which shouldn't 
have too much performance impact for a permanent running service.

**CAUTION**: Only structure fields will be processed. Their value should be a 
structure, too - object references will make problems and are not supported. 
Also variable length value counters won't be converted, which limits the 
endianess conversion support to structures which match to these limitations 
only:

- No object referencing fields
- Fixed length contents

**TIP**: A structure may implement the `IStreamSerializer` inferface and be 
written using the `WriteSerialized*` methods. For reading you can use the 
`ReadSerializedStruct*` methods.

## Custom serializer

### Using the `StreamSerializerAttribute` attribute

The type needs to have a constructor without parameters. Properties with a 
public getter and setter can be serialized:

```cs
[StreamSerializer(StreamSerializerModes.OptOut)]
public class YourType
{
	public YourType() { }
	
	public string Serialized { get; set; } = null!;
	
	[StreamSerializer(1)]
	public string? NotSerialized { get; set; }
}

stream.WriteAnyObject(new YourType(){ Serialized = "Test1", NotSerialized = "Test2" });
stream.Position = 0;
YourType deserialized = stream.ReadAnyObject<YourType>();
Assert.AreEqual(deserialized.Serialized, "Test1");
Assert.IsNull(deserialized.NotSerialized);
```

Using the `StreamSerializerAttribute` attribute at a type, you can set the 
serializer mode to `OptOut` (the default when not using the attribute) or 
`OptIn`. `OptOut` includes all properties, except the ones with a 
`StreamSerializerAttribute` (if the attribute doesn't `OptIn`). `OptIn` only 
includes properties with a `StreamSerializerAttribute`, except the attribute 
mode was set to `OptOut`.

To skip property name checksums, which require one extra byte per serialized 
property, you can set the `SkipPropertyNameChecksum` property value of the 
attribute to `true`.

You can use a property versioning, which supports skipping newer properties 
when deserializing an older binary sequence. The type attribute defines the 
object version, while the property attributes define the object version in 
which they (dis)appear.

By setting a property value position, you can modify the order of the value 
within the binary sequence. This ordering will be applied:

- `Position`
- `Name`

To exclude a property depending on the object version:

- `FromVersion`: First object version which **includes** the property 
(optional)
- `Version`: Last object version which **includes** the property (optional)

You can set some details for the deserializer methods using these properties:

- `OptionsType`: Individual serializer options type to use
- `KeyOptionsType`: Individual key serializer options type to use
- `ValueOptionsType`: Individual value serializer options type to use
- `MinLen`: Minimum length
- `Maxlen`: Maximum length

The attribute type can be extended, methods are virtual.

#### Stream factory

If you use the attribute on a `Stream` property, you can define a stream 
factory using the `StreamFactoryType` and `StreamFactoryMethod`, or you set a 
`StreamFactory_Delegate` to the `StreamFactory` property (using reflections).

You can use the `StreamSerializerAttribute.MemoryStreamFactory` method as 
memory stream factory.

#### Serializer options factory

For automatic object deserialization the deserializer will try to get an 
`ISerializerOptions` object, which is being used to provide deserialization 
details to the deserializer method. You may define a serializer options 
factory using the `SerializerOptionsFactoryType` and 
`SerializerOptionsFactoryMethod` properties, or you set the serializer options 
to the `SerializerOptions` property (using reflections).

The same applies to

- `KeySerializerOptions`: Used by deserializer methods which deserialize a 
dictionary, for example
- `ValueSerializerOptions`: Used by deserializer methods which deserialize a 
dictionary, array or list, for example

The `OptionsType`, `KeyOptionsType` and `ValueOptionsType` properties are used 
when no factory method was defined. Those types need to implement a 
constructor which takes a `PropertyInfo?` and a `StreamSerializerAttribute?` 
parameter.

### Automatic serializable objects

You can extend from the `(Disposable)AutoStreamSerializerBase` type, if you 
fully implement serialization using the `StreamSerializerAttribute` for the 
final type and its properties, and match these pre-requirements:

1. The `StreamSerializerAttribute` of the final type requires an object 
version number in the `Version` property
2. The stream serializer mode must be `OptIn`

The auto stream serializer fully relies on a correct use of the 
`StreamSerializerAttribute` and object versioning for both, the type and its 
serialized properties.

If a serialized object property has an unchanged default value, the serializer 
will only write a flag which is telling the deserializer not to deserialize a 
value. If you don't want that, you can disable defult value flags for the 
whole type by setting the `StreamSerializerAttribute.UseDefaultValues` value 
to `false` (this property is also evaluated per property). To make a decision 
for each property based on the object version, you can create your own 
`StreamSerializerAttribute` and override the `GetUseDefaultValue` method, 
which per default simply returns the `UseDefaultValues` flag.

### Extending the `StreamSerializer`

```cs
StreamSerializer.SyncSerializer.AddOrUpdate(
    typeof(YourType),
    (stream, value) => 
    {
        // Serialize value to stream
    }
);
StreamSerializer.AsyncSerializer.AddOrUpdate(
    typeof(YourType),
    async (stream, value, cancellationToken) => 
    {
        // Serialize value to stream
    }
);
StreamSerializer.SyncDeserializer.AddOrUpdate(
    typeof(YourType),
    (stream, type, version) => 
    {
        // Deserialize value from stream
        return value;
    }
);
StreamSerializer.AsyncDeserializer.AddOrUpdate(
    typeof(YourType),
    YourType.DeserializeAsync
);

public class YourType
{
    ...

    public static async Task<YourType> DeserializeAsync(Stream stream, Type type, int version, CancellationToken cancellationToken)
    {
        // Deserialize value
        return value;
    }
}

// Then you can (de)serialize like this:
stream.WriteObject(new YourType());
stream.Position = 0;
YourType instance = stream.ReadObject<YourType>();
```

**NOTE**: The asynchronous  deserializer delegate uses a `Task` return type, 
while internal the task will be converted to `Task<YourType>`, to get the 
result. Because there's a lack of support for generic delegates, this seems to 
be the only way to go :(

**NOTE**: You can attach to the `StreamSerializer.OnInit` event to add your 
custom type serializers on start. During initialization, the 
`StreamSerializer.SyncObject` object will be thread locked, and you shouldn't 
use the `Find*` methods.

**NOTE**: You can use the `SerializerHelper` class methods for boiler plate 
tasks like ensuring a non-null value, or validating a deserialized length 
value, for example.

**NOTE**: You should throw a `SerializerException` on any (de)serializing 
issue!

### Using the `IStreamSerializer` interface

Your object can implement the `IStreamSerializer` interface or use the 
`StreamSerializerBase` base class, which implements the `IStreamSerializer` 
interface. Then you can use the `WriteSerialized*`, `WriteObject*`, 
`ReadSerialized*` and `ReadObject*` methods for (de)serialization.

```cs
public class YourType : StreamSerializerBase
{
    public YourType() : base() { }

    public YourType(Stream stream, int version) : base(stream, version) { }

    protected override void Serialize(Stream stream)
    {
        ...
    }

    protected override void Deserialize(Stream stream, int version)
    {
        ...
    }
}
```

When deserializing using the `ReadAny*` methods, the target type needs to be 
loaded from the environment. You can add your own type loading handler using 
the `StreamSerializer.OnLoadType` event. The library uses the `wan24-Core` 
NuGet package. If you want to use the `wan24-Core` type helper for loading 
types:

```cs
StreamSerializer.OnInit += (e) => StreamSerializer.OnLoadType += (s, e) =>
{
    if(e.Type != null) return;
    e.Type = TypeHelper.Instance.GetType(e.Name);
};
```

**CAUTION**: By adding the `wan24-Core` type helper like this, any type may be 
deserialized, which ~~may be~~ is a security issue!

When using the `StreamSerializerBase` base class, you can also give a value 
for the parameter `objectVersion` to the base constructor to enable object 
versioning. This makes it possible that newer object versions are able to 
deserialize from older binary sequences, and it ensures that old object 
versions can't deserialize from a newer binary sequence. During 
deserialization you can get the serialized object version like this:

```cs
    ...
    public const int OBJECT_VERSION = 1;

    public YourType() : base(objectVersion: OBJECT_VERSION) { }

    public YourType(Stream stream, int version) : base(stream, version, objectVersion: OBJECT_VERSION) { }

    ...

    protected override void Deserialize(Stream stream, int version)
    {
        int serializedVersion = ((IStreamSerializer)this).SerializedObjectVersion!;
        if(serializedVersion < 1) throw new SerializerExeption($"Unsupported {GetType()} binary sequence version #{serializedVersion}");
        ...
    }

    ...
```

The `SerializedObjectVersion` property will have a non-null value, if the 
object was deserialized, and the `StreamSerializerBase` base constructor got a 
object version as `objectVersion` parameter. Based on the serialized object 
version you can switch and handle the binary sequence in the required way. To 
access the versioning information of an object, you can use the optional 
`IStreamSerializerVersion` interface, which is implemented by 
`StreamSerializerBase`, too.

## Deserializer limitations

When deserializing variable length objects (like arrays or strings), you can 
limit the allowed number of items/bytes (or request a minimum count) using the 
specific (de)serializer methods, by giving `minLen` and `maxLen` parameter 
values.

## Serializer version

The `StreamSerializer.Version` property holds the serializer version 
information, which you may write at the beginning of a serialized stream. The 
first byte (values 0-255) is used by the stream serializer internal. If you'd 
like custom versioning, please use bytes 2 to 4 (values 256+) for your own 
version number (which you can make more readable by bit shifting the value). 
The stream serializer extensions will only use the first 8 bits to identify a 
serializer version number, which can be given to all deserializer methods, 
while the `StreamSerializer.Version` value is the default serializer version 
number to use in case no version parameter was given to a deserializer method.

**NOTE**: The serializing methods will always use the latest binary sequence 
format version when writing, no matter which value was set to 
`StreamSerializer.Version`!

## Binary sequence size

Since the serializer may be used without any configuration and contract 
information, it tries to create the smallest binary sequence size which still 
offers enough information for an also unconfigured deserialization without 
contracts. The currently resulting sequence size is a trade off between size 
and usability. By defining customized type serializers you can optimize the 
resulting sequence size for any type. Keep in mind, that using the most 
specific (de)serializing methods will result in smaller binary sequences.

## Stream object enumeration

You can enumerate serialized objects from any stream like this:

```cs
foreach(AnyType obj in stream.EnumerateSerialized<AnyType>())
{
    ...
}

await foreach(AnyType obj in stream.EnumerateSerializedAsync<AnyType>())
{
    ...
}
```

In this example it's assumed that `AnyType` implements `IStreamSerializer`.

For enumerating any other type, you can implement an enumerator using the 
`StreamEnumeratorBase` and `StreamAsyncEnumeratorBase` base classes. The only 
thing that you'll have to do is to override the `ReadObject(Async)` method, 
which finally reads the next object to yield from the `Stream`. Then you can 
enumerate easily using the static `Enumerate(Async)` methods of the base types.

This is a sample `bool` enumerator implementation, which uses the `ReadBool` 
method:

```cs
public class StreamBoolEnumerator : StreamEnumeratorBase<bool>
{
    public StreamBoolEnumerator(Stream stream, int? version = null)
        :base(stream, version)
        { }

    protected override int ReadObject() => Stream.ReadBool(SerializerVersion);
}
```

To provide the enumerator as a stream extension method:

```cs
public static IEnumerable<bool> EnumerateBool(this Stream stream, int? version = null)
    => StreamBoolEnumerator.Enumerate<StreamBoolEnumerator>(stream, version);
```

These enumerators are implemented at present:

| Type | Enumerator | Serializer method | Stream extension |
| --- | --- | --- | --- |
| `IStreamSerializer` | `StreamSerializer(Async)Enumerator` | `ReadSerialized(Async)` | `EnumerateSerialized(Async)` |
| Numeric types | `StreamNumber(Async)Enumerator` | `ReadNumber(Async)` | `EnumerateNumber(Async)` |
| `string` | `StreamString(Async)Enumerator` | `ReadString(Async)` | `EnumerateString(Async)` |

## Security

The base serializer supports basic types and lists. Especially when 
deserializing lists, you should define a minimum and a maximum length.

Per default all objects that you want to deserialize using a `ReadAnyObject*` 
method, the type is required to use the `StreamSerializerAttribute` for 
security reasons. If you need to allow all types, you can set the 
`StreamExtensions.AnyObjectRequireAttribute` property value to `false`.

**CAUTION**: If you allow deserialization of any type, deserializing a 
manipulated input stream could harm your computer!

**CAUTION**: During serialization it's possible to end up in an endless 
recursion, if any nested property serves an object which is in the current 
stack already.

**CAUTION**: If you don't use versioning, you may end up in broken binary 
sequences which can't be deserialized anymore. Also a deserialization attempt 
could harm your computer!

The job of the serializer is to write and read objects to/from a binary 
sequence. There's no compression, encryption or hashing built in. If you want 
to compress/protect a created binary sequence, you can apply compression, 
encryption and hashing on the result as you want.

Object validation will be applied to deserialized objects to ensure their 
validity.
