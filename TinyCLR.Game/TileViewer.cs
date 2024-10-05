using System;
using System.Drawing;

namespace TinyCLR.Game
{
    public class TileViewer : GameObject
    {
        public const int TileSize = 8;
        public const int HalfTileSize = TileSize / 2;
        public const int TileModMask = TileSize - 1;
        public const int TileSizeShift = 3;
        public const int ScrollMax = (int)(TileSize * 2) - 1;

        private const int MaxRepaintList = 10;

        private Bitmap _tileSheet;
        private Rect[] _tiles;
        private int[][] _map;

        private int _viewWidth;
        private int _viewHeight;
        private int _viewTilesX;
        private int _viewTilesY;
        private int _mapTilesX;
        private int _mapTilesY;
        private int _xLimit;
        private int _yLimit;

        private float _offsetX = 0;
        private float _offsetY = 0;
        private int _prevOffsetX = 0;
        private int _prevOffsetY = 0;

        private int _scrollX = 0;
        private int _scrollY = 0;

        private GameObject _trackedObject;
        private float _trackX;
        private float _trackY;

        private bool _fullRefresh = true;

        private readonly TileEntry[] _repaintList = new TileEntry[MaxRepaintList];
        private int _repaintCount;

        private Bitmap _layer;
        private Graphics _layerSurface;
        private Bitmap _backBuffer;
        private Graphics _backBufferSirface;

        public event ViewScrolledHandler ViewScrolled;

        public int TileCountX { get { return _mapTilesX; } }
        public int TileCountY { get { return _mapTilesY; } }

        public TileViewer()
        {
            ViewScrolled += (a, b, c) => { };
        }

        public void Initialize(Bitmap tileSheet, int viewTilesX, int viewTilesY, Rect[] tiles, int[][] map)
        {
            _tileSheet = tileSheet;
            _tiles = tiles;
            _map = map;

            _viewTilesX = viewTilesX;
            _viewTilesY = viewTilesY;

            _viewWidth = viewTilesX << TileSizeShift;
            _viewHeight = viewTilesY << TileSizeShift;

            _mapTilesX = _map[0].Length;
            _mapTilesY = _map.Length;

            _xLimit = (_mapTilesX << TileSizeShift) - _viewWidth;
            _yLimit = (_mapTilesY << TileSizeShift) - _viewHeight;

            _layer = new Bitmap(_viewWidth, _viewHeight);
            _backBuffer = new Bitmap(_viewWidth, _viewHeight);

            _layerSurface = Graphics.FromImage(_layer);
            _backBufferSirface = Graphics.FromImage(_backBuffer);
        }

        public void TrackObject(GameObject obj)
        {
            _trackedObject = obj;
            _trackX = obj.X;
            _trackY = obj.Y;
            CenterOnPixel((int)_trackX, (int)_trackY);
        }

        public void ScrollBy(int x, int y)
        {
            _scrollX = x;
            _scrollY = -y;
        }

        public void CenterOnPixel(int x, int y)
        {
            _offsetX = x - ((_viewTilesX >> 1) << TileSizeShift);
            _offsetY = y - ((_viewTilesY >> 1) << TileSizeShift);
            _fullRefresh = true;
        }

        public void CenterOnTile(int x, int y)
        {
            CenterOnPixel(x << TileSizeShift, y << TileSizeShift);
        }

        public int GetTileValue(int tileX, int tileY)
        {
            return _map[tileY][tileX];
        }

        public int GetTileValueAtPixel(int x, int y)
        {
            return _map[y >> TileSizeShift][x >> TileSizeShift];
        }

        public void SetTileValue(int tileX, int tileY, int tile)
        {
            _map[tileY][tileX] = tile;
            if (_repaintCount + 1 < MaxRepaintList)
            {
                _repaintList[_repaintCount++] = new TileEntry() { TileX = tileX, TileY = tileY, TileId = tile };
            }
        }

        public void SetTileValueAtPixel(int x, int y, int tile)
        {
            int tileX = x >> TileSizeShift;
            int tileY = y >> TileSizeShift;

            _map[tileY][tileX] = tile;
            if (_repaintCount + 1 < MaxRepaintList)
            {
                _repaintList[_repaintCount++] = new TileEntry() { TileX = tileX, TileY = tileY, TileId = tile };
            }
        }

