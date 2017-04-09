using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;

namespace L2ACP.Cryptography
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class RijndaelManaged : Rijndael
    {
        private readonly Aes _impl;

        public RijndaelManaged()
        {
            LegalBlockSizesValue = new KeySizes[] { new KeySizes(minSize: 128, maxSize: 128, skipSize: 0) };

            // This class wraps Aes
            _impl = Aes.Create();
        }

        public override int BlockSize
        {
            get { return _impl.BlockSize; }
            set
            {
                // Values which were legal in desktop RijndaelManaged but not here in this wrapper type
                if (value == 192 || value == 256)
                    throw new PlatformNotSupportedException("BlockSize must be 128 in this implementation.");

                // Any other invalid block size will get the normal “invalid block size” exception.
                _impl.BlockSize = value;
            }
        }

        public override byte[] IV
        {
            get { return _impl.IV; }
            set { _impl.IV = value; }
        }

        public override byte[] Key
        {
            get { return _impl.Key; }
            set { _impl.Key = value; }
        }

        public override int KeySize
        {
            get { return _impl.KeySize; }
            set { _impl.KeySize = value; }
        }
        public override CipherMode Mode
        {
            get { return _impl.Mode; }
            set { _impl.Mode = value; }
        }

        public override PaddingMode Padding
        {
            get { return _impl.Padding; }
            set { _impl.Padding = value; }
        }

        // LegalBlockSizes not forwarded because base has correct information
        public override KeySizes[] LegalKeySizes => _impl.LegalKeySizes;
        public override ICryptoTransform CreateEncryptor() => _impl.CreateEncryptor();
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV) => _impl.CreateEncryptor(rgbKey, rgbIV);
        public override ICryptoTransform CreateDecryptor() => _impl.CreateDecryptor();
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV) => _impl.CreateDecryptor(rgbKey, rgbIV);
        public override void GenerateIV() => _impl.GenerateIV();
        public override void GenerateKey() => _impl.GenerateKey();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _impl.Dispose();
                base.Dispose(disposing);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class Rijndael : SymmetricAlgorithm
    {
        public static Rijndael Create()
        {
            return new RijndaelImplementation();
        }

        protected Rijndael()
        {
            LegalBlockSizesValue = s_legalBlockSizes.CloneKeySizesArray();
            LegalKeySizesValue = s_legalKeySizes.CloneKeySizesArray();
            KeySizeValue = 256;
            BlockSizeValue = 128;
        }

        private static readonly KeySizes[] s_legalBlockSizes =
        {
            new KeySizes(minSize: 128, maxSize: 256, skipSize: 64)
        };

        private static readonly KeySizes[] s_legalKeySizes =
        {
            new KeySizes(minSize: 128, maxSize: 256, skipSize: 64)
        };
    }
    /// <summary>
    /// Internal implementation of Rijndael.
    /// This class is returned from Rijndael.Create() instead of the public RijndaelManaged to 
    /// be consistent with the rest of the static Create() methods which return opaque types.
    /// They both have have the same implementation.
    /// </summary>
    internal sealed class RijndaelImplementation : Rijndael
    {
        private readonly Aes _impl;

        internal RijndaelImplementation()
        {
            LegalBlockSizesValue = new KeySizes[] { new KeySizes(minSize: 128, maxSize: 128, skipSize: 0) };

            // This class wraps Aes
            _impl = Aes.Create();
        }

        public override int BlockSize
        {
            get { return _impl.BlockSize; }
            set
            {
                // Values which were legal in desktop RijndaelManaged but not here in this wrapper type
                if (value == 192 || value == 256)
                    throw new PlatformNotSupportedException("BlockSize must be 128 in this implementation.");

                // Any other invalid block size will get the normal “invalid block size” exception.
                _impl.BlockSize = value;
            }
        }

        public override byte[] IV
        {
            get { return _impl.IV; }
            set { _impl.IV = value; }
        }

        public override byte[] Key
        {
            get { return _impl.Key; }
            set { _impl.Key = value; }
        }

        public override int KeySize
        {
            get { return _impl.KeySize; }
            set { _impl.KeySize = value; }
        }
        public override CipherMode Mode
        {
            get { return _impl.Mode; }
            set { _impl.Mode = value; }
        }

        public override PaddingMode Padding
        {
            get { return _impl.Padding; }
            set { _impl.Padding = value; }
        }

        public override KeySizes[] LegalKeySizes => _impl.LegalKeySizes;
        public override ICryptoTransform CreateEncryptor() => _impl.CreateEncryptor();
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV) => _impl.CreateEncryptor(rgbKey, rgbIV);
        public override ICryptoTransform CreateDecryptor() => _impl.CreateDecryptor();
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV) => _impl.CreateDecryptor(rgbKey, rgbIV);
        public override void GenerateIV() => _impl.GenerateIV();
        public override void GenerateKey() => _impl.GenerateKey();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _impl.Dispose();
            }
        }
    }

    internal static class CryptoHelpers
    {
        public static byte[] CloneByteArray(this byte[] src)
        {
            if (src == null)
            {
                return null;
            }

            return (byte[])(src.Clone());
        }

        public static KeySizes[] CloneKeySizesArray(this KeySizes[] src)
        {
            return (KeySizes[])(src.Clone());
        }

        public static bool UsesIv(this CipherMode cipherMode)
        {
            return cipherMode != CipherMode.ECB;
        }

        public static byte[] GetCipherIv(this CipherMode cipherMode, byte[] iv)
        {
            if (cipherMode.UsesIv())
            {
                if (iv == null)
                {
                    throw new CryptographicException("The cipher mode specified requires that an initialization vector (IV) be used.");
                }

                return iv;
            }

            return null;
        }

        public static bool IsLegalSize(this int size, KeySizes[] legalSizes)
        {
            for (int i = 0; i < legalSizes.Length; i++)
            {
                KeySizes currentSizes = legalSizes[i];

                // If a cipher has only one valid key size, MinSize == MaxSize and SkipSize will be 0
                if (currentSizes.SkipSize == 0)
                {
                    if (currentSizes.MinSize == size)
                        return true;
                }
                else if (size >= currentSizes.MinSize && size <= currentSizes.MaxSize)
                {
                    // If the number is in range, check to see if it's a legal increment above MinSize
                    int delta = size - currentSizes.MinSize;

                    // While it would be unusual to see KeySizes { 10, 20, 5 } and { 11, 14, 1 }, it could happen.
                    // So don't return false just because this one doesn't match.
                    if (delta % currentSizes.SkipSize == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static byte[] GenerateRandom(int count)
        {
            byte[] buffer = new byte[count];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }
            return buffer;
        }

        // encodes the integer i into a 4-byte array, in big endian.
        public static void WriteInt(uint i, byte[] arr, int offset)
        {
            unchecked
            {
                Debug.Assert(arr != null);
                Debug.Assert(arr.Length >= offset + sizeof(uint));

                arr[offset] = (byte)(i >> 24);
                arr[offset + 1] = (byte)(i >> 16);
                arr[offset + 2] = (byte)(i >> 8);
                arr[offset + 3] = (byte)i;
            }
        }

        public static byte[] FixupKeyParity(this byte[] key)
        {
            byte[] oddParityKey = new byte[key.Length];
            for (int index = 0; index < key.Length; index++)
            {
                // Get the bits we are interested in
                oddParityKey[index] = (byte)(key[index] & 0xfe);

                // Get the parity of the sum of the previous bits
                byte tmp1 = (byte)((oddParityKey[index] & 0xF) ^ (oddParityKey[index] >> 4));
                byte tmp2 = (byte)((tmp1 & 0x3) ^ (tmp1 >> 2));
                byte sumBitsMod2 = (byte)((tmp2 & 0x1) ^ (tmp2 >> 1));

                // We need to set the last bit in oddParityKey[index] to the negation
                // of the last bit in sumBitsMod2
                if (sumBitsMod2 == 0)
                    oddParityKey[index] |= 1;
            }
            return oddParityKey;
        }

        internal static void ConvertIntToByteArray(uint value, byte[] dest)
        {
            Debug.Assert(dest != null);
            Debug.Assert(dest.Length == 4);
            dest[0] = (byte)((value & 0xFF000000) >> 24);
            dest[1] = (byte)((value & 0xFF0000) >> 16);
            dest[2] = (byte)((value & 0xFF00) >> 8);
            dest[3] = (byte)(value & 0xFF);
        }
    }
}