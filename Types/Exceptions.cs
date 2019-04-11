using System;

namespace Loko.Station
{

    public class NotFoundException<T> : Exception
    {
        public readonly T what;

        public NotFoundException()
            : base("Resource not found") { }

        public NotFoundException(T what)
            : this(what, "Resource not found", null) { }

        public NotFoundException(T what, string message)
            : this(what, message, null) { }

        public NotFoundException(T what, Exception inner)
            : this(what, "Resource not found", inner) { }

        public NotFoundException(T what, string message, Exception inner)
            : base(message, inner)
        {
            this.what = what;
        }
    }

    public class ImageNotFoundException : NotFoundException<StationDesc>
    {
        public ImageNotFoundException(StationDesc what)
            : base(what, $"Image not found: {what.Image}") { }
    }

    public class StationNotFoundException : NotFoundException<StationDesc>
    {
        public StationNotFoundException(StationDesc what)
            : base(what, $"Image not found: {what.Serialize()}") { }
    }

    public class NotPermittedException : NotFoundException<StationDesc>
    {
        public NotPermittedException(StationDesc what)
            : base(what, $"Access not permitted to: {what.Serialize()}") { }
    }

    public class UnmanagedStatusCodeException : NotFoundException<int>
    {
        public UnmanagedStatusCodeException(int what)
            : base(what, $"Received unmanaged status code from Metro server: {what}") { }
    }
}