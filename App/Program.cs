using Loko;
using System;

namespace App
{
    public class App : IApp
    {
        public void Open(IStation station, String[] args)
        {
            station.Log("noop");
        }
    }
}