﻿using System;
using fNbt.Tags;
using NUnit.Framework;

namespace fNbt.Tests;

[TestFixture]
public class MiscTests
{
    [Test]
    public void CopyConstructorTest()
    {
        var byteTag = new NbtByte("byteTag", 1);
        var byteTagClone = (NbtByte)byteTag.Clone();
        Assert.AreNotSame(byteTag, byteTagClone);
        Assert.AreEqual(byteTag.Name, byteTagClone.Name);
        Assert.AreEqual(byteTag.Value, byteTagClone.Value);
        Assert.Throws<ArgumentNullException>(() => new NbtByte((NbtByte)null));

        var byteArrTag = new NbtByteArray("byteArrTag", new byte[] { 1, 2, 3, 4 });
        var byteArrTagClone = (NbtByteArray)byteArrTag.Clone();
        Assert.AreNotSame(byteArrTag, byteArrTagClone);
        Assert.AreEqual(byteArrTag.Name, byteArrTagClone.Name);
        Assert.AreNotSame(byteArrTag.Value, byteArrTagClone.Value);
        CollectionAssert.AreEqual(byteArrTag.Value, byteArrTagClone.Value);
        Assert.Throws<ArgumentNullException>(() => new NbtByteArray((NbtByteArray)null));

        var compTag = new NbtCompound("compTag", new NbtTag[] { new NbtByte("innerTag", 1) });
        var compTagClone = (NbtCompound)compTag.Clone();
        Assert.AreNotSame(compTag, compTagClone);
        Assert.AreEqual(compTag.Name, compTagClone.Name);
        Assert.AreNotSame(compTag["innerTag"], compTagClone["innerTag"]);
        Assert.AreEqual(compTag["innerTag"].Name, compTagClone["innerTag"].Name);
        Assert.AreEqual(compTag["innerTag"].ByteValue, compTagClone["innerTag"].ByteValue);
        Assert.Throws<ArgumentNullException>(() => new NbtCompound((NbtCompound)null));

        var doubleTag = new NbtDouble("doubleTag", 1);
        var doubleTagClone = (NbtDouble)doubleTag.Clone();
        Assert.AreNotSame(doubleTag, doubleTagClone);
        Assert.AreEqual(doubleTag.Name, doubleTagClone.Name);
        Assert.AreEqual(doubleTag.Value, doubleTagClone.Value);
        Assert.Throws<ArgumentNullException>(() => new NbtDouble((NbtDouble)null));

        var floatTag = new NbtFloat("floatTag", 1);
        var floatTagClone = (NbtFloat)floatTag.Clone();
        Assert.AreNotSame(floatTag, floatTagClone);
        Assert.AreEqual(floatTag.Name, floatTagClone.Name);
        Assert.AreEqual(floatTag.Value, floatTagClone.Value);
        Assert.Throws<ArgumentNullException>(() => new NbtFloat((NbtFloat)null));

        var intTag = new NbtInt("intTag", 1);
        var intTagClone = (NbtInt)intTag.Clone();
        Assert.AreNotSame(intTag, intTagClone);
        Assert.AreEqual(intTag.Name, intTagClone.Name);
        Assert.AreEqual(intTag.Value, intTagClone.Value);
        Assert.Throws<ArgumentNullException>(() => new NbtInt((NbtInt)null));

        var intArrTag = new NbtIntArray("intArrTag", new[] { 1, 2, 3, 4 });
        var intArrTagClone = (NbtIntArray)intArrTag.Clone();
        Assert.AreNotSame(intArrTag, intArrTagClone);
        Assert.AreEqual(intArrTag.Name, intArrTagClone.Name);
        Assert.AreNotSame(intArrTag.Value, intArrTagClone.Value);
        CollectionAssert.AreEqual(intArrTag.Value, intArrTagClone.Value);
        Assert.Throws<ArgumentNullException>(() => new NbtIntArray((NbtIntArray)null));

        var longArrTag = new NbtLongArray("longArrTag", new long[] { 1, 2, 3, 4 });
        var longArrTagClone = (NbtLongArray)longArrTag.Clone();
        Assert.AreNotSame(longArrTag, longArrTagClone);
        Assert.AreEqual(longArrTag.Name, longArrTagClone.Name);
        Assert.AreNotSame(longArrTag.Value, longArrTagClone.Value);
        CollectionAssert.AreEqual(longArrTag.Value, longArrTagClone.Value);
        Assert.Throws<ArgumentNullException>(() => new NbtLongArray((NbtLongArray)null));

        var listTag = new NbtList("listTag", new NbtTag[] { new NbtByte(1) });
        var listTagClone = (NbtList)listTag.Clone();
        Assert.AreNotSame(listTag, listTagClone);
        Assert.AreEqual(listTag.Name, listTagClone.Name);
        Assert.AreNotSame(listTag[0], listTagClone[0]);
        Assert.AreEqual(listTag[0].ByteValue, listTagClone[0].ByteValue);
        Assert.Throws<ArgumentNullException>(() => new NbtList((NbtList)null));

        var longTag = new NbtLong("longTag", 1);
        var longTagClone = (NbtLong)longTag.Clone();
        Assert.AreNotSame(longTag, longTagClone);
        Assert.AreEqual(longTag.Name, longTagClone.Name);
        Assert.AreEqual(longTag.Value, longTagClone.Value);
        Assert.Throws<ArgumentNullException>(() => new NbtLong((NbtLong)null));

        var shortTag = new NbtShort("shortTag", 1);
        var shortTagClone = (NbtShort)shortTag.Clone();
        Assert.AreNotSame(shortTag, shortTagClone);
        Assert.AreEqual(shortTag.Name, shortTagClone.Name);
        Assert.AreEqual(shortTag.Value, shortTagClone.Value);
        Assert.Throws<ArgumentNullException>(() => new NbtShort((NbtShort)null));

        var stringTag = new NbtString("stringTag", "foo");
        var stringTagClone = (NbtString)stringTag.Clone();
        Assert.AreNotSame(stringTag, stringTagClone);
        Assert.AreEqual(stringTag.Name, stringTagClone.Name);
        Assert.AreEqual(stringTag.Value, stringTagClone.Value);
        Assert.Throws<ArgumentNullException>(() => new NbtString((NbtString)null));
    }


