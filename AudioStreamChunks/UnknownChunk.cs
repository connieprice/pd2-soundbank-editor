using System.IO;

namespace PD2SoundBankEditor.AudioStreamChunks {
	class UnknownChunk : AbstractChunk {
		public override string Tag => tag;
		private string tag;

		public override uint DataSize => (uint) data.Length;
		public byte[] data;

		public override void Read(BinaryReader binaryReader) {
			tag = new string(binaryReader.ReadChars(4));

			uint size = binaryReader.ReadUInt32();
			data = binaryReader.ReadBytes((int) size);
		}

		public override void Write(BinaryWriter binaryWriter) {
			binaryWriter.Write(tag.ToCharArray());
			binaryWriter.Write(DataSize);

			binaryWriter.Write(data);
		}
	}
}
