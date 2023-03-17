using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PD2SoundBankEditor.AudioStreamChunks {
	[ChunkTag("RIFF")]
	public class RIFFChunk : AbstractChunk {
		public override uint DataSize => formatChunk.TotalSize + (uint) extraChunks.Sum(chunk => chunk.TotalSize) + dataChunk.TotalSize + 4;

		public string FileType;
		public FMTChunk formatChunk;
		public List<AbstractChunk> extraChunks = new List<AbstractChunk>();
		public DATAChunk dataChunk;

		public override void Read(BinaryReader binaryReader) {
			uint size = ReadHeader(binaryReader);
			uint end = (uint) binaryReader.BaseStream.Position + size;

			FileType = new string(binaryReader.ReadChars(4));

			while (binaryReader.BaseStream.Position < end - 8) {
				AbstractChunk chunk = ChunkLookup.ReadChunk(binaryReader, Tag, FileType);
				if (chunk.GetType() == typeof(JUNKChunk)) continue;

				if (chunk.GetType() == typeof(FMTChunk)) {
					formatChunk = (FMTChunk) chunk;
				} else if (chunk.GetType() == typeof(DATAChunk)) {
					dataChunk = (DATAChunk) chunk;
				} else {
					extraChunks.Add(chunk);
				}
			}

			binaryReader.BaseStream.Position = end;
		}

		public override void Write(BinaryWriter binaryWriter) {
			WriteHeader(binaryWriter);
			binaryWriter.Write(FileType.ToCharArray());

			formatChunk.Write(binaryWriter);
			extraChunks.ForEach(chunk => chunk.Write(binaryWriter));
			dataChunk.Write(binaryWriter);
		}
	}
}
