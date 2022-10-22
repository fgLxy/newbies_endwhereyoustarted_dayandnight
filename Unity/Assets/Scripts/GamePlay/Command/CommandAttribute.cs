using System;
using UnityEngine;

namespace SunAndMoon
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public KeyCode Code;

        public CommandAttribute(KeyCode code)
        {
            Code = code;
        }
    }
}