using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PD2SoundBankEditor.AudioStreamChunks;

namespace PD2SoundBankEditor.Misc {
	static class CueUtilities {
		public class Cue : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;

			public double Time {
				get => time;
				set {
					time = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
				}
			}

			private double time;

			public string Label {
				get => label;
				set {
					label = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
				}
			}

			private string label;
		}

		public static List<Cue> CueListFromAudioStream(AudioStream audioStream) {
			List<Cue> cueList = new List<Cue>();

			RIFFChunk riffChunk = audioStream.riffChunk;
			if (riffChunk == null) return cueList;

			FMTChunk formatChunk = riffChunk.formatChunk;
			CUEChunk cueChunk = riffChunk.extraChunks.OfType<CUEChunk>().FirstOrDefault();
			LISTChunk listChunk = audioStream.riffChunk.extraChunks.OfType<LISTChunk>().Where(chunk => chunk.ListType.ToUpper() == "ADTL").FirstOrDefault();
			if (formatChunk == null || cueChunk == null || listChunk == null) return cueList;

			Dictionary<uint, CUEChunk.CuePoint> cuePoints = cueChunk.CuePoints.ToDictionary(cuePoint => cuePoint.ID);
			List<LISTChunk.ADTLSubChunks.LABLSubChunk> labls = listChunk.SubChunks.OfType<LISTChunk.ADTLSubChunks.LABLSubChunk>().ToList();

			foreach(LISTChunk.ADTLSubChunks.LABLSubChunk labl in labls) {
				CUEChunk.CuePoint cuePoint = cuePoints[labl.CuePointID];
				double seconds = cuePoint.Position / (double) formatChunk.SamplesPerSecond;

				cueList.Add(new Cue {
					Time = seconds,
					Label = labl.Text
				});
			}

			cueList = cueList.OrderBy(cue => cue.Time).ToList();
			return cueList;
		}

		public static void ApplyCueListToAudioStream(AudioStream audioStream, List<Cue> cueList) {
			cueList = cueList.OrderBy(cue => cue.Time).ToList();

			RIFFChunk riffChunk = audioStream.riffChunk;
			if (riffChunk == null) return;

			FMTChunk formatChunk = riffChunk.formatChunk;
			if (formatChunk == null) return;

			CUEChunk cueChunk = riffChunk.extraChunks.OfType<CUEChunk>().FirstOrDefault();
			LISTChunk listChunk = audioStream.riffChunk.extraChunks.OfType<LISTChunk>().Where(chunk => chunk.ListType.ToUpper() == "ADTL").FirstOrDefault();

			if (cueChunk == null) {
				cueChunk = new CUEChunk();
				riffChunk.extraChunks.Add(cueChunk);
			}

			if (listChunk == null) {
				listChunk = new LISTChunk();
				listChunk.ListType = "ADTL";
				riffChunk.extraChunks.Add(listChunk);
			}

			cueChunk.CuePoints.Clear();
			listChunk.SubChunks.Clear();

			uint cueID = 0;
			foreach(Cue cue in cueList) {
				cueID++;

				cueChunk.CuePoints.Add(new CUEChunk.CuePoint {
					ID = cueID,
					Position = (uint) Math.Round(cue.Time * formatChunk.SamplesPerSecond),
					DataChunkID = CUEChunk.DataChunk.DATA,
					ChunkStart = 0,
					BlockStart = 0,
					SampleStart = (uint) Math.Round(cue.Time * formatChunk.SamplesPerSecond)
				});

				listChunk.SubChunks.Add(new LISTChunk.ADTLSubChunks.LABLSubChunk() {
					CuePointID = cueID,
					Text = cue.Label
				});
			}
		}
	}
}