        public override void Update(float elapsedTime)
        {
            float dx;
            float dy;

            if (_trackedObject == null)
            {
                dx = _scrollX * elapsedTime;
                dy = _scrollY * elapsedTime;
                _scrollY = _scrollX = 0;
            }
            else
            {
                dx = _trackedObject.X - _trackX;
                dy = _trackedObject.Y - _trackY;
                _trackX = _trackedObject.X;
                _trackY = _trackedObject.Y;
            }

            if (dx != 0)
            {
                float temp = _offsetX;
                _offsetX += dx;
                if (_offsetX < 0) _offsetX = 0;
                if (_offsetX >= _xLimit) _offsetX = _xLimit;

                dx = (_offsetX - temp);
                ChildOffsetX -= dx;
            }

            if (dy != 0)
            {
                float temp = _offsetY;
                _offsetY += dy;
                if (_offsetY < 0) _offsetY = 0;
                if (_offsetY >= _yLimit) _offsetY = _yLimit;

                dy = (_offsetY - temp);
                ChildOffsetY -= dy;
            }

            if (dx != 0 || dy != 0)
            {
                OnViewScrolled(dx, dy);
            }

            base.Update(elapsedTime);
        }

        public override void Draw()
        {
            //var sw = dotnetwarrior.NetMF.Diagnostics.Stopwatch.StartNew();

            int ox = (int)_offsetX;
            int oy = (int)_offsetY;
            int deltaX = ox - _prevOffsetX;
            int deltaY = oy - _prevOffsetY;

            int firstTileX = System.Math.Max(0, ox >> TileSizeShift);
            int firstTileY = System.Math.Max(0, oy >> TileSizeShift);

            int xTileLimit = System.Math.Min(_mapTilesX, firstTileX + _viewTilesX + 1);
            int yTileLimit = System.Math.Min(_mapTilesY, firstTileY + _viewTilesY + 1);

            int adjustX = ox & TileModMask;
            int adjustY = oy & TileModMask;

            if (System.Math.Abs(deltaX) > ScrollMax || System.Math.Abs(deltaY) > ScrollMax)
            {
                _fullRefresh = true;
            }

            int tileId;
            Rect rc;
            if (_fullRefresh)
            {
                for (int y = -adjustY, yTile = firstTileY; yTile < yTileLimit; y += TileSize, yTile++)
                {
                    for (int x = -adjustX, xTile = firstTileX; xTile < xTileLimit; x += TileSize, xTile++)
                    {
                        tileId = _map[yTile][xTile];
                        if (tileId == -1) continue;
                        rc = _tiles[tileId];
                        _layerSurface.DrawImage(x, y, _tileSheet, rc.X, rc.Y, rc.Width, rc.Height, 255);
                    }
                }
                _fullRefresh = false;
            }
            else if (deltaX != 0 || deltaY != 0)
            {
                int adx = System.Math.Abs(deltaX);
                int ady = System.Math.Abs(deltaY);

                // Scroll the background image
                _backBufferSirface.DrawImage(
                  deltaX < 0 ? adx : 0,
                  deltaY < 0 ? ady : 0,
                  _layer,
                  deltaX > 0 ? adx : 0,
                  deltaY > 0 ? ady : 0,
                  _viewWidth - adx, _viewHeight - ady, 255);

                var tempSurface = _layerSurface;
                _layerSurface = _backBufferSirface;
                _backBufferSirface = tempSurface;

                var tempBmp = _layer;
                _layer = _backBuffer;
                _backBuffer = tempBmp;

                // Update the edges with the new tiles that need to be rendered after the scroll
                if (deltaX > 0) // We moved left we need to repaint the right edge
                {
                    int xStartTile = firstTileX + _viewTilesX - 1;
                    int xLastTile = xTileLimit - 1;
                    int x1 = ((_viewTilesX - 1) << TileSizeShift) - adjustX;
                    int x2 = x1 + TileSize;

                    for (int yTile = firstTileY, y = -adjustY; yTile < yTileLimit; yTile++, y += TileSize)
                    {
                        int[] row = _map[yTile];

                        rc = _tiles[row[xStartTile]];
                        _layerSurface.DrawImage(x1, y, _tileSheet, rc.X, rc.Y, rc.Width, rc.Height, 255);

                        rc = _tiles[row[xLastTile]];
                        _layerSurface.DrawImage(x2, y, _tileSheet, rc.X, rc.Y, rc.Width, rc.Height, 255);
                    }
                }
                else if (deltaX < 0) // We moved right we need to repaint the left edge
                {
                    int xLastTile = firstTileX + 1;
                    int x1 = -adjustX;
                    int x2 = x1 + TileSize;
                    for (int yTile = firstTileY, y = -adjustY; yTile < yTileLimit; yTile++, y += TileSize)
                    {
                        int[] row = _map[yTile];

                        rc = _tiles[row[firstTileX]];
                        _layerSurface.DrawImage(x1, y, _tileSheet, rc.X, rc.Y, rc.Width, rc.Height, 255);

                        rc = _tiles[row[xLastTile]];
                        _layerSurface.DrawImage(x2, y, _tileSheet, rc.X, rc.Y, rc.Width, rc.Height, 255);
                    }
                }

                if (deltaY > 0) // We moved down we need to repaint the bottom edge
                {
                    int yLastTile = firstTileY + _viewTilesY - 1;

                    int[] row1 = _map[yLastTile];
                    int[] row2 = _map[yTileLimit - 1];
                    int y1 = ((_viewTilesY - 1) << TileSizeShift) - adjustY;
                    int y2 = y1 + TileSize;
                    for (int xTile = firstTileX, x = -adjustX; xTile < xTileLimit; xTile++, x += TileSize)
                    {
                        rc = _tiles[row1[xTile]];
                        _layerSurface.DrawImage(x, y1, _tileSheet, rc.X, rc.Y, rc.Width, rc.Height, 255);

                        rc = _tiles[row2[xTile]];
                        _layerSurface.DrawImage(x, y2, _tileSheet, rc.X, rc.Y, rc.Width, rc.Height, 255);
                    }
                }
                else if (deltaY < 0) // We moved up we need to repaint the top edge
                {
                    int[] row1 = _map[firstTileY];
                    int[] row2 = _map[firstTileY + 1];
                    int y1 = -adjustY;
                    int y2 = y1 + TileSize;
                    for (int xTile = firstTileX, x = -adjustX; xTile < xTileLimit; xTile++, x += TileSize)
                    {
                        rc = _tiles[row1[xTile]];
                        _layerSurface.DrawImage(x, y1, _tileSheet, rc.X, rc.Y, rc.Width, rc.Height, 255);

                        rc = _tiles[row2[xTile]];
                        _layerSurface.DrawImage(x, y2, _tileSheet, rc.X, rc.Y, rc.Width, rc.Height, 255);
                    }
                }
            }

            for (int i = 0; i < _repaintCount; i++)
            {
                TileEntry entry = _repaintList[i];
                int x = (entry.TileX << TileSizeShift) - ox;
                int y = (entry.TileY << TileSizeShift) - oy;
                rc = _tiles[entry.TileId];
                _layerSurface.DrawImage(x, y, _tileSheet, rc.X, rc.Y, rc.Width, rc.Height, 255);
            }
            _repaintCount = 0;

            _prevOffsetX = ox;
            _prevOffsetY = oy;

            // Ensure that all child objects are scrolled to the correct offset
            ChildOffsetX = -ox;
            ChildOffsetY = -oy;

            // Render buffer to the surface
            Surface.DrawImage(WorldX, WorldY, _layer, 0, 0, _viewWidth, _viewHeight, 255);

            Surface.SetClippingRectangle(WorldX, WorldY, _viewWidth, _viewHeight);
            base.Draw();
            Surface.SetClippingRectangle(0, 0, Surface.Width, Surface.Height);
            //sw.Stop();
            //Debug.Print(sw.ElapsedMilliseconds.ToString());
        }

        protected void OnViewScrolled(float dx, float dy)
        {
            ViewScrolled(this, dx, dy);
        }
    }

    public struct TileEntry
    {
        public int TileX;
        public int TileY;
        public int TileId;
    }

    /// <summary>
    /// Delegate called to notify of scroll changes of the TileViewer.
    /// </summary>
    /// <param name="sender">TileViewer instance that scrolled.</param>
    /// <param name="dx">Change in X.</param>
    /// <param name="dy">Change in Y.</param>
    public delegate void ViewScrolledHandler(TileViewer sender, float dx, float dy);
}
