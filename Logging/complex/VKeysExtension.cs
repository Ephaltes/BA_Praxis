namespace Logging.complex;

public static class VKeysExtension
{
    public static string ToStringCustom(this VKeys key)
    {
        return key switch
        {
            VKeys.Key0 or
                VKeys.Key1 or
                VKeys.Key2 or
                VKeys.Key3 or
                VKeys.Key4 or
                VKeys.Key5 or
                VKeys.Key6 or
                VKeys.Key7 or
                VKeys.Key8 or
                VKeys.Key9 or
                VKeys.Numpad0 or
                VKeys.Numpad1 or
                VKeys.Numpad2 or
                VKeys.Numpad3 or
                VKeys.Numpad4 or
                VKeys.Numpad5 or
                VKeys.Numpad6 or
                VKeys.Numpad7 or
                VKeys.Numpad8 or
                VKeys.Numpad9 or
                VKeys.KeyA or
                VKeys.KeyB or
                VKeys.KeyC or
                VKeys.KeyD or
                VKeys.KeyE or
                VKeys.KeyF or
                VKeys.KeyG or
                VKeys.KeyH or
                VKeys.KeyI or
                VKeys.KeyJ or
                VKeys.KeyK or
                VKeys.KeyL or
                VKeys.KeyM or
                VKeys.KeyN or
                VKeys.KeyO or
                VKeys.KeyP or
                VKeys.KeyQ or
                VKeys.KeyR or
                VKeys.KeyS or
                VKeys.KeyT or
                VKeys.KeyU or
                VKeys.KeyV or
                VKeys.KeyW or
                VKeys.KeyX or
                VKeys.KeyY or
                VKeys.KeyZ => key.ToString().Substring(key.ToString().Length - 1),

            VKeys.Multiply => "*",

            VKeys.OemPlus or
                VKeys.Add => "+",

            VKeys.OemMinus or
                VKeys.Subtract => "-",

            VKeys.Separator => "NUM_SEP",
            VKeys.Divide => "/",

            VKeys.OemComma => ",",
            VKeys.OemPeriod => ".",

            VKeys.Return => " <Return>\n",
            VKeys.Back => " <Backspace> ",
            VKeys.Tab => " <Tab> ",
            VKeys.Escape => " <Esc> ",

            VKeys.Hashtag => "#",
            VKeys.NextKey0 => "ß",
            VKeys.NextKey1 => "^",
            VKeys.NextBack => "´",
            VKeys.SpecialA => "Ä",
            VKeys.SpecialO => "Ö",
            VKeys.SpecialU => "U",

            VKeys.KeySmaller => "<",

            VKeys.Space => " ",

            _ => key.ToString()
        };
    }
}