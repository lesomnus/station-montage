using System;

namespace Loko.Station
{

    public class StationDesc
    {
        public StationDesc(String flowID, String name, String image)
        {
            this.FlowID = flowID;
            this.Name = name;
            this.Image = image;
        }
        public StationDesc() : this("", "", "") { }
        public StationDesc(Metro.Api.Station station) : this(
            flowID: station.Id,
            name: station.Name,
            image: station.Image)
        { }
        public StationDesc(String serialized) : this()
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

        public String FlowID;
        public String Name;
        public String Image;

        public String Serialize() => this.Image + (String.IsNullOrWhiteSpace(this.Name) ? "~" + this.Name : "");
        static StationDesc Deserialize(String serialized) => new StationDesc(serialized);
    }
}