// using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
// using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.WebAssembly;
using System;

namespace Maml;

public class WasmRuntime
{
    #region public

    public static WasmRuntime Instance => instance ??= new WasmRuntime();

    public WebAssemblyJSRuntime? Runtime { get; private set; }

    public void Invoke(string identifier, bool warn = true, params object[] args)
    {
        try
        {
            Runtime!.InvokeVoid(identifier, args);
        }
        catch (Exception e)
        {
            if (warn)
            {
                Console.WriteLine($"WARNING: function {identifier}({string.Join(", ", args)}): {e.Message}");
            }
        }
    }

    public T? Invoke<T>(string identifier, bool warn = true, params object[] args)
    {
        try
        {
            if (typeof(T) == typeof(JSObject))
            {
                return (T)Convert.ChangeType(new JSObject(Runtime!.Invoke<IJSInProcessObjectReference>(identifier, args)), typeof(T));
            }
            return Runtime!.Invoke<T>(identifier, args);
        }
        catch (Exception e)
        {
            if (warn)
            {
                Console.WriteLine($"WARNING: function {identifier}({string.Join(", ", args)}): {e.Message}");
            }
            return default;
        }
    }

    public JSObject? GetElementByID(string id, bool warn = true)
    {
        return Invoke<JSObject>("document.getElementById", warn: warn, id);
    }

    #endregion public

    #region private

    private static WasmRuntime? instance;

    private WasmRuntime()
    {
        // var args = new string[] { "Web", "WebGL2" };
        // var builder = WebAssemblyHostBuilder.CreateDefault(args);
        // var host = builder.Build();
        // host.RunAsync();
        // Runtime = (WebAssemblyJSRuntime?)host.Services.GetRequiredService<IJSRuntime>();
    }

    #endregion private

}
