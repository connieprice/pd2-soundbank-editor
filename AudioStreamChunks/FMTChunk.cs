using System.IO;

namespace PD2SoundBankEditor.AudioStreamChunks {
	public enum FormatType : ushort {
		IMA = 0x0002,
		VORBIS = 0xFFFF
	}

	public class VORBISExtraData {
		public uint TotalPCMFrames;
		public uint LoopStartPacketOffset;
		public uint LoopEndPacketOffset;
		public ushort LoopBeginExtra;
		public ushort LoopEndExtra;
		public uint SeekTableSize;
		public uint VorbisDataOffset;
		public ushort MaxPacketSize;
		public ushort LastGranuleExtra;
		public uint DecodeAllocSize;
		public uint DecodeX64AllocSize;
		public uint Unknown;
		public byte BlockSizes0;
		public byte BlockSizes1;

		public void Read(BinaryReader binaryReader) {
			TotalPCMFrames = binaryReader.ReadUInt32();
			LoopStartPacketOffset = binaryReader.ReadUInt32();
			LoopEndPacketOffset = binaryReader.ReadUInt32();
			LoopBeginExtra = binaryReader.ReadUInt16();
			LoopEndExtra = binaryReader.ReadUInt16();
			SeekTableSize = binaryReader.ReadUInt32();
			VorbisDataOffset = binaryReader.ReadUInt32();
			MaxPacketSize = binaryReader.ReadUInt16();
			LastGranuleExtra = binaryReader.ReadUInt16();
			DecodeAllocSize = binaryReader.ReadUInt32();
			DecodeX64AllocSize = binaryReader.ReadUInt32();
			Unknown = binaryReader.ReadUInt32();
			BlockSizes0 = binaryReader.ReadByte();
			BlockSizes1 = binaryReader.ReadByte();
		}

		public void Write(BinaryWriter binaryWriter) {
			binaryWriter.Write(TotalPCMFrames);
			binaryWriter.Write(LoopStartPacketOffset);
			binaryWriter.Write(LoopEndPacketOffset);
			binaryWriter.Write(LoopBeginExtra);
			binaryWriter.Write(SeekTableSize);
			binaryWriter.Write(VorbisDataOffset);
			binaryWriter.Write(MaxPacketSize);
			binaryWriter.Write(LastGranuleExtra);
			binaryWriter.Write(DecodeAllocSize);
			binaryWriter.Write(DecodeX64AllocSize);
			binaryWriter.Write(Unknown);
			binaryWriter.Write(BlockSizes0);
			binaryWriter.Write(BlockSizes1);
		}
	}

	[ChunkTag("fmt "), ParentChunkTag("RIFF"), ParentChunkTagType("WAVE")]
	public class FMTChunk : AbstractChunk {
		public override uint DataSize { get {
			switch (this.FormatTag) {
				case FormatType.IMA:
					return 24;
				case FormatType.VORBIS:
					return 66;
			}

			return 0;
		}}

		public FormatType FormatTag;
		public ushort ChannelCount;
		public uint SamplesPerSecond;
		public uint AverageBytesPerSecond;
		public ushort BlockAlign;
		public ushort BitsPerSample;

		public ushort ExtraSize;
		public ushort Unknown;

		public uint ChannelLayout;

		public VORBISExtraData vorbisExtraData = new VORBISExtraData();

		public override void Read(BinaryReader binaryReader) {
			ReadHeader(binaryReader);

			FormatTag = (FormatType) binaryReader.ReadUInt16();
			ChannelCount = binaryReader.ReadUInt16();
			SamplesPerSecond = binaryReader.ReadUInt32();
			AverageBytesPerSecond = binaryReader.ReadUInt32();
			BlockAlign = binaryReader.ReadUInt16();
			BitsPerSample = binaryReader.ReadUInt16();

			ExtraSize = binaryReader.ReadUInt16();
			Unknown = binaryReader.ReadUInt16();

			ChannelLayout = binaryReader.ReadUInt32();

			if (FormatTag == FormatType.VORBIS) {
				vorbisExtraData.Read(binaryReader);
			}
		}

		public override void Write(BinaryWriter binaryWriter) {
			WriteHeader(binaryWriter);

			binaryWriter.Write((ushort) FormatTag);
			binaryWriter.Write(ChannelCount);
			binaryWriter.Write(SamplesPerSecond);
			binaryWriter.Write(AverageBytesPerSecond);
			binaryWriter.Write(BlockAlign);
			binaryWriter.Write(BitsPerSample);

			binaryWriter.Write(ExtraSize);
			binaryWriter.Write(Unknown);

			binaryWriter.Write(ChannelLayout);

			if (FormatTag == FormatType.VORBIS) {
				vorbisExtraData.Write(binaryWriter);
			}
		}
	}
}