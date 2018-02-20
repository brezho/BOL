using System;

namespace Core.Documents.Internal
{
    static class LocalHelper
    {
        public static string[] SplitAddress(string address)
        {
            int i;
            for (i = 0; i < address.Length; i++)
                if (address[i] >= '0' && address[i] <= '9')
                    break;
            if (i == address.Length)
                throw new Exception("Invalid cell address format");
            return new[] {
                address.Substring(0, i),
                address.Substring(i)
            };
        }

        public static string IndexToColumnAddress(this int index)
        {
            if (index < 26)
            {
                char c = (char)((int)'A' + index);
                string s = new string(c, 1);
                return s;
            }
            if (index < 702)
            {
                int i = index - 26;
                int i1 = (int)(i / 26);
                int i2 = i % 26;
                string s = new string((char)((int)'A' + i1), 1) +
                    new string((char)((int)'A' + i2), 1);
                return s;
            }
            if (index < 18278)
            {
                int i = index - 702;
                int i1 = (int)(i / 676);
                i = i - i1 * 676;
                int i2 = (int)(i / 26);
                int i3 = i % 26;
                string s = new string((char)((int)'A' + i1), 1) +
                    new string((char)((int)'A' + i2), 1) +
                    new string((char)((int)'A' + i3), 1);
                return s;
            }
            throw new Exception("Invalid column address");
        }

        public static int ColumnAddressToIndex(this string columnAddress)
        {
            if (columnAddress.Length == 1)
            {
                char c = columnAddress[0];
                int i = c - 'A';
                return i;
            }
            if (columnAddress.Length == 2)
            {
                char c1 = columnAddress[0];
                char c2 = columnAddress[1];
                int i1 = c1 - 'A';
                int i2 = c2 - 'A';
                return (i1 + 1) * 26 + i2;
            }
            if (columnAddress.Length == 3)
            {
                char c1 = columnAddress[0];
                char c2 = columnAddress[1];
                char c3 = columnAddress[2];
                int i1 = c1 - 'A';
                int i2 = c2 - 'A';
                int i3 = c3 - 'A';
                return (i1 + 1) * 676 + (i2 + 1) * 26 + i3;
            }
            throw new Exception("Invalid column address");
        }
    }
}
