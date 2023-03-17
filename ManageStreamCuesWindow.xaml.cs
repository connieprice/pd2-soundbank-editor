using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using PD2SoundBankEditor.Misc;

namespace PD2SoundBankEditor {
	/// <summary>
	/// Interaction logic for ManageStreamCuesWindow.xaml
	/// </summary>
	public partial class ManageStreamCuesWindow : Window {
		enum StreamSource {
			LOOSE_FILE,
			SOUNDBANK
		}

		private StreamSource streamSource;
		private string filePath;
		private StreamInfo streamInfo;

		private CollectionViewSource cueListViewSource = new CollectionViewSource();

		private AudioStream audioStream;
		private List<CueUtilities.Cue> cueList;

		public ManageStreamCuesWindow(string path) {
			filePath = path;
			streamSource = StreamSource.LOOSE_FILE;

			Setup(new AudioStream(path), Path.GetFileName(path));
		}

		public ManageStreamCuesWindow(StreamInfo info) {
			streamInfo = info;
			streamSource = StreamSource.SOUNDBANK;

			Setup(new AudioStream(streamInfo), $"{info.Id} - ({info.Note})");
		}

		private void Setup(AudioStream stream, string name = "") {
			InitializeComponent();

			Title = $"Manage Stream Cues - {name}";

			audioStream = stream;
			cueList = CueUtilities.CueListFromAudioStream(audioStream);

			cueListViewSource.Source = cueList;

			dataGrid.ItemsSource = cueListViewSource.View;
			dataGrid.DataContext = cueListViewSource.View;
		}

		private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e) {
			deleteSelectedButton.IsEnabled = dataGrid.SelectedItems.Count > 0;
		}

		private void OnAddRowButtonClick(object sender, RoutedEventArgs e) {
			cueList.Add(new CueUtilities.Cue());
			cueListViewSource.View.Refresh();
		}

		private void OnDeleteSelectedClick(object sender, RoutedEventArgs e) {
			List<CueUtilities.Cue> cues = dataGrid.SelectedItems.OfType<CueUtilities.Cue>().ToList();
			cues.ForEach(cue => cueList.Remove(cue));
			cueListViewSource.View.Refresh();
		}

		private void OnCloseClick(object sender, RoutedEventArgs e) {
			Close();
		}

		private void OnSaveClick(object sender, RoutedEventArgs e) {
			CueUtilities.ApplyCueListToAudioStream(audioStream, cueList);

			switch(streamSource) {
				case StreamSource.LOOSE_FILE:
					audioStream.Write(filePath);
					break;
				case StreamSource.SOUNDBANK:
					audioStream.Write(streamInfo);
					break;
			}

			Close();
		}
	}
}
