using System;

namespace Loko.Station
{

    public struct StationDesc : IEquatable<StationDesc>
    {

        internal string FlowID;
        public string Name;
        public string Image;

        public StationDesc(string flowID, string name, string image)
        {
            this.FlowID = null;
            this.Name = name;
            this.Image = image;
        }
        public StationDesc(string name, string image)
        {
            this.FlowID = "";
            this.Name = name;
            this.Image = image;
        }
        public StationDesc(Metro.Api.Station station) : this(
            flowID: station.Id,
            name: station.Name,
            image: station.Image)
        { }
        public StationDesc(string serialized) : this()
        {
            var splited = serialized.Split("~");

            switch (splited.Length)
            {
                case 2:
                    this.Name = splited[1];
                    goto case 1;
                case 1:
                    this.Image = splited[0];
                    break;
                default: break;
            }
        }

        public string Serialize() => this.Image + (String.IsNullOrWhiteSpace(this.Name) ? "~" + this.Name : "");
        static StationDesc Deserialize(string serialized) => new StationDesc(serialized);

        public bool Equals(StationDesc other) => (this.Name == other.Name) && (this.Image == other.Image);

        public override bool Equals(Object other) => other is StationDesc && Equals((StationDesc)other);

        public override int GetHashCode() => Serialize().GetHashCode();
    }
}