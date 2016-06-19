namespace Dono
{
    using System;
    using System.Numerics;
    using System.Text;

    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Crypto.Digests;

    internal class Dono
    {
        public static readonly int MIN_KEY_LENGTH = 17;

        public static readonly int MAX_DK_LEN = 64;

        private static BigInteger[] rounds =
        {
            BigInteger.Parse("56641855831775999999999999999"),
            BigInteger.Parse("2178532916606769230769230768"),
            BigInteger.Parse("83789727561798816568047336"),
            BigInteger.Parse("3222681829299954483386435"),
            BigInteger.Parse("123949301126921326284092"),
            BigInteger.Parse("4767280812573897164771"),
            BigInteger.Parse("183356954329765275567"),
            BigInteger.Parse("7052190551144818290"),
            BigInteger.Parse("271238098120954548"),
            BigInteger.Parse("10432234543113635"),
            BigInteger.Parse("401239790119754"),
            BigInteger.Parse("15432299619989"),
            BigInteger.Parse("593549985383"),
            BigInteger.Parse("22828845590"),
            BigInteger.Parse("878032521"),
            BigInteger.Parse("33770480"),
            BigInteger.Parse("1298863"),
            BigInteger.Parse("49955"),
            BigInteger.Parse("1920"),
            BigInteger.Parse("72"),
            BigInteger.Parse("1"),
        };

        private static string MagicSalt = "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b";

        private static char MagicsSymbol = '!';

        private static char MagicCapital = 'A';

        public string ComputePassword(string k, string l, int dkLen = 64, bool addFixedSymbol = false, bool addFixedCapital = false)
        {
            if (k.Length < Dono.MIN_KEY_LENGTH)
            {
                throw new Exception("key.length < MIN_KEY_LENGTH");
            }

            if (dkLen > Dono.MAX_DK_LEN)
            {
                throw new Exception("dkLen > MAX_DK_LEN");
            }

            l = l.ToLower().TrimStart(' ').TrimEnd(' ');

            var c = this.GetIterations(k);

            dkLen = addFixedSymbol ? dkLen - 1 : dkLen;
            dkLen = addFixedCapital ? dkLen - 1 : dkLen;

            var d = this.DerivePassword(k, l, c, dkLen);

            if (addFixedSymbol)
            {
                d += Dono.MagicsSymbol;
            }

            if (addFixedCapital)
            {
                d += Dono.MagicCapital;
           }
            return d;
        }

        #region Helper Methods

        private int GetIterations(string k)
        {
            if (k.Length < Dono.rounds.Length)
            {
                return (int)Dono.rounds[k.Length];
            }
            else
            {
                return (int)Dono.rounds[Dono.rounds.Length - 1];
            }
        }

        private string DerivePassword(string k, string l, int c, int dkLen)
        {
            var s = this.SHA256(k + l + Dono.MagicSalt);

            return this.PBKDF2_SHA256(k, s, c, dkLen);
        }

        private string PBKDF2_SHA256(string password, string salt, int iterations, int dkLen)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            var pbkdf2 = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            pbkdf2.Init(passwordBytes, saltBytes, iterations);
            var key = ((KeyParameter)pbkdf2.GenerateDerivedParameters(dkLen * 8)).GetKey();

            return this.BinToHex(key).Substring(0, dkLen);
        }

        string SHA256(string message)
        {
            var sha256 = new Sha256Digest();
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var digestBytes = new byte[sha256.GetDigestSize()];
            sha256.BlockUpdate(messageBytes, 0, messageBytes.Length);
            sha256.DoFinal(digestBytes, 0);

            return this.BinToHex(digestBytes);
        }

        private string BinToHex(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
        }

        #endregion Helper Methods
    }
}
