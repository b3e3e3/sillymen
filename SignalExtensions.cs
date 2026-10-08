using System.Threading.Tasks;
using Godot;

namespace Sillymen;

public static class SignalExtensions
{
    public static async Task<T> ToSignal<[MustBeVariant] T>(this GodotObject source, StringName signal) where T : GodotObject
    {
        var args = await source.ToSignal(source, signal);
        return args[0].As<T>();
    }
}