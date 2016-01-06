using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naotaco.Jpeg.MetaData.Misc
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException() { }
        public InvalidTypeException(String message)
            : base(message) { }
    }

    public class UnsupportedFileFormatException : Exception
    {
        public UnsupportedFileFormatException() { }
        public UnsupportedFileFormatException(String message)
            : base(message) { }            
    }

    public class GpsInformationAlreadyExistsException : Exception
    {
        public GpsInformationAlreadyExistsException() { }

        public GpsInformationAlreadyExistsException(String message)
            : base(message)
        { }
    }
}
