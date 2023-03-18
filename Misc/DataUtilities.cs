namespace PD2SoundBankEditor.Misc {
	static class DataUtilities {
		public static uint PaddingCalculator(uint size, uint alignment) {
			uint paddingNeeded = alignment - (size % alignment);
			return paddingNeeded == alignment ? 0 : paddingNeeded;
		}

		public static uint SizePaddingCalculator(uint size, uint alignment) {
			return size + PaddingCalculator(size, alignment);
		}
	}
}
