using System.Reflection;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Colony;

/*
000:
001:
002:
003:
004:
005:
006:
007:
008:
009:
010:
011:
012:
013:
014:
015:
016:
017:
018:
019:
020:
021:
022:
023:
024:
025:
026:
027:
028:
029:
030:
031:
032:
033: !
034: "
035: #
036: $
037: %
038: &
039: '
040: (
041: )
042: *
043: +
044: ,
045: -
046: .
047: /
048: 0
049: 1
050: 2
051: 3
052: 4
053: 5
054: 6
055: 7
056: 8
057: 9
058: :
059: ;
060: <
061: =
062: >
063: ?
064: @
065: A
066: B
067: C
068: D
069: E
070: F
071: G
072: H
073: I
074: J
075: K
076: L
077: M
078: N
079: O
080: P
081: Q
082: R
083: S
084: T
085: U
086: V
087: W
088: X
089: Y
090: Z
091: [
092: \
093: ]
094: ^
095: _
096: `
097: a
098: b
099: c
100: d
101: e
102: f
103: g
104: h
105: i
106: j
107: k
108: l
109: m
110: n
111: o
112: p
113: q
114: r
115: s
116: t
117: u
118: v
119: w
120: x
121: y
122: z
123: {
124: |
125: }
126: ~
127:
128: Ç
129: ü
130: é
131: â
132: ä
133: à
134: å
135: ç
136: ê
137: ë
138: è
139: ï
140: î
141: ì
142: Ä
143: Å
144: É
145: æ
146: Æ
147: ô
148: ö
149: ò
150: û
151: ù
152: ÿ
153: Ö
154: Ü
155: ¢
156: £
157: ¥
158: ₧
159: ƒ
160: á
161: í
162: ó
163: ú
164: ñ
165: Ñ
166: ª
167: º
168: ¿
169: ⌐
170: ¬
171: ½
172: ¼
173: ¡
174: «
175: »
176: ░
177: ▒
178: ▓
179: │
180: ┤
181: ╡
182: ╢
183: ╖
184: ╕
185: ╣
186: ║
187: ╗
188: ╝
189: ╜
190: ╛
191: ┐
192: └
193: ┴
194: ┬
195: ├
196: ─
197: ┼
198: ╞
199: ╟
200: ╚
201: ╔
202: ╩
203: ╦
204: ╠
205: ═
206: ╬
207: ╧
208: ╨
209: ╤
210: ╥
211: ╙
212: ╘
213: ╒
214: ╓
215: ╫
216: ╪
217: ┘
218: ┌
219: █
220: ▄
221: ▌
222: ▐
223: ▀
224: α
225: ß
226: Γ
227: π
228: Σ
229: σ
230: µ
231: τ
232: Φ
233: Θ
234: Ω
235: δ
236: ∞
237: φ
238: ε
239: ∩
240: ≡
241: ±
242: ≥
243: ≤
244: ⌠
245: ⌡
246: ÷
247: ≈
248: °
249: ∙
250: ·
251: √
252: ⁿ
253: ²
254: ■
255:
*/

public static class Atlas
{
    public static Image<Rgba32> Build()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        const string resourceName = $"Colony.Assets.Fonts.{Configuration.Font}";

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName) ??
                           throw new Exception($"Could not find resource '{resourceName}'");

        var font = BdfFont.Load(stream);

        const int tileSizeX = 16;
        const int tileSizeY = 16;
        const int gridWidth = 16;
        const int gridHeight = 16;
        const int imageWidth = tileSizeX * gridWidth;
        const int imageHeight = tileSizeY * gridHeight;

        var image = new Image<Rgba32>(imageWidth, imageHeight, new Rgba32(0, 0, 0, 0));
        var i = 0;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var unicodeChars = new char[256];
        var cp437 = Encoding.GetEncoding(437);

        for (var j = 0; j < 256; j++)
        {
            unicodeChars[j] = cp437.GetString(new[] { (byte)j })[0];
        }

        foreach (var ch in unicodeChars)
        {
            var aChar = font.Characters[ch];

            var xPosition = (i % gridWidth) * tileSizeX;
            var yPosition = (i / gridWidth) * tileSizeY;

            for (var row = 0; row < aChar.Height; row++)
            {
                for (var col = 0; col < aChar.Width; col++)
                {
                    if (aChar.Bitmap[row, col] != 1)
                        continue;

                    for (var dy = 0; dy < 2; dy++)
                    {
                        for (var dx = 0; dx < 2; dx++)
                        {
                            var scaledX = xPosition + (col * 2) + dx;
                            var scaledY = yPosition + (row * 2) + dy;

                            if (scaledX >= imageWidth || scaledY >= imageHeight)
                                continue;

                            image[scaledX, scaledY] = new Rgba32(255, 255, 255, 255);
                        }
                    }
                }
            }

            i++;
        }

        return image;
    }
}