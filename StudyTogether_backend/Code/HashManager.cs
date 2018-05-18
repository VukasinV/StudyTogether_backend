using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace StudyTogether_backend.Code
{
    class HashManager
    {
        private static HashManager instance;

        private HashManager() {}

        public static HashManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new HashManager();

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
            HashAlgorithm algorithm = new SHA256Managed();
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