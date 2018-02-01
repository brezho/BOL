using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace X.Editor.Controls.Gdi
{
    public class GraphicsBuffer
    {
        Bitmap _dataBuffer;
        Size _size;
        Graphics _graphics;
        bool _antiAliasing;


        public GraphicsBuffer(Size size)
        {
            _size = size;
            _antiAliasing = true;
            Init();
        }
        public Bitmap Buffer => _dataBuffer;

        public Graphics Graphics
        {
            get
            {
                return _graphics;
            }
        }

        public void Resize(Size size)
        {
            Release();
            _size = size;
            Init();
        }

        void Init()
        {
            if (_size == Size.Empty) return;
            _dataBuffer = new Bitmap(_size.Width, _size.Height);
            _graphics = Graphics.FromImage(_dataBuffer);

            // Antialiased Polygons and Text?
            if (_antiAliasing)
            {
                _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                _graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }

            _graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            _graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            _graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            _graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
        }

        public void FlushTo(Graphics graphics)
        {
            graphics.Clear(Color.Black);
            graphics.DrawImageUnscaled(_dataBuffer, 0, 0);
        }

        void Release()
        {
            if (_dataBuffer != null) _dataBuffer.Dispose();
            if (_graphics != null) _graphics.Dispose();
        }
    }
}
