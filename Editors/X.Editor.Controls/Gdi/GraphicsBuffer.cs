using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace X.Editor.Controls.Gdi
{
    public class GraphicsBuffer
    {
        Bitmap _dataBuffer;
        Graphics _graphics;
        bool _antiAliasing;

        ManualResetEventSlim _waitHandle = new ManualResetEventSlim(false);

        public GraphicsBuffer(Size size)
        {
            _antiAliasing = true;
            Init(size);
            _waitHandle.Set();
        }

        public Bitmap Copy()
        {
            _waitHandle.Wait();
            Bitmap clone = (Bitmap)_dataBuffer.Clone();
            BitmapData data = clone.LockBits(new Rectangle(0, 0, clone.Width, clone.Height), ImageLockMode.ReadOnly, clone.PixelFormat);
            clone.UnlockBits(data);
            //var clone = new Bitmap(_dataBuffer);
            _waitHandle.Set();
            return clone;
        }

        public void FlushTo(Graphics graphics)
        {
            _waitHandle.Wait();
            graphics.DrawImageUnscaled(_dataBuffer, 0, 0);
            _waitHandle.Set();
        }

        public void Draw(Action<Graphics> drawingMethod)
        {
            _waitHandle.Wait();
            try
            {
                drawingMethod(_graphics);
            }
            catch { }
            _waitHandle.Set();
        }

        public void Resize(Size size)
        {
            _waitHandle.Wait();
            Init(size);
            _waitHandle.Set();
        }

        void Init(Size size)
        {
            if (size == Size.Empty) size = new Size(1, 1);
            Release();

            _dataBuffer = new Bitmap(size.Width, size.Height);
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


        void Release()
        {
            if (_dataBuffer != null) _dataBuffer.Dispose();
            if (_graphics != null) _graphics.Dispose();
        }
    }
}
