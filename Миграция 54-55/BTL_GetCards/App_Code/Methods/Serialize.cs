using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Summary description for Serialize
/// </summary>

public static class StringExtensions
{
    public static string ToBase64(this string text)
    {
        return ToBase64(text, Encoding.UTF8);
    }

    public static string ToBase64(this string text, Encoding encoding)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        byte[] textAsBytes = encoding.GetBytes(text);
        return Convert.ToBase64String(textAsBytes);
    }

    public static bool TryParseBase64(this string text, out string decodedText)
    {
        return TryParseBase64(text, Encoding.UTF8, out decodedText);
    }

    public static bool TryParseBase64(this string text, Encoding encoding, out string decodedText)
    {
        if (string.IsNullOrEmpty(text))
        {
            decodedText = text;
            return false;
        }

        try
        {
            byte[] textAsBytes = Convert.FromBase64String(text);
            decodedText = encoding.GetString(textAsBytes);
            return true;
        }
        catch (Exception)
        {
            decodedText = null;
            return false;
        }
    }
}

public static class SerializeHelper
{
    public static T DeserializeXMLToObject<T>(string xmlText)
    {
        if (string.IsNullOrEmpty(xmlText)) return default(T);
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(new UnicodeEncoding().GetBytes(xmlText)))
        using (System.Xml.XmlTextReader xsText = new System.Xml.XmlTextReader(memoryStream))
        {
            xsText.Normalization = true;
            return (T)xs.Deserialize(xsText);
        }
    }

    public static string getXmlForObject<T>(this T value)
    {
        if (value == null)
        {
            return string.Empty;
        }
        try
        {
            var xmlserializer = new XmlSerializer(typeof(T));
            var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlserializer.Serialize(writer, value);
                return stringWriter.ToString();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred in getXmlForObject", ex);
        }
    }
}