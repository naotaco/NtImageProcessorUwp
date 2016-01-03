NtImageLib
==========

Provides functions to parse/edit JPEG metadata (like Exif) in C#

- No dependencies
- Works on UWP (Projects for Windows10)
* For WindowsPhone 8/8.1, See [NtImageLib](https://github.com/naotaco/NtImageLib)
- VisualStudio 2015 Community is required to run included tests.
- Supports geotagging to JPEG files

## JPEG metadata (Exif and other) operator

### Parser/Builder

JpegMetaDataParser parses metadata in JPEG image file simply.
Both of byte array and byte stream are supported as input format.
JpegMetaData structure may contains 3 sections(called IFD), Primary, Exif, and GPS.
Each IFD has a dictionary of Entry with keys.

```cs
var Metadata = JpegMetaDataParser.ParseImage(image);
```

JpegMetaDataProcessor#SetMetaData function overwrites metadata to given image.

```cs
var metadata = JpegMetaDataParser.ParseImage(image);
// do something to the metadata
var newImage = JpegMetaDataProcessor.SetMetaData(image, metadata);
```

### Add Geoposition to metadata

It has a method to add GPS IFD from Geoposition(Windows.Devices.Geolocation.Geoposition).

```cs
// parse given image first
var exif = JpegMetaDataParser.ParseImage(image);

// check whether the image already contains GPS section or not
if (exif.PrimaryIfd.Entries.ContainsKey(Definitions.GPS_IFD_POINTER_TAG))
{
	// You can throw excpetion
	throw new GpsInformationAlreadyExistsException("This image contains GPS information.");
}
// or overwrite section.

// Create IFD structure from given GPS info
var gpsIfdData = GpsIfdDataCreator.CreateGpsIfdData(position);

// Add GPS info to exif structure
exif.GpsIfd = gpsIfdData;

// create a new image with given location info
var newImage = JpegMetaDataProcessor.SetMetaData(image, exif);
```



