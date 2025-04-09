using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Services;
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string MaskSensitiveData(string value);
}

