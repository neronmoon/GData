namespace GData.Converter
{
    public interface IConverter<out T>
    {
        T Convert(string value);
    }
}