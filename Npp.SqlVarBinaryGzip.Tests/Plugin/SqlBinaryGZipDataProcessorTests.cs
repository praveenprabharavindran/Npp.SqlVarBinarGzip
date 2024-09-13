using Npp.SqlVarBinaryGzip.Plugin;

namespace Npp.SqlVarBinaryGzip.Tests.Plugin
{
    public class SqlBinaryGZipDataProcessorTests
    {
        public class DecompressShould
        {
            [Fact]
            public void ReturnDecompressedData()
            {
                var expected = "A quick brown fox jumps over the lazy dog.";
                var data = "0x1f8b080000000000000a7354282ccd4cce56482aca2fcf5348cbaf50c82acd2d2856c82f4b2d5228c94855c849acaa5448c94fd70300e15f6b0b2a000000";
                var gZipDataProcessor = new SqlBinaryGZipDataProcessor(1000);
                var result = gZipDataProcessor.Decompress(data);
                Assert.Equal(expected, result);
            }

            [Fact]
            public void ReturnErrorWhenNonHexStringIsPassed()
            {
                var data = "0x1f8b080000000000000a7354282ccd4cce56482aca2fcf5348cbaf50c82acd2d2856c82f4b2d5228c94855c849acaa5448c94fd70300e15f6b0b2a000000,abc";
                var gZipDataProcessor = new SqlBinaryGZipDataProcessor(1000);
                Assert.Throws<ArgumentException>(() => gZipDataProcessor.Decompress(data));
            }

            [Fact]
            public void ReturnErrorWhenDataTooLarge()
            {
                var data = "0x1f8b080000000000000a7354282ccd4cce56482aca2fcf5348cbaf50c82acd2d2856c82f4b2d5228c94855c849acaa5448c94fd70300e15f6b0b2a000000";
                var gZipDataProcessor = new SqlBinaryGZipDataProcessor(20);
                var result = gZipDataProcessor.Decompress(data);
            }
        }

        public class CompressShould
        {
            [Fact]
            public void ReturnCompressedData()
            {
                var expected = "0x1f8b080000000000000a7354282ccd4cce56482aca2fcf5348cbaf50c82acd2d2856c82f4b2d5228c94855c849acaa5448c94fd70300e15f6b0b2a000000";
                var data = "A quick brown fox jumps over the lazy dog.";
                var gZipDataProcessor = new SqlBinaryGZipDataProcessor(1000);
                var result = gZipDataProcessor.Compress(data);
                // Do a case insensitive comparison
                Assert.Equal("0x1f8b080000000000000a7354282ccd4cce56482aca2fcf5348cbaf50c82acd2d2856c82f4b2d5228c94855c849acaa5448c94fd70300e15f6b0b2a000000", result.ToLower());
            }


        }

    }

}