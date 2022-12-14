using SDL2;
using System;
using System.Collections.Generic;
using System.IO;

static partial class Engine
{
    private static string GetAssetPath(string path)
    {
        return Path.Combine("Assets", path);
    }

    /// <summary>
    /// Loads a texture from the Assets directory. Supports the following formats: BMP, GIF, JPEG, PNG, SVG, TGA, TIFF, WEBP.
    /// </summary>
    /// <param name="path">The path to the texture file, relative to the Assets directory.</param>
    public static Texture LoadTexture(string path)
    {
        IntPtr handle = SDL_image.IMG_LoadTexture(Renderer, GetAssetPath(path));
        if (handle == IntPtr.Zero)
        {
            throw new Exception("Failed to load texture.");
        }

        uint format;
        int access, width, height;
        SDL.SDL_QueryTexture(handle, out format, out access, out width, out height);

        return new Texture(handle, width, height);
    }

    /// <summary>
    /// Loads a resizable texture from the Assets directory. Supports the following formats: BMP, GIF, JPEG, PNG, SVG, TGA, TIFF, WEBP.
    /// See the documentation for an explanation of what these parameters _actually_ mean.
    /// </summary>
    /// <param name="path">The path to the texture file, relative to the Assets directory.</param>
    /// <param name="leftOffset">The resize offset from the left of the texture (in pixels).</param>
    /// <param name="rightOffset">The resize offset from the right of the texture (in pixels).</param>
    /// <param name="topOffset">The resize offset from the top of the texture (in pixels).</param>
    /// <param name="bottomOffset">The resize offset from the bottom of the texture (in pixels).</param>
    public static ResizableTexture LoadResizableTexture(string path, int leftOffset, int rightOffset, int topOffset, int bottomOffset)
    {
        IntPtr handle = SDL_image.IMG_LoadTexture(Renderer, GetAssetPath(path));
        if (handle == IntPtr.Zero)
        {
            throw new Exception("Failed to load texture.");
        }

        uint format;
        int access, width, height;
        SDL.SDL_QueryTexture(handle, out format, out access, out width, out height);

        // Convert the relative offsets (from the edges) into absolute offsets (from the origin):
        rightOffset = width - rightOffset - 1;
        bottomOffset = height - bottomOffset - 1;

        if (leftOffset < 0 || rightOffset >= width || topOffset < 0 || bottomOffset >= height || leftOffset > rightOffset || topOffset > bottomOffset)
        {
            throw new Exception("Invalid offset parameter.");
        }

        return new ResizableTexture(handle, width, height, leftOffset, rightOffset, topOffset, bottomOffset);
    }

    /// <summary>
    /// Loads a font from the Assets directory for a single text size. Supports the following formats: TTF, FON.
    /// </summary>
    /// <param name="path">The path to the font file, relative to the Assets directory.</param>
    /// <param name="pointSize">The size of the text that will be rendered by this font (in points).</param>
    public static Font LoadFont(string path, int pointSize)
    {
        IntPtr handle = SDL_ttf.TTF_OpenFont(GetAssetPath(path), pointSize);
        if (handle == IntPtr.Zero)
        {
            throw new Exception("Failed to load font.");
        }

        return new Font(handle);
    }

    /// <summary>
    /// Loads a sound file from the Assets directory. Supports the following formats: WAV, OGG.
    /// </summary>
    /// <param name="path">The path to the sound file, relative to the Assets directory.</param>
    public static Sound LoadSound(string path)
    {
        IntPtr handle = SDL_mixer.Mix_LoadWAV(GetAssetPath(path));
        if (handle == IntPtr.Zero)
        {
            throw new Exception("Failed to load sound.");
        }

        return new Sound(handle);
    }

    /// <summary>
    /// Loads a music file from the Assets directory. Supports the following formats: WAV, OGG, MP3, FLAC.
    /// </summary>
    /// <param name="path">The path to the music file, relative to the Assets directory.</param>
    public static Music LoadMusic(string path)
    {
        IntPtr handle = SDL_mixer.Mix_LoadMUS(GetAssetPath(path));
        if (handle == IntPtr.Zero)
        {
            throw new Exception("Failed to load music.");
        }

        return new Music(handle);
    }
}


/// <summary>
/// Representation of a polygon for geometry and rendering.
/// </summary>
public class Polygon
{
    public Vector2[] points;
    public Color color;
    public int xMin, xMax, yMin, yMax;
    public int vertices;

    public Polygon(Vector2[] initial, Color color)
    {
        if (initial.Length > 0)
        {
            this.color = color;

            points = initial;
            xMin = 0;
            xMax = 0;

            calcMinMax();
        }
    }

    public Polygon(float[] xVals, float[] yVals, Color color)
    {
        if (xVals.Length > 0 && xVals.Length == yVals.Length)
        {
            this.color = color;

            vertices = xVals.Length;
            points = new Vector2[vertices];

            for (int i = 0; i < vertices; i++)
            {
                points[i].X = xVals[i];
                points[i].Y = yVals[i];
            }

            calcMinMax();
        }
    }

