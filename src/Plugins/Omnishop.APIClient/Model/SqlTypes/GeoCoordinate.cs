namespace OmnishopConnector.Model.SqlTypes
{
    public struct GeoCoordinate
    {
        public static bool operator ==(GeoCoordinate c1, GeoCoordinate c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(GeoCoordinate c1, GeoCoordinate c2)
        {
            return !c1.Equals(c2);
        }

        // Summary:
        //     Gets or sets the altitude in meters.
        public double Altitude { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        // Summary:
        //     Gets or sets the heading in degrees, relative to true north.
        public double Course { get; set; }

        // Summary:
        //     Gets or sets the speed in meters per second.
        //
        // Returns:
        //     The speed in meters per second. The speed must be greater than or equal to zero,
        //     or System.Double.NaN.
        public double Speed { get; set; }

        // Summary:
        //     Gets or sets the accuracy of the altitude in meters.
        public double VerticalAccuracy { get; set; }

        // Summary:
        //     Gets or sets the accuracy of the latitude and longitude in meters.
        public double HorizontalAccuracy { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is GeoCoordinate))
                return false;

            var other = (GeoCoordinate)obj;

            return this.Latitude == other.Latitude &&
                   this.Longitude == other.Longitude &&
                   this.Altitude == other.Altitude &&
                   this.Course == other.Course &&
                   this.Speed == other.Speed;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash + 23 * Latitude.GetHashCode();
            hash = hash + 23 * Longitude.GetHashCode();
            hash = hash + 23 * Altitude.GetHashCode();
            hash = hash + 23 * Course.GetHashCode();
            hash = hash + 23 * Speed.GetHashCode();
            return hash;
        }
    }
}
