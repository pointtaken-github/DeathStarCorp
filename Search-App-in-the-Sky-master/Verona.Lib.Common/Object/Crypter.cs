using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Verona.Lib.Common.Object
{
    public class Crypter
    {
        private readonly string[] _ceasarletters =
        {
            "xÆHk=&yMDu*IeS$v7ØhPW!J@,1r9¤QcFmC<XKdR?zBq4o; E{fOÅZL.+]82t/Gbjæ_3åYg[N>Up£aw:0lV(6ø}%in)A5-sT#§",
            "<pN:u.dlæ#YmnWoB3>!_h]Uce8S+Gå= -0&7)@Q(£T9rVFKÆ[aJ,gvqsyZbXtØ/%6HIj§DPwiE*¤?5Åøk1AOR;$M2}CxfLz{4",
            "PaEG7VAeU{9Xu:TQCy/å.+c)? ;KL!$0æ#rl}*§øMd¤p8g5vSt&kzW-3IYsFB]oRÅx@bJ4Ø%>Z<,6jiÆNh(wmH£f[_n=D1Oq2",
            "wB@[T£(I*VEMN]!Æv>gAbzf§Q0?&d/ai:.Kjpe=GR;q)y54¤Åur8_FoZs%Y tl7LmHxC9UØcDkPX<36#$}n-øOS,1W{æ2Jh+å",
            "æ9#ef$§i/2Å>Cnjqp-TOM%?ZUPx&ø]vbsS4ay{)¤zÆ=mgRQc+tW7}.D[ 08BX6Ih(J_owY;LlF3@£E*!Vr5,GKdØ1uH:A<Nåk",
            "/,§8tæhFSÆp;ZC[m¤=ÅH_åQØ2>D7UnW%*rkAX<6 gxLcNq-l)z{O0.øB1obaIP:s]K3!uyfVTe#+E&}R5(jM4dw9J£Y?G$iv@",
            "%/å2g;Ms(W9tZ5n-=l B7mz6§VwfIu):_3{Åo#$Æeh@0LTJAvNC,P]?R+rx4H8dDø.kE>F}YUb[cia¤p*yqXQ1ØK<!æGj£&SO",
            "HOB,ZM#å]2Fa7g(yzØ8mS>I0teT)uj%Uc1?[WsXh§V.+4{vP<ø;o3}D£JæÅ5!L9/xYCQ*6k EKp&l¤db_ÆN=Rn-:qf$G@Awir",
            "C3£wDk:U@aH6j M$0_]bPYå,r.lt}1*?qÅs{ceBK%VdnIhT>ENøQÆip9Fy&S<L+7A#;JZGXæf5mu/O84R)oxz!=v¤Øg[2W§(-",
            "%8dDH§LPxøIQYb;l)R4q =z&*oA+v.sÆNOgf_:9?ÅJmXwai26-£{/Øc<$EkU¤[njFZuKåtpW@3rGS(!B1y7,5C#0}V>TMheæ]"
        };

        public enum CryptType
        {
            Caesar = 1,
            Recommended = 2
        }

        private static string PasswordHash { get { return "P@@Sw0rd"; } }
        private static string SaltKey { get { return "S@LT&KEY"; } }
        private static string ViKey { get { return "@1B2c3D4e5F6g7H8"; } }

        private CryptType Ctype { get; set; }
        
        public bool LastOperationSucceeded { get; private set; }

        public static Crypter Instance(CryptType type)
        {
            var obj = new Crypter {Ctype = type};
            return obj;
        }

        public string Encrypt(string plainText)
        {
            if (Ctype.Equals(null)) return string.Empty;
            switch (Ctype)
            {
                case CryptType.Caesar: return CaesarEncrypt(plainText);
                case CryptType.Recommended: return RecommendedEncrypt(plainText);
            }
            return string.Empty;
        }

        public string Decrypt(string encryptedText)
        {
            if (Ctype.Equals(null)) return string.Empty;
            switch (Ctype)
            {
                case CryptType.Caesar: return CaesarDecrypt(encryptedText);
                case CryptType.Recommended: return RecommendedDecrypt(encryptedText);
            }
            return string.Empty;
        }

        private string CaesarEncrypt(string plainText)
        {
            const int shift = 1;
            var cryptedText = string.Empty;

            for (var index = 0; index < plainText.Length; index++)
            {
                char c = plainText[index];
                var tblIdx = index;
                while (tblIdx > 9) tblIdx = tblIdx/10;
                var curPos = _ceasarletters[tblIdx].IndexOf(c);
                var substPos = curPos + shift;
                if (substPos > (_ceasarletters[tblIdx].Length - 1)) substPos = substPos - _ceasarletters[tblIdx].Length;
                cryptedText += _ceasarletters[tblIdx].Substring(substPos, 1);
            }

            return cryptedText;
        }

        private string CaesarDecrypt(string encryptedText)
        {
            const int shift = -1;
            var plainText = string.Empty;

            for (var index = 0; index < encryptedText.Length; index++)
            {
                char c = encryptedText[index];
                var tblIdx = index;
                while (tblIdx > 9) tblIdx = tblIdx/10;
                var curPos = _ceasarletters[tblIdx].IndexOf(c);
                var substPos = curPos + shift;
                if (substPos < 0) substPos = substPos + _ceasarletters[tblIdx].Length;
                plainText += _ceasarletters[tblIdx].Substring(substPos, 1);
            }

            return plainText;
        }

        public string RecommendedEncrypt(string plainText)
        {
            if (string.IsNullOrEmpty(PasswordHash) || string.IsNullOrEmpty(SaltKey) || string.IsNullOrEmpty(ViKey))
            {
                LastOperationSucceeded = false;
                return string.Empty;
            }

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            var keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(ViKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }

            LastOperationSucceeded = true;

            return Convert.ToBase64String(cipherTextBytes);
        }

        public string RecommendedDecrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(PasswordHash) || string.IsNullOrEmpty(SaltKey) || string.IsNullOrEmpty(ViKey))
            {
                LastOperationSucceeded = false;
                return string.Empty;
            }

            var cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(ViKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var plainTextBytes = new byte[cipherTextBytes.Length];

            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();

            LastOperationSucceeded = true;

            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }
}
