using Windows.Win32.Foundation;

namespace Maml.Utils;

internal static class Bits
{
	internal static short HiWord(int value) => (short)((value >> 16) & (0xffff));
	internal static short HiWord(LPARAM value) => HiWord((int)value.Value);
	internal static short HiWord(WPARAM value) => HiWord((int)value.Value);
	internal static short LoWord(int value) => (short)(value & (0xffff));
	internal static short LoWord(LPARAM value) => LoWord((int)value.Value);
	internal static short LoWord(WPARAM value) => LoWord((int)value.Value);
}