﻿using System;
using System.IO;
using fNbt.Tags;
using NUnit.Framework;

namespace fNbt.Tests;

[TestFixture]
public class NbtFileTests
{
    [SetUp]
    public void NbtFileTestSetup()
    {
        Directory.CreateDirectory(TestDirName);
    }


    [TearDown]
    public void NbtFileTestTearDown()
    {
        if (Directory.Exists(TestDirName))
        {
            foreach (var file in Directory.GetFiles(TestDirName))
            {
                File.Delete(file);
            }

            Directory.Delete(TestDirName);
        }
    }

    private const string TestDirName = "NbtFileTests";

    [Test]
    public void TestNbtSmallFileLoadingUncompressed()
    {
        var file = new NbtFile(TestFiles.Small);
        Assert.AreEqual(TestFiles.Small, file.FileName);
        Assert.AreEqual(NbtCompression.None, file.FileCompression);
        TestFiles.AssertNbtSmallFile(file);
    }


    [Test]
    public void LoadingSmallFileGZip()
    {
        var file = new NbtFile(TestFiles.SmallGZip);
        Assert.AreEqual(TestFiles.SmallGZip, file.FileName);
        Assert.AreEqual(NbtCompression.GZip, file.FileCompression);
        TestFiles.AssertNbtSmallFile(file);
    }


    [Test]
    public void LoadingSmallFileZLib()
    {
        var file = new NbtFile(TestFiles.SmallZLib);
        Assert.AreEqual(TestFiles.SmallZLib, file.FileName);
        Assert.AreEqual(NbtCompression.ZLib, file.FileCompression);
        TestFiles.AssertNbtSmallFile(file);
    }

    [Test]
    public void LoadingBigFileUncompressed()
    {
        var file = new NbtFile();
        var length = file.LoadFromFile(TestFiles.Big);
        TestFiles.AssertNbtBigFile(file);
        Assert.AreEqual(length, new FileInfo(TestFiles.Big).Length);
    }


    [Test]
    public void LoadingBigFileGZip()
    {
        var file = new NbtFile();
        var length = file.LoadFromFile(TestFiles.BigGZip);
        TestFiles.AssertNbtBigFile(file);
        Assert.AreEqual(length, new FileInfo(TestFiles.BigGZip).Length);
    }


    [Test]
    public void LoadingBigFileZLib()
    {
        var file = new NbtFile();
        var length = file.LoadFromFile(TestFiles.BigZLib);
        TestFiles.AssertNbtBigFile(file);
        Assert.AreEqual(length, new FileInfo(TestFiles.BigZLib).Length);
    }


    [Test]
    public void LoadingBigFileBuffer()
    {
        var fileBytes = File.ReadAllBytes(TestFiles.Big);
        var file = new NbtFile();

        Assert.Throws<ArgumentNullException>(
            () => file.LoadFromBuffer(null, 0, fileBytes.Length, NbtCompression.AutoDetect, null));

        var length = file.LoadFromBuffer(fileBytes, 0, fileBytes.Length, NbtCompression.AutoDetect, null);
        TestFiles.AssertNbtBigFile(file);
        Assert.AreEqual(length, new FileInfo(TestFiles.Big).Length);
    }


    [Test]
    public void LoadingBigFileStream()
    {
        var fileBytes = File.ReadAllBytes(TestFiles.Big);
        using (var ms = new MemoryStream(fileBytes))
        {
            using (var nss = new NonSeekableStream(ms))
            {
                var file = new NbtFile();
                var length = file.LoadFromStream(nss, NbtCompression.None, null);
                TestFiles.AssertNbtBigFile(file);
                Assert.AreEqual(length, new FileInfo(TestFiles.Big).Length);
            }
        }
    }


    [Test]
    public void TestNbtSmallFileSavingUncompressed()
    {
        var file = TestFiles.MakeSmallFile();
        var testFileName = Path.Combine(TestDirName, "test.nbt");
        file.SaveToFile(testFileName, NbtCompression.None);
        FileAssert.AreEqual(TestFiles.Small, testFileName);
    }


