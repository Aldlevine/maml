using Microsoft.JSInterop;
using System;

namespace Maml;

public class JSObject : IDisposable
{
    public IJSInProcessObjectReference Reference { get; private set; }

    public JSObject(IJSInProcessObjectReference reference)
    {
        Reference = reference;
    }

    public void Invoke(string identifier, bool warn = true, params object[] args)
    {
        try
        {
            Reference.InvokeVoid(identifier, args);
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
                return (T)Convert.ChangeType(new JSObject(Reference.Invoke<IJSInProcessObjectReference>(identifier, args)), typeof(T));
            }
            return Reference.Invoke<T>(identifier, args);
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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        Reference?.Dispose();
    }
}
