using PD2SoundBankEditor.AudioStreamChunks;
using System;
using System.IO;

namespace PD2SoundBankEditor {
	public class AudioStream {
		public RIFFChunk riffChunk;

		public AudioStream(string file) {
			using (BinaryReader binaryReader = new BinaryReader(new FileStream(file, FileMode.Open))) {
				Read(binaryReader);
			}
		}

		public AudioStream(BinaryReader binaryReader) {
			Read(binaryReader);
		}

		public void Read(BinaryReader binaryReader) {
			AbstractChunk chunk = ChunkLookup.ReadChunk(binaryReader);

			if (chunk.GetType() != typeof(RIFFChunk)) throw new Exception();

			riffChunk = (RIFFChunk) chunk;
		}

		public void Write(string file) {
			using (FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write)) {
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream)) {
					Write(binaryWriter);
				}
			}
		}

		public void Write(BinaryWriter binaryWriter) {
			riffChunk.Write(binaryWriter);
		}
	}
}
