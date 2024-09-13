using System.IO.Compression;
using System.Text;

namespace Npp.SqlVarBinaryGzip.Plugin;

public class SqlBinaryGZipDataProcessor(int maxDataSize)
{
    /// <summary>
    ///     Decompresses a GZip compressed hex string.
    /// </summary>
    /// <param name="sqlBinaryData">The hex string to decompress.</param>
    /// <returns>The decompressed string.</returns>
    /// <exception cref="ArgumentException">Thrown when the input data is invalid.</exception>
    public string Decompress(string sqlBinaryData)
    {
        if (!sqlBinaryData.StartsWith("0x"))
        {
            throw new ArgumentException("The data must start with '0x'.");
        }

        if (sqlBinaryData.Length > maxDataSize)
        {
            throw new ArgumentException($"The data length must be less than or equal to {maxDataSize} characters.");
        }

        byte[] compressedData;
        try
        {
            compressedData = HexStringToByteArray(sqlBinaryData[2..]);
        }
        catch
        {
            throw new ArgumentException("The data must be a valid hex string.");
        }

        byte[] decompressedData;
        try
        {
            using (var compressedStream = new MemoryStream(compressedData))
            using (var decompressedStream = new MemoryStream())
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(decompressedStream);
                decompressedData = decompressedStream.ToArray();
            }
        }
        catch

        {
            throw new ArgumentException("The data is not valid gzip compressed data.");
        }

        // Convert the decompressed data to a string
        var result = Encoding.UTF8.GetString(decompressedData);

        // Print the decompressed data
        return result;
    }

    /// <summary>
    ///     Compresses a plain text string to a GZip compressed hex string.
    /// </summary>
    /// <param name="plainText">The plain text to compress.</param>
    /// <returns>The compressed hex string.</returns>
    /// <exception cref="ArgumentException">Thrown when the input data is too large.</exception>
    public string Compress(string plainText)
    {
        if (plainText.Length > maxDataSize)
        {
            throw new ArgumentException($"The data length must be less than or equal to {maxDataSize} characters.");
        }

        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        using var compressedStream = new MemoryStream();
        using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
        {
            gzipStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        }

        var compressedData = compressedStream.ToArray();
        return "0x" + BitConverter.ToString(compressedData).Replace("-", "");
    }

    private byte[] HexStringToByteArray(string hex)
    {
        var numberChars = hex.Length;
        var bytes = new byte[numberChars / 2];
        for (var i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }

        return bytes;
    }

    private string ByteArrayToHexString(byte[] byteArray)
    {
        var hex = new StringBuilder(byteArray.Length * 2);
        foreach (var b in byteArray)
        {
            hex.AppendFormat("{0:x2}", b);
        }

        return hex.ToString();
    }
}