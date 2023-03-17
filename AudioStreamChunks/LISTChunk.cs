using System.Collections.Generic;
using System.IO;
using System.Linq;

using PD2SoundBankEditor.Misc;

namespace PD2SoundBankEditor.AudioStreamChunks {
	[ChunkTag("LIST"), ParentChunkTag("RIFF")]
	class LISTChunk : AbstractChunk {
		public override uint DataSize => (uint) SubChunks.Sum(chunk => chunk.TotalSize) + 4;

		public string ListType;
		public List<AbstractChunk> SubChunks = new List<AbstractChunk>();

		public override void Read(BinaryReader binaryReader) {
			uint size = ReadHeader(binaryReader);
			uint end = (uint) binaryReader.BaseStream.Position + size;

			ListType = new string(binaryReader.ReadChars(4));

			while (binaryReader.BaseStream.Position < end) {
				AbstractChunk chunk = ChunkLookup.ReadChunk(binaryReader, Tag, ListType);
				SubChunks.Add(chunk);
			}
		}

		public override void Write(BinaryWriter binaryWriter) {
			WriteHeader(binaryWriter);
			binaryWriter.Write(ListType.ToCharArray());
			SubChunks.ForEach(chunk => chunk.Write(binaryWriter));
		}

		public static class ADTLSubChunks {
			[ChunkTag("labl"), ParentChunkTag("LIST"), ParentChunkTagType("adtl")]
			public class LABLSubChunk : AbstractChunk {
				public override uint DataSize => (uint) Text.Length + 5;
				public uint CuePointID;
				public string Text;

				public override void Read(BinaryReader binaryReader) {
					ReadHeader(binaryReader);

					CuePointID = binaryReader.ReadUInt32();
					Text = binaryReader.ReadCString();
				}

				public override void Write(BinaryWriter binaryWriter) {
					WriteHeader(binaryWriter);

					binaryWriter.Write(CuePointID);
					binaryWriter.WriteCString(Text);
				}
			}
		}
	}
}
