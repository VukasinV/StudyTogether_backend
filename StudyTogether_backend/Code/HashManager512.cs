using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace StudyTogether_backend.Code
{
    class HashManager512
    {
        private static HashManager512 instance;

        private HashManager512() { }

        public static HashManager512 Instance
        {
            get
            {
                if (instance == null)
                    instance = new HashManager512();

                return instance;
            }
        }

        public byte[] GenerateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[size];
            rng.GetBytes(salt);

            return salt;
        }

        public byte[] HashPassword(string password, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA512Managed();
            byte[] pwd = Encoding.UTF8.GetBytes(password);

            byte[] PwdSalt = new byte[pwd.Length + salt.Length];

            for (int i = 0; i < pwd.Length; i++)
            {
                PwdSalt[i] = pwd[i];
            }

            for (int i = 0; i < salt.Length; i++)
            {
                PwdSalt[pwd.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(PwdSalt);
        }
    }
}