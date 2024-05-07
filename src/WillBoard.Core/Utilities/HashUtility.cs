using System.Text;

namespace WillBoard.Core.Utilities
{
    public static class HashUtility
    {
        public static string Serialize(byte[] hash)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}