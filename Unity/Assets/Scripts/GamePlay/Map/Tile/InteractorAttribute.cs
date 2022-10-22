using System;

namespace SunAndMoon
{
    public class InteractorAttribute : Attribute
    {
        public InteractorType InteractorType;

        public InteractorAttribute(InteractorType type)
        {
            InteractorType = type;
        }
    }
}