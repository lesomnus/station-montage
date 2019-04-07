using System;
using System.IO;

namespace Loko
{
    internal static class Token
    {
        private static String _id = null;
        static Token()
        {
            try
            {
                foreach (var cgroup in File.ReadAllLines("/proc/self/cgroup"))
                {
                    if (!cgroup.Contains("docker")) continue;
                    _id = cgroup.Split("docker/")[1];
                }
            }
            catch (System.Exception) { }

            if (_id == null) _id = "zzasdf";

            Console.WriteLine("The tokens is " + _id);
        }

        public static Metro.Api.Token Create()
        {
            var token = new Metro.Api.Token();
            token.Id = _id;
            return token;
        }
    }
}