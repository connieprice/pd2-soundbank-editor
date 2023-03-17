using System.IO;

namespace PD2SoundBankEditor.AudioStreamChunks {
	[ChunkTag("data"), ParentChunkTag("RIFF")]
	public class DATAChunk : AbstractChunk {
		public override uint DataSize => (uint) data.Length;
		public byte[] data;

		public override void Read(BinaryReader binaryReader) {
			uint size = ReadHeader(binaryReader);
			data = binaryReader.ReadBytes((int) size);
		}

		public override void Write(BinaryWriter binaryWriter) {
			WriteHeader(binaryWriter);
			binaryWriter.Write(data);
		}
	}
}