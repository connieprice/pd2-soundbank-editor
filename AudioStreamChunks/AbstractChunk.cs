using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PD2SoundBankEditor.AudioStreamChunks {
	public abstract class AbstractChunk {
		public virtual string Tag => GetType().GetCustomAttribute<ChunkTagAttribute>().Tag;
		public virtual uint TotalSize => DataSize + 8;
		public virtual uint DataSize => 0;

		public abstract void Write(BinaryWriter binaryWriter);
		public virtual void WriteHeader(BinaryWriter binaryWriter) {
			binaryWriter.Write(Tag.ToCharArray());
			binaryWriter.Write(DataSize);
		}

		public abstract void Read(BinaryReader binaryReader);
		public virtual uint ReadHeader(BinaryReader binaryReader, bool validateSize = false) {
			string tag = new string(binaryReader.ReadChars(4));
			if (tag.ToUpper() != Tag.ToUpper()) throw new Exception($"Mismatched tag!");

			uint size = binaryReader.ReadUInt32();
			if (validateSize && size != DataSize) throw new Exception($"Size of '{tag}' section incorrect!");

			size = (uint) Math.Min(size, binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);

			return size;
		}
	}

	public class ChunkLookup {
		public struct OneTag {
			public string Tag;
		}

		public struct TwoTag {
			public string Tag;
			public string ParentTag;
		}

		public struct ThreeTag {
			public string Tag;
			public string ParentTag;
			public string ParentTagType;
		}

		private static Dictionary<OneTag, Type> oneTagLookupTable;
		private static Dictionary<TwoTag, Type> twoTagLookupTable;
		private static Dictionary<ThreeTag, Type> threeTagLookupTable;

		static ChunkLookup() {
			IEnumerable<Type> allTypes = Assembly.GetCallingAssembly().GetTypes().Where(type => Attribute.IsDefined(type, typeof(ChunkTagAttribute)));

			IEnumerable<Type> oneTagTypes = allTypes.Where(type =>
				!Attribute.IsDefined(type, typeof(ParentChunkTagAttribute)) &&
				!Attribute.IsDefined(type, typeof(ParentChunkTagTypeAttribute))
			);
			oneTagLookupTable = oneTagTypes.ToDictionary(
				type => {
					return new OneTag {
						Tag = type.GetCustomAttribute<ChunkTagAttribute>().Tag.ToUpper()
					};
				}
			);

			IEnumerable<Type> twoTagTypes = allTypes.Where(type =>
				Attribute.IsDefined(type, typeof(ParentChunkTagAttribute)) &&
				!Attribute.IsDefined(type, typeof(ParentChunkTagTypeAttribute))
			);
			twoTagLookupTable = twoTagTypes.ToDictionary(
				type => {
					return new TwoTag {
						Tag = type.GetCustomAttribute<ChunkTagAttribute>().Tag.ToUpper(),
						ParentTag = type.GetCustomAttribute<ParentChunkTagAttribute>().Tag.ToUpper()
					};
				}
			);

			IEnumerable<Type> threeTagTypes = allTypes.Where(type =>
				Attribute.IsDefined(type, typeof(ParentChunkTagAttribute)) &&
				Attribute.IsDefined(type, typeof(ParentChunkTagTypeAttribute))
			);
			threeTagLookupTable = threeTagTypes.ToDictionary(
				type => {
					return new ThreeTag {
						Tag = type.GetCustomAttribute<ChunkTagAttribute>().Tag.ToUpper(),
						ParentTag = type.GetCustomAttribute<ParentChunkTagAttribute>().Tag.ToUpper(),
						ParentTagType = type.GetCustomAttribute<ParentChunkTagTypeAttribute>().TagType.ToUpper()
					};
				}
			);
		}

		public static Type ByTag(string tag) {
			OneTag tags = new OneTag {
				Tag = tag.ToUpper()
			};

			return InternalByTag<OneTag>(tags, oneTagLookupTable, typeof(UnknownChunk));
		}
		public static Type ByTag(string tag, string parentTag) {
			TwoTag tags = new TwoTag {
				Tag = tag.ToUpper(),
				ParentTag = parentTag.ToUpper()
			};

			return InternalByTag<TwoTag>(tags, twoTagLookupTable, ByTag(tag));
		}
		public static Type ByTag(string tag, string parentTag, string parentTagType) {
			ThreeTag tags = new ThreeTag {
				Tag = tag.ToUpper(),
				ParentTag = parentTag.ToUpper(),
				ParentTagType = parentTagType.ToUpper(),
			};

			return InternalByTag<ThreeTag>(tags, threeTagLookupTable, ByTag(tag, parentTag));
		}
		private static Type InternalByTag<T>(T tags, Dictionary<T, Type> lookupTable, Type fallback) {
			Type type = null;

			lookupTable.TryGetValue(tags, out type);
			if (type == null) type = fallback;

			return type;
		}

		public static AbstractChunk ReadChunk(BinaryReader binaryReader, string parentTag = null, string parentTagType = null) {
			long tempPosition = binaryReader.BaseStream.Position;
			string tag = new string(binaryReader.ReadChars(4));

			AbstractChunk chunk;
			if (parentTagType == null && parentTag == null) {
				chunk = (AbstractChunk) Activator.CreateInstance(ByTag(tag));
			} else if (parentTagType == null) {
				chunk = (AbstractChunk) Activator.CreateInstance(ByTag(tag, parentTag));
			} else {
				chunk = (AbstractChunk) Activator.CreateInstance(ByTag(tag, parentTag, parentTagType));
			}
			binaryReader.BaseStream.Position = tempPosition;
			chunk.Read(binaryReader);

			return chunk;
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class ChunkTagAttribute : Attribute {
		public string Tag { get; }

		public ChunkTagAttribute(string tag) {
			Debug.Assert(tag.Length == 4);
			this.Tag = tag;
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class ParentChunkTagAttribute : Attribute {
		public string Tag { get; }

		public ParentChunkTagAttribute(string tag) {
			Debug.Assert(tag.Length == 4);
			this.Tag = tag;
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class ParentChunkTagTypeAttribute : Attribute {
		public string TagType { get; }

		public ParentChunkTagTypeAttribute(string tag) {
			Debug.Assert(tag.Length == 4);
			this.TagType = tag;
		}
	}
}
