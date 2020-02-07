using System;

namespace GData.Attribute
{
    public class GConverter : System.Attribute
    {
        public Type ConverterType;

        public GConverter(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}