    public bool isConvex()
    {
        if (vertices < 4)
        {
            return true;
        } 

        float dir = Vector2.Cross(points[1] - points[0], points[0] - points[vertices - 1]);
        for (int i = 1; i < vertices; i++)
        {
            float cur = Vector2.Cross(points[(i + 1) % vertices] - points[i], points[i] - points[i - 1]);
            if (dir == 0)
            {
                dir = cur;
            }
            else if (Math.Sign(dir) != Math.Sign(cur))
            {
                return false;
            }
        }
        return true;
    }

    public void splice(float height) // Assumes that height is a valid line that passes through
    {
        int start = 0;

        int enter = 0;

        while(points[start].Y < height)
        {
            start++;
            if(start >= points.Length)
            {
                return;
            }
        }

        for (int k = 0; k < points.Length; k++)
        {
            int i = (start + k) % points.Length;
            int ia = (i + 1) % points.Length; // looking at segment from i to ia, and ia to iaa
            int iaa = (ia + 1) % points.Length;

            if (points[i].Y >= height && points[ia].Y < height && points[iaa].Y >= height) // 1 bad vertex
            {
                Vector2[] newP = new Vector2[vertices + 1];

                newP[0] = points[i];

                float m = (points[i].Y - points[ia].Y) / (points[i].X - points[ia].X);

                newP[1] = new Vector2(points[i].X + (height - points[i].Y) / m, height);

                m = (points[ia].Y - points[iaa].Y) / (points[ia].X - points[iaa].X);

                newP[2] = new Vector2(points[ia].X + (height - points[ia].Y) / m, height);

                int cur = iaa;

                for (int j = 3; j < newP.Length; j++)
                {
                    newP[j] = points[cur];
                    cur = (cur + 1) % points.Length;
                }

                points = newP;

                break;
            }

            if (points[i].Y >= height && points[ia].Y < height) // entering removed aera
            {
                enter = ia;

                float m = (points[i].Y - points[ia].Y) / (points[i].X - points[ia].X);

                points[ia] = new Vector2(points[i].X + (height - points[i].Y) / m, height);

            }
            else if (points[i].Y < height && points[ia].Y >= height) // exiting removed area
            {
                float m = (points[i].Y - points[ia].Y) / (points[i].X - points[ia].X);

                List<Vector2> temp = new List<Vector2>();

                temp.Add(new Vector2(points[i].X + (height - points[i].Y) / m, height));

                for (int j = ia; j != enter; j = (j + 1) % points.Length)
                {
                    temp.Add(points[j]);
                }

                temp.Add(points[enter]);

                points = temp.ToArray();
                
                break;
            }
        }

        calcMinMax();
    }

    private void calcMinMax()
    {
        vertices = points.Length;
        xMin = 0;
        xMax = 0;

        for (int i = 0; i < vertices; i++)
        {
            if (points[i].X < points[xMin].X)
            {
                xMin = i;
            }

            if (points[i].X > points[xMax].X)
            {
                xMax = i;
            }
        }
    }

    public void calcMinMaxY()
    {
        vertices = points.Length;
        yMin = 0;
        yMax = 0;

        for (int i = 0; i < vertices; i++)
        {
            if (points[i].Y < points[yMin].Y)
            {
                yMin = i;
            }

            if (points[i].Y > points[yMax].Y)
            {
                yMax = i;
            }
        }
    }
}

/// <summary>
/// A handle to a texture. These should only be created by calling LoadTexture().
/// </summary>
public class Texture
{
    public readonly IntPtr Handle;
    public readonly int Width;
    public readonly int Height;
    public readonly Vector2 Size;

    public Texture(IntPtr handle, int width, int height)
    {
        Handle = handle;
        Width = width;
        Height = height;
        Size = new Vector2(width, height);
    }
}

/// <summary>
/// A handle to a resizable texture. These should only be created by calling LoadResizableTexture().
/// </summary>
class ResizableTexture
{
    public readonly IntPtr Handle;
    public readonly int Width;
    public readonly int Height;
    public readonly int LeftOffset;
    public readonly int RightOffset;
    public readonly int TopOffset;
    public readonly int BottomOffset;

    public ResizableTexture(IntPtr handle, int width, int height, int leftOffset, int rightOffset, int topOffset, int bottomOffset)
    {
        Handle = handle;
        Width = width;
        Height = height;
        LeftOffset = leftOffset;
        RightOffset = rightOffset;
        TopOffset = topOffset;
        BottomOffset = bottomOffset;
    }
}

/// <summary>
/// A handle to a font. These should only be created by calling LoadFont().
/// </summary>
class Font
{
    public readonly IntPtr Handle;

    public Font(IntPtr handle)
    {
        Handle = handle;
    }
}

/// <summary>
/// A handle to a sound file. These should only be created by calling LoadSound().
/// </summary>
class Sound
{
    public readonly IntPtr Handle;

    public Sound(IntPtr handle)
    {
        Handle = handle;
    }
}

/// <summary>
/// A handle to a music file. These should only be created by calling LoadMusic().
/// </summary>
class Music
{
    public readonly IntPtr Handle;

    public Music(IntPtr handle)
    {
        Handle = handle;
    }
}
