using System.Collections.Generic;
using System.IO;

namespace PD2SoundBankEditor.AudioStreamChunks {
	[ChunkTag("cue "), ParentChunkTag("RIFF")]
	public class CUEChunk : AbstractChunk {
		public override uint DataSize => ((uint) CuePoints.Count * 24) + 4;

		public List<CuePoint> CuePoints = new List<CuePoint>();

		public override void Read(BinaryReader binaryReader) {
			ReadHeader(binaryReader);

			uint CuePointCount = binaryReader.ReadUInt32();
			for (uint index = 0; index < CuePointCount; index++) {
				CuePoint cuePoint = new CuePoint();
				cuePoint.Read(binaryReader);
				CuePoints.Add(cuePoint);
			};
		}

		public override void Write(BinaryWriter binaryWriter) {
			WriteHeader(binaryWriter);
			binaryWriter.Write(CuePoints.Count);
			CuePoints.ForEach(cuePoint => cuePoint.Write(binaryWriter));
		}

		public enum DataChunk : uint {
			DATA = 0x61746164,
			SILENT = 0x746E6C73
		}

		public class CuePoint {
			public uint ID;
			public uint Position;
			public DataChunk DataChunkID;
			public uint ChunkStart;
			public uint BlockStart;
			public uint SampleStart;

			public void Read(BinaryReader binaryReader) {
				ID = binaryReader.ReadUInt32();
				Position = binaryReader.ReadUInt32();
				DataChunkID = (DataChunk) binaryReader.ReadUInt32();
				ChunkStart = binaryReader.ReadUInt32();
				BlockStart = binaryReader.ReadUInt32();
				SampleStart = binaryReader.ReadUInt32();
			}

			public void Write(BinaryWriter binaryWriter) {
				binaryWriter.Write(ID);
				binaryWriter.Write(Position);
				binaryWriter.Write((uint) DataChunkID);
				binaryWriter.Write(ChunkStart);
				binaryWriter.Write(BlockStart);
				binaryWriter.Write(SampleStart);
			}
		}
	}
}
