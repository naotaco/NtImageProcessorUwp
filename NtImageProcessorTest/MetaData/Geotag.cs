#define WINDOWS_APP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Naotaco.ImageProcessor.MetaData;
using Naotaco.ImageProcessor.MetaData.Misc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Naotaco.ImageProcessorTest.MetaData
{
    [TestClass]
    public class Geotag
    {
        async Task<Geoposition> GetGeoposition()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            Geoposition position = null;
            try
            {
                position = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                Debug.WriteLine("Succeed to get location.");
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004 || (uint)ex.HResult == 0x80070005) {
                    Debug.WriteLine("Failed to get location, maybe due to location setting of OS");
                }
                else
                {
                    Debug.WriteLine("Failed to get location.");
                }

            }
            return position;
        }

        Task<Geoposition> WaitLocation()
        {
            return GetGeoposition();
        }

        [TestMethod]
        public void GeoTagAddition()
        {
            var task = WaitLocation();
            task.Wait();
            var position = task.Result;
            Debug.WriteLine("pos: " + position.Coordinate.Longitude + " " + position.Coordinate.Latitude);
            //var mediaLibrary = new MediaLibrary();

            int count = 0;
            foreach (string filename in TestFiles.GeotagTargetImages)
            {
                var image = TestUtil.GetResourceByteArray(filename);
                var originalMetadata = JpegMetaDataParser.ParseImage(image);
                var NewImage = MetaDataOperator.AddGeoposition(image, position);
                var newMetadata = JpegMetaDataParser.ParseImage(NewImage);
                try
                {
                    //var pic = mediaLibrary.SavePicture(string.Format("Exif addition test_" + count + "_{0:yyyyMMdd_HHmmss}.jpg", DateTime.Now), NewImage);
                }
                catch (NullReferenceException) { }

                TestUtil.IsGpsDataAdded(originalMetadata, newMetadata);

                using (var imageStream = TestUtil.GetResourceStream(filename))
                {
                    originalMetadata = JpegMetaDataParser.ParseImage(imageStream);
                    var newImageStream = MetaDataOperator.AddGeoposition(imageStream, position);
                    try
                    {
                        //var pic2 = mediaLibrary.SavePicture(string.Format("Exif addition test_" + count + "_stream_{0:yyyyMMdd_HHmmss}.jpg", DateTime.Now), newImageStream);
                    }
                    catch (NullReferenceException) { }
                    finally { newImageStream.Dispose(); }

                    TestUtil.IsGpsDataAdded(originalMetadata, newMetadata);
                    imageStream.Dispose();
                    newImageStream.Dispose();
                    count++;
                }
                GC.Collect(); // Saving many big images in short time, memory mey be run out and it may throws NullReferenceException.
            }
        }

        [TestMethod]
        public void GeotagingFailure()
        {
            var task = WaitLocation();
            task.Wait();
            var position = task.Result;
            Debug.WriteLine("pos: " + position.Coordinate.Point.Position.Longitude + " " + position.Coordinate.Point.Position.Latitude);

            foreach (string filename in TestFiles.GeotagImages)
            {
                var image = TestUtil.GetResourceByteArray(filename);
                Assert.ThrowsException<GpsInformationAlreadyExistsException>(() =>
                {
                    var NewImage = MetaDataOperator.AddGeoposition(image, position);
                });

                GC.Collect();
                using (var imageStream = TestUtil.GetResourceStream(filename))
                {
                    Assert.ThrowsException<GpsInformationAlreadyExistsException>(() =>
                    {
                        var newImageStream = MetaDataOperator.AddGeoposition(imageStream, position);
                        newImageStream.Dispose();
                    });
                }
            }
        }

        [TestMethod]
        public async void OverwriteGeotag()
        {
            var task = WaitLocation();
            task.Wait();
            var position = task.Result;
            Debug.WriteLine("pos: " + position.Coordinate.Point.Position.Longitude + " " + position.Coordinate.Point.Position.Latitude);
            //var mediaLibrary = new MediaLibrary();

            int count = 0;
            foreach (string filename in TestFiles.ValidImages)
            {
                var image = TestUtil.GetResourceByteArray(filename);
                var originalMetadata = JpegMetaDataParser.ParseImage(image);
                var NewImage = MetaDataOperator.AddGeoposition(image, position, true);
                var newMetadata = JpegMetaDataParser.ParseImage(NewImage);

                try
                {
                    //var pic = mediaLibrary.SavePicture(string.Format("Exif addition test_" + count + "_{0:yyyyMMdd_HHmmss}.jpg", DateTime.Now), NewImage);
                }
                catch (NullReferenceException) { }

                GC.Collect();
                TestUtil.IsGpsDataAdded(originalMetadata, newMetadata);

                using (var imageStream = TestUtil.GetResourceStream(filename))
                {
                    originalMetadata = JpegMetaDataParser.ParseImage(imageStream);
                    var newImageStream = MetaDataOperator.AddGeoposition(imageStream, position, true);
                    try
                    {
                        //var pic2 = mediaLibrary.SavePicture(string.Format("Exif addition test_" + count + "_stream_{0:yyyyMMdd_HHmmss}.jpg", DateTime.Now), newImageStream);
                    }
                    catch (NullReferenceException) { }
                    finally { newImageStream.Dispose(); }
                    TestUtil.IsGpsDataAdded(originalMetadata, newMetadata);

                    count++;
                }
                

                GC.Collect(); // Saving many big images in short time, memory mey be run out and it may throws NullReferenceException or OutOfMemoryException.
            }
        }

        [TestMethod]
        public void NegativeValue()
        {
            using (var imageStream = TestUtil.GetResourceStream(TestFiles.ImagesWithNegativeValues[0]))
            {
                var meta = JpegMetaDataParser.ParseImage(imageStream);
            }
        }

    }
}
