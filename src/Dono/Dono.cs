namespace Dono
{
    using System.Numerics;

    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;
    using Windows.Storage.Streams;

    internal class Dono
    {
        public static int MIN_KEY_LENGTH = 17;

        private static BigInteger[] rounds =
        {
            BigInteger.Parse("21688899074207999999999999999"),
            BigInteger.Parse("834188425931076923076923075"),
            BigInteger.Parse("32084170228118343195266271"),
            BigInteger.Parse("1234006547235320892125624"),
            BigInteger.Parse("47461790278281572774061"),
            BigInteger.Parse("1825453472241598952847"),
            BigInteger.Parse("70209748932369190493"),
            BigInteger.Parse("2700374958937276556"),
            BigInteger.Parse("103860575343741405"),
            BigInteger.Parse("3994637513220822"),
            BigInteger.Parse("153639904354646"),
            BigInteger.Parse("5909227090562"),
            BigInteger.Parse("227277965020"),
            BigInteger.Parse("8741460192"),
            BigInteger.Parse("336210006"),
            BigInteger.Parse("12931153"),
            BigInteger.Parse("497351"),
            BigInteger.Parse("19127"),
            BigInteger.Parse("734"),
            BigInteger.Parse("27"),
            BigInteger.Parse("0"),
        };

        private static string evaluatorCheat = "!A";

        public static string ComputePassword(string k, string st)
        {
            st = st.ToLower().TrimStart(' ').TrimEnd(' ');
            var s = "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b";

            s = Dono.sha256(k + st + s);
            var d = Dono.sha256(k + st + s);

            var rs = Dono.DecideRounds(k);

            for (var i = BigInteger.Zero; i < rs; i = i + BigInteger.One)
            {
                s = Dono.sha256(d + s);
                d = Dono.sha256(d + s);
            }

            return d + Dono.evaluatorCheat;
        }

        #region Helper Methods

        private static BigInteger DecideRounds(string k)
        {
            if (k.Length < Dono.rounds.Length)
            {
                return Dono.rounds[k.Length];
            }
            else
            {
                return Dono.rounds[Dono.rounds.Length - 1];
            }
        }
        static string sha256(string password)
        {
            HashAlgorithmProvider hap = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            CryptographicHash ch = hap.CreateHash();
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
            ch.Append(buffer);
            IBuffer b_hash = ch.GetValueAndReset();

            return CryptographicBuffer.EncodeToHexString(b_hash);
        }

        #endregion Helper Methods
    }
}
