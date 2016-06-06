using System;

namespace SCMS.CoreBusinessLogic.Security
{
    public interface IEncryptionService
    {
        string CreateSaltKey(int size);
        String GetSHA256(String value, String salt, bool useSystemSalt);
    }
}
