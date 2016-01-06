using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Naotaco.Jpeg.MetaData;
using Naotaco.Jpeg.MetaData.Misc;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Naotaco.JpegMetadataTest.MetaData
{
    [TestClass]
    public class MetaDataOperatorTest
    {
        [TestMethod]
        public async Task ParseInvalidImage()
        {
            foreach (string file in TestFiles.InvalidImages)
            {
                using (Stream stream = await TestUtil.GetResourceStreamAsync(file))
                {
                    Assert.ThrowsException<UnsupportedFileFormatException>(() =>
                    {
                        JpegMetaDataParser.ParseImage(stream);
                    });

                    var array = await TestUtil.GetResourceByteArrayAsync(file);
                    Assert.ThrowsException<UnsupportedFileFormatException>(() =>
                    {
                        JpegMetaDataParser.ParseImage(array);
                    });
                }
                GC.Collect();
            }
        }

        [TestMethod]
        public async Task Test()
        {
            Assert.AreEqual(0, 0);
            await Task.Run(async () =>
            {
                await Task.Delay(1000);
            });
        }

        [TestMethod]
        public async Task ParseValidImage()
        {
            foreach (string file in TestFiles.ValidImages)
            {
                using (var stream = await TestUtil.GetResourceStreamAsync(file))
                {
                    JpegMetaDataParser.ParseImage(stream);

                    var array = await TestUtil.GetResourceByteArrayAsync(file);
                    JpegMetaDataParser.ParseImage(array);
                }
                Debug.WriteLine("pased: " + file);
                GC.Collect();
                Assert.AreEqual(1, 1);
            }
        }

        [TestMethod]
        public async Task ParseTestWithoutGeotag()
        {
            foreach (string file in TestFiles.ImagesWithoutGeotag)
            {
                using (var stream = await TestUtil.GetResourceStreamAsync(file))
                {
                    var meta1 = JpegMetaDataParser.ParseImage(stream);

                    var array = await TestUtil.GetResourceByteArrayAsync(file);
                    var meta2 = JpegMetaDataParser.ParseImage(array);

                    TestUtil.CompareJpegMetaData(meta1, meta2, file, false);
                }
                GC.Collect();
            }
        }

        [TestMethod]
        public async Task ParseTestWithLessTag()
        {
            foreach (string file in TestFiles.ImagesWithoutGeotagAndExiftag)
            {
                using (var stream = await TestUtil.GetResourceStreamAsync(file))
                {
                    var meta1 = JpegMetaDataParser.ParseImage(stream);

                    var array = await TestUtil.GetResourceByteArrayAsync(file);
                    var meta2 = JpegMetaDataParser.ParseImage(array);

                    TestUtil.CompareJpegMetaData(meta1, meta2, file, false, false);
                }
                GC.Collect();
            }
        }

        [TestMethod]
        public async Task ParseTestWithGeotag()
        {
            foreach (string file in TestFiles.GeotagImages)
            {
                using (var stream = await TestUtil.GetResourceStreamAsync(file))
                {
                    var meta1 = JpegMetaDataParser.ParseImage(stream);

                    var array = await TestUtil.GetResourceByteArrayAsync(file);
                    var meta2 = JpegMetaDataParser.ParseImage(array);

                    TestUtil.CompareJpegMetaData(meta1, meta2, file, true);
                }
                GC.Collect();
            }
        }
    }
}
