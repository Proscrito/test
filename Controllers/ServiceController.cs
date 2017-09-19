using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yamvc.Core;
using yamvc.Models;
using System.Security.Cryptography;
using System.Text;
using Remotion.Linq.Clauses.ResultOperators;

namespace yamvc.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class ServiceController : Controller
    {
        public IActionResult Welcome(string id)
        {
            var model = new WelcomeModel
            {
                IsAdmin = HttpContext.User.IsInRole(UserRole.Admin),
                Login = HttpContext.User.Identity.Name,
                Data = id
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AesProcess(string data)
        {
            var processedData = DoTheJob(data);
            return RedirectToAction("Welcome", new { id = processedData });
        }

        private string DoTheJob(string data)
        {
            var encryptor = EncryptionHelper.Instance;
            var key = encryptor.GenerateRandomKey();

            //encrypt data
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            keyBytes = SHA256.Create().ComputeHash(keyBytes); 
            var encrypted = encryptor.EncryptAES(dataBytes, keyBytes);
            //encrypt key
            //with RSA 2048 we cannot encrypt string longer than 245 bytes
            var key1 = key.Substring(0, 128);
            var key2 = key.Substring(128);
            var encryptedKey1 = encryptor.EncryptRSA(key1);
            var encryptedKey2 = encryptor.EncryptRSA(key2);
            //decrypt key
            key1 = encryptor.DecryptRSA(encryptedKey1);
            key2 = encryptor.DecryptRSA(encryptedKey2);
            key = key1 + key2;
            //decrypt data
            keyBytes = Encoding.UTF8.GetBytes(key);
            keyBytes = SHA256.Create().ComputeHash(keyBytes);
            var decrypted = encryptor.DecryptAES(encrypted, keyBytes);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}