using PD2SoundBankEditor.AudioStreamChunks;
using System;
using System.IO;

namespace PD2SoundBankEditor {
	public class AudioStream {
		public RIFFChunk riffChunk;

		public AudioStream(string file) {
			using (FileStream fileStream = new FileStream(file, FileMode.Open)) {
				using (BinaryReader binaryReader = new BinaryReader(fileStream)) {
					Read(binaryReader);
				}
			}
		}

		public AudioStream(StreamInfo streamInfo) {
			using (MemoryStream memoryStream = new MemoryStream(streamInfo.Data)) {
				using (BinaryReader binaryReader = new BinaryReader(memoryStream)) {
					Read(binaryReader);
				}
			}
		}

		public AudioStream(BinaryReader binaryReader) {
			Read(binaryReader);
		}

		public void Read(BinaryReader binaryReader) {
			AbstractChunk chunk = ChunkLookup.ReadChunk(binaryReader);

			if (chunk.GetType() != typeof(RIFFChunk)) throw new Exception("File is not a RIFF chunk");

			riffChunk = (RIFFChunk) chunk;
		}

		public void Write(string file) {
			using (FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write)) {
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream)) {
					Write(binaryWriter);
				}
			}
		}

		public void Write(StreamInfo streamInfo) {
			using (MemoryStream memoryStream = new MemoryStream()) {
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream)) {
					Write(binaryWriter);
				}

				streamInfo.Data = memoryStream.ToArray();
			}
		}

		public void Write(BinaryWriter binaryWriter) {
			riffChunk.Write(binaryWriter);
		}
	}
}
