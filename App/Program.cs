using Loko.Station;
using System;

namespace App
{
    public class App : IApp
    {
        public void Open(IStation station, string[] args)
        {
            station.Log("noop");
        }
    }
}