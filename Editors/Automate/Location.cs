using System;
using System.Helpers;

namespace Automate
{
    public enum LocationType
    {
        Directory,
        File
    }
    public class Location : NotifyingObject
    {
        public string Name { get { return Get<string>(); } set { Set(value); } }
        public string Path { get { return Get<string>(); } set { Set(value); } }
        public LocationType Type { get { return Get<LocationType>(); } set { Set(value); } }
        public override string ToString()
        {
            return Name ?? Path ?? "New Location";
        }
    }
}
