namespace TownOfUsReworked.Modules;

public struct HSBColor
{
    public float h { get; set; }
    public float s { get; set; }
    public float b { get; set; }
    public float a { get; set; }

    public HSBColor(float h, float s, float b, float a)
    {
        this.h = h;
        this.s = s;
        this.b = b;
        this.a = a;
    }

    public HSBColor(float h, float s, float b) : this(h, s, b, 1f) {}

    public HSBColor(UColor col)
    {
        var temp = FromColor(col);
        h = temp.h;
        s = temp.s;
        b = temp.b;
        a = temp.a;
    }

    public static HSBColor FromColor(UColor color)
    {
        var ret = new HSBColor(0f, 0f, 0f, color.a);

        var r = color.r;
        var g = color.g;
        var b = color.b;

        var max = Mathf.Max(r, Mathf.Max(g, b));

        if (max <= 0)
            return ret;

        var min = Mathf.Min(r, Mathf.Min(g, b));
        var dif = max - min;

        if (max > min)
        {
            if (g == max)
                ret.h = ((b - r) / dif * 60f) + 120f;
            else if (b == max)
                ret.h = ((r - g) / dif * 60f) + 240f;
            else if (b > g)
                ret.h = ((g - b) / dif * 60f) + 360f;
            else
                ret.h = (g - b) / dif * 60f;

            while (ret.h < 0)
                ret.h += 360f;

            while (ret.h > 360)
                ret.h -= 360f;
        }
        else
            ret.h = 0;

        ret.h /= 360f;
        ret.s = dif / max;
        ret.b = max;
        return ret;
    }

    public static UColor ToColor(HSBColor hsbColor)
    {
        var r = 0f;
        var g = 0f;
        var b = 0f;

        if (hsbColor.s != 0)
        {
            var max = hsbColor.b;
            var dif = hsbColor.b * hsbColor.s;
            var min = hsbColor.b - dif;

            var h = hsbColor.h * 360f;

            if (h < 60f)
            {
                r = max;
                g = (h * dif / 60f) + min;
                b = min;
            }
            else if (h < 120f)
            {
                r = (-(h - 120f) * dif / 60f) + min;
                g = max;
                b = min;
            }
            else if (h < 180f)
            {
                r = min;
                g = max;
                b = ((h - 120f) * dif / 60f) + min;
            }
            else if (h < 300)
            {
                r = min;
                g = (-(h - 240f) * dif / 60f) + min;
                b = max;
            }
            else if (h <= 360f)
            {
                r = max;
                g = min;
                b = (-(h - 360f) * dif / 60) + min;
            }
        }

        return new(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
    }

    public readonly UColor ToColor() => ToColor(this);

    public override readonly string ToString() => $"H: {h} S: {s} B: {b}";

    public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
    {
        float h, s;

        // Check special case black (color.b == 0): interpolate neither hue nor saturation!
        // Check special case grey (color.s == 0): don't interpolate hue!

        if (a.b == 0)
        {
            h = b.h;
            s = b.s;
        }
        else if (b.b == 0)
        {
            h = a.h;
            s = a.s;
        }
        else
        {
            if (a.s == 0)
                h = b.h;
            else if (b.s == 0)
                h = a.h;
            else
            {
                var angle = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);

                while (angle < 0f)
                    angle += 360f;

                while (angle > 360f)
                    angle -= 360f;

                h = angle / 360f;
            }

            s = Mathf.Lerp(a.s, b.s, t);
        }

        return new(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
    }

    /*
        test case = "0.54,0.6,1,true;1,0.69,0.23;0.5,1;1"
        basically what this does is split the string by ";" and these will be the parameters for the hue, sat, bright and alpha values
        it'll then split by ","; the main reason here is for ping pong colors where the value depends on the time it's being used
        if the count is 1, just input the value directly
        if it's 2, get a random number in the middle (could lead to some funny values :P)
        if it's 3, get a ping pong value and then put it in
        if it's 4, then parse the 4th value as a bool to check if it's in reverse (idk why i'm doing this but might as well) and get a ping pong value as such
        the alpha is optional, not including it will just set it to 1
        i'm quite proud of myself here
        PS: this is really laggy
    */
    /*public static HSBColor Parse(string input)
    {
        input = input.Replace(" ", "");
        var parts = input.Split(';');

        if (parts.Length is not (3 or 4))
            throw new IncorrectLengthException(input);

        var parts2 = new List<float>();

        foreach (var part in parts)
        {
            var parts3 = part.Split(',');

            if (parts3.Length == 1)
                parts2.Add(float.Parse(parts3[0]));
            else if (parts3.Length == 2)
                parts2.Add(URandom.RandomRange(float.Parse(parts3[0]), float.Parse(parts3[1])));
            else if (parts3.Length is 3 or 4)
            {
                var min = float.Parse(parts3[0]);
                var max = float.Parse(parts3[1]);
                var mul = float.Parse(parts3[2]);
                var reverse = parts.Length == 4 && bool.Parse(parts3[3]);
                parts2.Add(reverse ? PingPongReverse(min, max, mul) : PingPong(min, max, mul));
            }
            else
                throw new IncorrectLengthException($"{input}:{part}");
        }

        return new(parts2[0], parts2[1], parts2[2], parts2.Count == 4 ? parts2[3] : 1f);
    }

    public static bool TryParse(string input, out HSBColor color)
    {
        try
        {
            color = Parse(input);
            return true;
        }
        catch
        {
            color = default;
            return false;
        }
    }*/

    public static float PingPong(float min, float max, float mul) => min + Mathf.PingPong(Time.time * mul, max - min);

    public static float PingPongReverse(float min, float max, float mul) => max - Mathf.PingPong(Time.time * mul, max - min);

    /*public static void Test()
    {
        var color = new HSBColor(UColor.red);
        Info("red: " + color);

        color = new(UColor.green);
        Info("green: " + color);

        color = new(UColor.blue);
        Info("blue: " + color);

        color = new(UColor.grey);
        Info("grey: " + color);

        color = new(UColor.white);
        Info("white: " + color);

        color = new(new(0.4f, 1f, 0.84f, 1f));
        Info("0.4, 1f, 0.84: " + color);

        color = Parse("0.54,0.6,1,true;1,0.69,0.23;0.5,1;1");
        Info("test case : 0.54,0.6,1,true;1,0.69,0.23;0.5,1;1 ->" + color);

        Info("164, 82, 84 -> 0.643137f, 0.321568f, 0.329411f: " + ToColor(new(new(0.643137f, 0.321568f, 0.329411f))));
    }*/
}