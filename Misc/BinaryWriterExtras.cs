using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using System.Threading.Tasks;

namespace PD2SoundBankEditor.Misc {
	static class BinaryWriterExtras {
        static public void WriteCString(this BinaryWriter self, string str) {
            self.Write(str.ToCharArray());
            self.WritePadding(1);
        }

        static public void WritePadding(this BinaryWriter self, uint paddingNeeded) {
            for (int padCount = 0; padCount < paddingNeeded; padCount++) {
                self.Write((byte) 0);
            }
        }
    }
}