    [Test]
    public void ByteArrayIndexerTest()
    {
        // test getting/settings values of byte array tag via indexer
        var byteArray = new NbtByteArray("Test");
        CollectionAssert.AreEqual(new byte[0], byteArray.Value);
        byteArray.Value = new byte[]
        {
            1, 2, 3
        };
        Assert.AreEqual(1, byteArray[0]);
        Assert.AreEqual(2, byteArray[1]);
        Assert.AreEqual(3, byteArray[2]);
        byteArray[0] = 4;
        Assert.AreEqual(4, byteArray[0]);
    }


    [Test]
    public void IntArrayIndexerTest()
    {
        // test getting/settings values of int array tag via indexer
        var intArray = new NbtIntArray("Test");
        CollectionAssert.AreEqual(new int[0], intArray.Value);
        intArray.Value = new[]
        {
            1, 2000, -3000000
        };
        Assert.AreEqual(1, intArray[0]);
        Assert.AreEqual(2000, intArray[1]);
        Assert.AreEqual(-3000000, intArray[2]);
        intArray[0] = 4;
        Assert.AreEqual(4, intArray[0]);
    }


    [Test]
    public void LongArrayIndexerTest()
    {
        var longArray = new NbtLongArray("Test");
        CollectionAssert.AreEqual(new long[0], longArray.Value);
        longArray.Value = new[]
        {
            1,
            long.MaxValue,
            long.MinValue
        };
        Assert.AreEqual(1, longArray[0]);
        Assert.AreEqual(long.MaxValue, longArray[1]);
        Assert.AreEqual(long.MinValue, longArray[2]);
        longArray[0] = 4;
        Assert.AreEqual(4, longArray[0]);
    }


    [Test]
    public void DefaultValueTest()
    {
        // test default values of all value tags
        Assert.AreEqual(0, new NbtByte("test").Value);
        CollectionAssert.AreEqual(new byte[0], new NbtByteArray("test").Value);
        Assert.AreEqual(0d, new NbtDouble("test").Value);
        Assert.AreEqual(0f, new NbtFloat("test").Value);
        Assert.AreEqual(0, new NbtInt("test").Value);
        CollectionAssert.AreEqual(new int[0], new NbtIntArray("test").Value);
        CollectionAssert.AreEqual(new long[0], new NbtLongArray("test").Value);
        Assert.AreEqual(0L, new NbtLong("test").Value);
        Assert.AreEqual(0, new NbtShort("test").Value);
        Assert.AreEqual("", new NbtString().Value);
    }


    [Test]
    public void NullValueTest()
    {
        Assert.Throws<ArgumentNullException>(() => new NbtByteArray().Value = null);
        Assert.Throws<ArgumentNullException>(() => new NbtIntArray().Value = null);
        Assert.Throws<ArgumentNullException>(() => new NbtLongArray().Value = null);
        Assert.Throws<ArgumentNullException>(() => new NbtString().Value = null);
    }


    [Test]
    public void PathTest()
    {
        // test NbtTag.Path property
        var testComp = new NbtCompound
        {
            new NbtCompound("Compound")
            {
                new NbtCompound("InsideCompound")
            },
            new NbtList("List")
            {
                new NbtCompound
                {
                    new NbtInt("InsideCompoundAndList")
                }
            }
        };

        // parent-less tag with no name has empty string for a path
        Assert.AreEqual("", testComp.Path);
        Assert.AreEqual(".Compound", testComp["Compound"].Path);
        Assert.AreEqual(".Compound.InsideCompound", testComp["Compound"]["InsideCompound"].Path);
        Assert.AreEqual(".List", testComp["List"].Path);

        // tags inside lists have no name, but they do have an index
        Assert.AreEqual(".List[0]", testComp["List"][0].Path);
        Assert.AreEqual(".List[0].InsideCompoundAndList", testComp["List"][0]["InsideCompoundAndList"].Path);
    }


    [Test]
    public void BadParamsTest()
    {
        Assert.Throws<ArgumentNullException>(() => new NbtByteArray((byte[])null));
        Assert.Throws<ArgumentNullException>(() => new NbtIntArray((int[])null));
        Assert.Throws<ArgumentNullException>(() => new NbtLongArray((long[])null));
        Assert.Throws<ArgumentNullException>(() => new NbtString((string)null));
    }
}