using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remote_Admin.Model
{
    public static class MessageEncrypt
    {

        public static byte[] GetKeyAES(Socket _sock)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            SendPublicKeyRSA(_sock, Encoding.UTF8.GetBytes(RSA.ToXmlString(false)));
            byte[] AESKey = ReciveAESKey(_sock);
            return DecryptAESKey(AESKey, RSA);
        }
         
        private static void SendPublicKeyRSA(Socket _sock, byte[] _publicKey)
        {
            _sock.Send(_publicKey);
        }

        private static byte[] ReciveAESKey(Socket _sock)
        {
            byte[] AESKey = new byte[1000];
            int length = _sock.Receive(AESKey);
            byte[] key = new byte[length];
            for (int i = 0; i < length; i++)
                key[i] = AESKey[i];
            return key;
        }

        private static byte[] DecryptAESKey(byte[] _messageToDecrypt, RSACryptoServiceProvider _rsa)
        {
            byte[] aesKey = _rsa.Decrypt(_messageToDecrypt, false);
            return aesKey;
        }

        public static byte[] EncryptByRSA(byte[] _message, byte[] _key, int _length)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            string xmlKey = CommandMessage.GetNameFromByte(_key, _length);
            RSA.FromXmlString(xmlKey);
            return RSA.Encrypt(_message, false);
        }

        public static byte[] AESEncrypt(byte[] _message, AesCryptoServiceProvider _AES)
        {
            SymmetricAlgorithm sa = Rijndael.Create();
            ICryptoTransform ct = sa.CreateEncryptor(
                (new PasswordDeriveBytes(_AES.Key, null)).GetBytes(16),
                new byte[16]);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(_message, 0, _message.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        public static void EncryptAndSendAESKey(Socket _sock, byte[] _RSApublicKey, AesCryptoServiceProvider _AES, int _length)
        {
            byte[] EncryptedAESData = MessageEncrypt.EncryptByRSA(_AES.Key, _RSApublicKey, _length);
            _sock.Send(EncryptedAESData);
            Thread.Sleep(30);
            _sock.Send(_AES.IV);
        }

        public static string AESEncrypt(string _message, AesCryptoServiceProvider _AES)
        {
            return Convert.ToBase64String(AESEncrypt(Encoding.UTF8.GetBytes(_message), _AES));
        }

        public static byte[] AESDecrypt(byte[] data, AesCryptoServiceProvider _AES)
        {
            BinaryReader br = new BinaryReader(InternalDecrypt(data, _AES.Key));
            return br.ReadBytes(12);
        }

        public static string AESDecrypt(string data, AesCryptoServiceProvider _AES)
        {
            CryptoStream cs = InternalDecrypt(Convert.FromBase64String(data), _AES.Key);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
        private static CryptoStream InternalDecrypt(byte[] data, byte[] keyAES)
        {
            SymmetricAlgorithm sa = Rijndael.Create();
            ICryptoTransform ct = sa.CreateDecryptor(
                (new PasswordDeriveBytes(keyAES, null)).GetBytes(16),
                new byte[16]);
            MemoryStream ms = new MemoryStream(data);
            return new CryptoStream(ms, ct, CryptoStreamMode.Read);
        }
    }
}
