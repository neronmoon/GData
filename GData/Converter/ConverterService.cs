using System;
using System.Collections.Generic;

namespace GData.Converter
{
    public class ConverterService
    {
        private char _listSeparator = ';';
        private readonly Index _index;
        private DefaultConverter _defaultConverter;
        private readonly Dictionary<Type, dynamic> _registeredConverters = new Dictionary<Type, dynamic>();

        public ConverterService(Index index)
        {
            _index = index;
            _defaultConverter = new DefaultConverter(_index, ';');
        }

        public void SetListSeparator(char separator)
        {
            _defaultConverter = new DefaultConverter(_index, separator);
        }

        public void Register<T>(IConverter<T> implementation)
        {
            _registeredConverters.Add(typeof(T), implementation);
        }

        public dynamic Convert(dynamic rawValue, Type targetType, Type preferedConverter = null)
        {
            if (preferedConverter != null) {
                dynamic instance = Activator.CreateInstance(preferedConverter);
                return instance.Convert(rawValue);
            }

            return _registeredConverters.ContainsKey(targetType)
                ? _registeredConverters[targetType].Convert(rawValue)
                : _defaultConverter.Convert(rawValue, targetType);
        }
    }
}