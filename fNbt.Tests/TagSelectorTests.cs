﻿using fNbt.Tags;
using NUnit.Framework;

namespace fNbt.Tests;

[TestFixture]
public sealed class TagSelectorTests
{
    [Test]
    public void SkippingTagsOnFileLoad()
    {
        var loadedFile = new NbtFile();
        loadedFile.LoadFromFile(TestFiles.Big,
            NbtCompression.None,
            tag => tag.Name != "nested compound test");
        Assert.IsFalse(loadedFile.RootTag.Contains("nested compound test"));
        Assert.IsTrue(loadedFile.RootTag.Contains("listTest (long)"));

        loadedFile.LoadFromFile(TestFiles.Big,
            NbtCompression.None,
            tag => tag.TagType != NbtTagType.Float || tag.Parent.Name != "Level");
        Assert.IsFalse(loadedFile.RootTag.Contains("floatTest"));
        Assert.AreEqual(0.75f, loadedFile.RootTag["nested compound test"]["ham"]["value"].FloatValue);

        loadedFile.LoadFromFile(TestFiles.Big,
            NbtCompression.None,
            tag => tag.Name != "listTest (long)");
        Assert.IsFalse(loadedFile.RootTag.Contains("listTest (long)"));
        Assert.IsTrue(loadedFile.RootTag.Contains("byteTest"));

        loadedFile.LoadFromFile(TestFiles.Big,
            NbtCompression.None,
            tag => false);
        Assert.AreEqual(0, loadedFile.RootTag.Count);
    }


    [Test]
    public void SkippingLists()
    {
        {
            var file = new NbtFile(TestFiles.MakeListTest());
            var savedFile = file.SaveToBuffer(NbtCompression.None);
            file.LoadFromBuffer(savedFile, 0, savedFile.Length, NbtCompression.None,
                tag => tag.TagType != NbtTagType.List);
            Assert.AreEqual(0, file.RootTag.Count);
        }
        {
            // Check list-compound interaction
            var comp = new NbtCompound("root")
            {
                new NbtCompound("compOfLists")
                {
                    new NbtList("listOfComps")
                    {
                        new NbtCompound
                        {
                            new NbtList("emptyList", NbtTagType.Compound)
                        }
                    }
                }
            };
            var file = new NbtFile(comp);
            var savedFile = file.SaveToBuffer(NbtCompression.None);
            file.LoadFromBuffer(savedFile, 0, savedFile.Length, NbtCompression.None,
                tag => tag.TagType != NbtTagType.List);
            Assert.AreEqual(1, file.RootTag.Count);
        }
    }


    [Test]
    public void SkippingValuesInCompoundTest()
    {
        var root = TestFiles.MakeValueTest();
        var nestedComp = TestFiles.MakeValueTest();
        nestedComp.Name = "NestedComp";
        root.Add(nestedComp);

        var file = new NbtFile(root);
        var savedFile = file.SaveToBuffer(NbtCompression.None);
        file.LoadFromBuffer(savedFile, 0, savedFile.Length, NbtCompression.None, tag => false);
        Assert.AreEqual(0, file.RootTag.Count);
    }
}