    [Test]
    public void TestNbtSmallFileSavingUncompressedStream()
    {
        var file = TestFiles.MakeSmallFile();
        var nbtStream = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => file.SaveToStream(null, NbtCompression.None));
        Assert.Throws<ArgumentException>(() => file.SaveToStream(nbtStream, NbtCompression.AutoDetect));
        Assert.Throws<ArgumentOutOfRangeException>(() => file.SaveToStream(nbtStream, (NbtCompression)255));
        file.SaveToStream(nbtStream, NbtCompression.None);
        var testFileStream = File.OpenRead(TestFiles.Small);
        FileAssert.AreEqual(testFileStream, nbtStream);
    }


    [Test]
    public void ReloadFile()
    {
        ReloadFileInternal("bigtest.nbt", NbtCompression.None, true, true);
        ReloadFileInternal("bigtest.nbt.gz", NbtCompression.GZip, true, true);
        ReloadFileInternal("bigtest.nbt.z", NbtCompression.ZLib, true, true);
        ReloadFileInternal("bigtest.nbt", NbtCompression.None, false, true);
        ReloadFileInternal("bigtest.nbt.gz", NbtCompression.GZip, false, true);
        ReloadFileInternal("bigtest.nbt.z", NbtCompression.ZLib, false, true);
    }


    [Test]
    public void ReloadFileUnbuffered()
    {
        ReloadFileInternal("bigtest.nbt", NbtCompression.None, true, false);
        ReloadFileInternal("bigtest.nbt.gz", NbtCompression.GZip, true, false);
        ReloadFileInternal("bigtest.nbt.z", NbtCompression.ZLib, true, false);
        ReloadFileInternal("bigtest.nbt", NbtCompression.None, false, false);
        ReloadFileInternal("bigtest.nbt.gz", NbtCompression.GZip, false, false);
        ReloadFileInternal("bigtest.nbt.z", NbtCompression.ZLib, false, false);
    }


    private void ReloadFileInternal(string fileName, NbtCompression compression, bool bigEndian, bool buffered)
    {
        var loadedFile = new NbtFile(Path.Combine(TestFiles.DirName, fileName))
        {
            BigEndian = bigEndian
        };
        if (!buffered)
        {
            loadedFile.BufferSize = 0;
        }

        var bytesWritten = loadedFile.SaveToFile(Path.Combine(TestDirName, fileName), compression);
        var bytesRead = loadedFile.LoadFromFile(Path.Combine(TestDirName, fileName), NbtCompression.AutoDetect,
            null);
        Assert.AreEqual(bytesWritten, bytesRead);
        TestFiles.AssertNbtBigFile(loadedFile);
    }


    [Test]
    public void ReloadNonSeekableStream()
    {
        var loadedFile = new NbtFile(TestFiles.Big);
        using (var ms = new MemoryStream())
        {
            using (var nss = new NonSeekableStream(ms))
            {
                var bytesWritten = loadedFile.SaveToStream(nss, NbtCompression.None);
                ms.Position = 0;
                Assert.Throws<NotSupportedException>(() => loadedFile.LoadFromStream(nss, NbtCompression.AutoDetect));
                ms.Position = 0;
                Assert.Throws<InvalidDataException>(() => loadedFile.LoadFromStream(nss, NbtCompression.ZLib));
                ms.Position = 0;
                var bytesRead = loadedFile.LoadFromStream(nss, NbtCompression.None);
                Assert.AreEqual(bytesWritten, bytesRead);
                TestFiles.AssertNbtBigFile(loadedFile);
            }
        }
    }


    [Test]
    public void LoadFromStream()
    {
        LoadFromStreamInternal(TestFiles.Big, NbtCompression.None);
        LoadFromStreamInternal(TestFiles.BigGZip, NbtCompression.GZip);
        LoadFromStreamInternal(TestFiles.BigZLib, NbtCompression.ZLib);
    }


    private void LoadFromStreamInternal(string fileName, NbtCompression compression)
    {
        var file = new NbtFile();
        var fileBytes = File.ReadAllBytes(fileName);
        using (var ms = new MemoryStream(fileBytes))
        {
            file.LoadFromStream(ms, compression);
        }
    }


    [Test]
    public void SaveToBuffer()
    {
        var littleTag = new NbtCompound("Root");
        var testFile = new NbtFile(littleTag);

        var buffer1 = testFile.SaveToBuffer(NbtCompression.None);
        var buffer2 = new byte[buffer1.Length];
        Assert.AreEqual(testFile.SaveToBuffer(buffer2, 0, NbtCompression.None), buffer2.Length);
        CollectionAssert.AreEqual(buffer1, buffer2);
    }


    [Test]
    public void PrettyPrint()
    {
        var loadedFile = new NbtFile(TestFiles.Big);
        Assert.AreEqual(loadedFile.RootTag.ToString(), loadedFile.ToString());
        Assert.AreEqual(loadedFile.RootTag.ToString("   "), loadedFile.ToString("   "));
        Assert.Throws<ArgumentNullException>(() => loadedFile.ToString(null));
        Assert.Throws<ArgumentNullException>(() => NbtTag.DefaultIndentString = null);
    }


    [Test]
    public void ReadRootTag()
    {
        Assert.Throws<FileNotFoundException>(() => NbtFile.ReadRootTagName("NonExistentFile"));

        ReadRootTagInternal(TestFiles.Big, NbtCompression.None);
        ReadRootTagInternal(TestFiles.BigGZip, NbtCompression.GZip);
        ReadRootTagInternal(TestFiles.BigZLib, NbtCompression.ZLib);
    }


    private void ReadRootTagInternal(string fileName, NbtCompression compression)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NbtFile.ReadRootTagName(fileName, compression, true, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => NbtFile.ReadRootTagName(fileName, (NbtCompression)255, true, 0));

        Assert.AreEqual("Level", NbtFile.ReadRootTagName(fileName));
        Assert.AreEqual("Level", NbtFile.ReadRootTagName(fileName, compression, true, 0));

        var fileBytes = File.ReadAllBytes(fileName);
        using (var ms = new MemoryStream(fileBytes))
        {
            using (var nss = new NonSeekableStream(ms))
            {
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => NbtFile.ReadRootTagName(nss, compression, true, -1));
                NbtFile.ReadRootTagName(nss, compression, true, 0);
            }
        }
    }


    [Test]
    public void GlobalsTest()
    {
        Assert.AreEqual(NbtFile.DefaultBufferSize, new NbtFile(new NbtCompound("Foo")).BufferSize);
        Assert.Throws<ArgumentOutOfRangeException>(() => NbtFile.DefaultBufferSize = -1);
        NbtFile.DefaultBufferSize = 12345;
        Assert.AreEqual(12345, NbtFile.DefaultBufferSize);

        // Newly-created NbtFiles should use default buffer size
        var tempFile = new NbtFile(new NbtCompound("Foo"));
        Assert.AreEqual(NbtFile.DefaultBufferSize, tempFile.BufferSize);
        Assert.Throws<ArgumentOutOfRangeException>(() => tempFile.BufferSize = -1);
        tempFile.BufferSize = 54321;
        Assert.AreEqual(54321, tempFile.BufferSize);

        // Changing default buffer size should not retroactively change already-existing NbtFiles' buffer size.
        NbtFile.DefaultBufferSize = 8192;
        Assert.AreEqual(54321, tempFile.BufferSize);
    }


    [Test]
    public void HugeNbtFileTest()
    {
        // Tests writing byte arrays that exceed the max NbtBinaryWriter chunk size
        var val = new byte[5 * 1024 * 1024];
        var root = new NbtCompound("root")
        {
            new NbtByteArray("payload1")
            {
                Value = val
            }
        };
        var file = new NbtFile(root);
        file.SaveToStream(Stream.Null, NbtCompression.None);
    }


    [Test]
    public void RootTagTest()
    {
        var oldRoot = new NbtCompound("defaultRoot");
        var newFile = new NbtFile(oldRoot);

        // Ensure that inappropriate tags are not accepted as RootTag
        Assert.Throws<ArgumentNullException>(() => newFile.RootTag = null);
        Assert.Throws<ArgumentException>(() => newFile.RootTag = new NbtCompound());

        // Ensure that the root has not changed
        Assert.AreSame(oldRoot, newFile.RootTag);

        // Invalidate the root tag, and ensure that expected exception is thrown
        oldRoot.Name = null;
        Assert.Throws<NbtFormatException>(() => newFile.SaveToBuffer(NbtCompression.None));
    }


    [Test]
    public void NullParameterTest()
    {
        Assert.Throws<ArgumentNullException>(() => new NbtFile((NbtCompound)null));
        Assert.Throws<ArgumentNullException>(() => new NbtFile((string)null));

        var file = new NbtFile();
        Assert.Throws<ArgumentNullException>(() => file.LoadFromBuffer(null, 0, 1, NbtCompression.None));
        Assert.Throws<ArgumentNullException>(() => file.LoadFromBuffer(null, 0, 1, NbtCompression.None, tag => true));
        Assert.Throws<ArgumentNullException>(() => file.LoadFromFile(null));
        Assert.Throws<ArgumentNullException>(() => file.LoadFromFile(null, NbtCompression.None, tag => true));
        Assert.Throws<ArgumentNullException>(() => file.LoadFromStream(null, NbtCompression.AutoDetect));
        Assert.Throws<ArgumentNullException>(() => file.LoadFromStream(null, NbtCompression.AutoDetect, tag => true));

        Assert.Throws<ArgumentNullException>(() => file.SaveToBuffer(null, 0, NbtCompression.None));
        Assert.Throws<ArgumentNullException>(() => file.SaveToFile(null, NbtCompression.None));
        Assert.Throws<ArgumentNullException>(() => file.SaveToStream(null, NbtCompression.None));

        Assert.Throws<ArgumentNullException>(() => NbtFile.ReadRootTagName(null));
        Assert.Throws<ArgumentNullException>(
            () => NbtFile.ReadRootTagName((Stream)null, NbtCompression.None, true, 0));
    }
}