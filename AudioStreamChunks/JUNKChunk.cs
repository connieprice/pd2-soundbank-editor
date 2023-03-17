using System.IO;

namespace PD2SoundBankEditor.AudioStreamChunks {
	[ChunkTag("JUNK"), ParentChunkTag("RIFF")]
	class JUNKChunk : AbstractChunk {
		public override uint DataSize => JunkLength;
		public uint JunkLength;

		public override void Read(BinaryReader binaryReader) {
			JunkLength = ReadHeader(binaryReader);

			binaryReader.ReadBytes((int) JunkLength);
		}

		public override void Write(BinaryWriter binaryWriter) {
			WriteHeader(binaryWriter);

			for (uint index = 0; index < JunkLength; index++) binaryWriter.Write((byte) 0);
		}
	}
}