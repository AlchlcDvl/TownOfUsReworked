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
        var r = hsbColor.b;
        var g = hsbColor.b;
        var b = hsbColor.b;

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
            else if (h < 240f)
            {
                r = min;
                g = (-(h - 240f) * dif / 60f) + min;
                b = max;
            }
            else if (h < 300f)
            {
                r = ((h - 240f) * dif / 60f) + min;
                g = min;
                b = max;
            }
            else if (h <= 360f)
            {
                r = max;
                g = min;
                b = (-(h - 360f) * dif / 60) + min;
            }
            else
            {
                r = 0;
                g = 0;
                b = 0;
            }
        }

        return new(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
    }

    public readonly UColor ToColor() => ToColor(this);

    public override readonly string ToString() => $"H: {h} S: {s} B: {b}";

    public static float PingPong(float min, float max, float mul) => min + Mathf.PingPong(Time.time * mul, max - min);

    public static float PingPongReverse(float min, float max, float mul) => max - Mathf.PingPong(Time.time * mul, max - min);
}