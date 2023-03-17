using System.Collections.Generic;
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
		private CollectionViewSource cueListViewSource = new CollectionViewSource();

		private AudioStream audioStream;
		private string filePath;
		private List<CueUtilities.Cue> cueList;

		public ManageStreamCuesWindow(string file) {
			InitializeComponent();

			Title = $"Manage Stream Cues - {file}";

			filePath = file;
			audioStream = new AudioStream(filePath);
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
			audioStream.Write(filePath + ".output_test.stream");
			Close();
		}
	}
}
