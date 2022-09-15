using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.JSInterop.WebAssembly;

namespace Maml;

unsafe public static class Program
{
	private static void Main(string[] args)
	{

		WasmRuntime.Instance.Invoke("Input.Run", true);
		WasmRuntime.Instance.Invoke("Renderer.Run", true);
		WasmRuntime.Instance.Invoke("Renderer.DrawLine", true, 0, 0, 200, 200, Colors.Red.ToUintColor(), 10);
	}

	[JSInvokable]
	public static void ResizeHandler(uint width, uint height)
	{
		Console.WriteLine($"{width}x{height}");
	}
}
