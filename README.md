## This is a .NET 6 port from https://github.com/mstefarov/fNbt

Package may be manually uploaded to NuGet. Create issue if you have a problem.

[Named Binary Tag (NBT)](https://minecraft.gamepedia.com/NBT_format) is a structured binary file format used by Minecraft.
fNbt is a small library, written in C# for .NET 3.5+. It provides functionality
to create, load, traverse, modify, and save NBT files and streams.

Current released version is 0.6.4 (6 July 2018).

fNbt is based in part on Erik Davidson's (aphistic's) original LibNbt library,
now completely rewritten by Matvei Stefarov (fragmer).

Note that fNbt.Test.dll and nunit.framework.dll do NOT need to be bundled with
applications that use fNbt; they are only used for testing.


## FEATURES
- Load and save uncompressed, GZip-, and ZLib-compressed files/streams.
- Easily create, traverse, and modify NBT documents.
- Simple indexer-based syntax for accessing compound, list, and nested tags.
- Shortcut properties to access tags' values without unnecessary type casts.
- Compound tags implement `ICollection<T>` and List tags implement `IList<T>`, for easy traversal and LINQ integration.
- Good performance and low memory overhead.
- Built-in pretty-printing of individual tags or whole files.
- Every class and method are fully documented, annotated, and unit-tested.
- Can work with both big-endian and little-endian NBT data and systems.
- Optional high-performance reader/writer for working with streams directly.


## DOWNLOAD
Latest version of fNbt requires .NET Framework 3.5+ (client or full profile).

- **Package @ NuGet:**  https://www.nuget.org/packages/fNbt-netcore

## EXAMPLES
#### Loading a gzipped file
```cs
    var myFile = new NbtFile();
    myFile.LoadFromFile("somefile.nbt.gz");
    var myCompoundTag = myFile.RootTag;
```

#### Accessing tags (long/strongly-typed style)
```cs
    int intVal = myCompoundTag.Get<NbtInt>("intTagsName").Value;
    string listItem = myStringList.Get<NbtString>(0).Value;
    byte nestedVal = myCompTag.Get<NbtCompound>("nestedTag")
                              .Get<NbtByte>("someByteTag")
                              .Value;
```

#### Accessing tags (shortcut style)
```cs
    int intVal = myCompoundTag["intTagsName"].IntValue;
    string listItem = myStringList[0].StringValue;
    byte nestedVal = myCompTag["nestedTag"]["someByteTag"].ByteValue;
```

#### Iterating over all tags in a compound/list
```cs
    foreach( NbtTag tag in myCompoundTag.Values ){
        Console.WriteLine( tag.Name + " = " + tag.TagType );
    }
    foreach( string tagName in myCompoundTag.Names ){
        Console.WriteLine( tagName );
    }
    for( int i = 0; i < myListTag.Count; i++ ){
        Console.WriteLine( myListTag[i] );
    }
    foreach( NbtInt intItem in myIntList.ToArray<NbtInt>() ){
        Console.WriteLine( intItem.Value );
    }
```

#### Constructing a new document
```cs
    var serverInfo = new NbtCompound("Server");
    serverInfo.Add( new NbtString("Name", "BestServerEver") );
    serverInfo.Add( new NbtInt("Players", 15) );
    serverInfo.Add( new NbtInt("MaxPlayers", 20) );
    var serverFile = new NbtFile(serverInfo);
    serverFile.SaveToFile( "server.nbt", NbtCompression.None );
```

#### Constructing using collection initializer notation
```cs
    var compound = new NbtCompound("root"){
        new NbtInt("someInt", 123),
        new NbtList("byteList") {
            new NbtByte(1),
            new NbtByte(2),
            new NbtByte(3)
        },
        new NbtCompound("nestedCompound") {
            new NbtDouble("pi", 3.14)
        }
    };
```

#### Pretty-printing file structure
```cs
    Console.WriteLine( myFile.ToString("\t") ); // tabs
    Console.WriteLine( myRandomTag.ToString("    ") ); // spaces
```

#### Check out unit tests in fNbt.Test for more examples.


## API REFERENCE
Online reference can be found at http://www.fcraft.net/fnbt/v0.6.4/


## LICENSING
fNbt v0.5.0+ is licensed under 3-Clause BSD license;
see [docs/LICENSE](docs/LICENSE).
LibNbt2012 up to and including v0.4.1 kept LibNbt's original license (LGPLv3).


## VERSION HISTORY
See [docs/Changelog.md](docs/Changelog.md)


## OLD VERSIONS
If you need .NET 2.0 support, stick to using fNbt version 0.5.1.
Note that this 0.5.x branch of fNbt is no longer supported or updated.

- **Compiled binary:**  https://fcraft.net/fnbt/fNbt_v0.5.1.zip

- **Amalgamation** (single source file):
    - Non-annotated: https://fcraft.net/fnbt/fNbt_v0.5.1.cs
    - Annotated: https://fcraft.net/fnbt/fNbt_v0.5.1_Annotated.cs
