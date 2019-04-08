using System;

namespace Loko.Station
{
    public interface IApp
    {
        void Open(IStation station, String[] args);
    }
}
