namespace Maml;

public partial struct Color
{
	internal uint ToUintColor()
	{
		uint r = (uint)(R * 255) << 24;
		uint g = (uint)(G * 255) << 16;
		uint b = (uint)(B * 255) << 8;
		uint a = (uint)(A * 255) << 0;
		return r | g | b | a;
	}
}
