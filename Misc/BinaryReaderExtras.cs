using System.IO;
using System.Text;

namespace PD2SoundBankEditor.Misc {
    static class BinaryReaderExtras {
        static public string ReadCString(this BinaryReader self) {
            var sb = new StringBuilder();
            int buf;
            while ((buf = self.ReadByte()) != 0)
                sb.Append((char) buf);
            return sb.ToString();
        }
    }
}
