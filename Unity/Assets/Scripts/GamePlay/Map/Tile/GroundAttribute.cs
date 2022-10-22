using System;

namespace SunAndMoon
{
    public class GroundAttribute : Attribute
    {
        public GroundType GroundType;

        public GroundAttribute(GroundType type)
        {
            GroundType = type;
        }
    }
}