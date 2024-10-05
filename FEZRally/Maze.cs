using System;
using TinyCLR.Game;
using System.Collections;
using System.Drawing;

namespace FEZRally
{
    class Maze : TileViewer
    {
        private static int[][] _map = new int[][]
        {
      new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 9, 0, 9, 9, 9, 9, 9, 0, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 9, 0, 9, 9, 9, 9, 9, 0, 9, 9, 0, 0, 0, 9, 0, 0, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 9, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 9, 9, 0, 9, 9, 9, 0, 9, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 9, 9, 9, 9, 9, 9, 0, 9, 0, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 0, 0, 0, 0, 0, 0, 9, 0, 9, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 0, 9, 9, 9, 9, 0, 9, 0, 9, 0, 9, 9, 9, 0, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 9, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 0, 9, 0, 0, 0, 0, 9, 0, 9, 0, 9, 9, 9, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 0, 9, 0, 9, 9, 0, 9, 0, 9, 0, 0, 0, 0, 0, 9, 9, 9, 0, 9, 0, 0, 0, 0, 0, 0, 0, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 9, 0, 9, 0, 9, 9, 0, 9, 0, 9, 0, 9, 9, 9, 0, 9, 9, 9, 0, 9, 0, 9, 9, 9, 9, 9, 0, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 0, 9, 0, 0, 0, 0, 9, 0, 9, 0, 9, 9, 9, 0, 9, 9, 9, 0, 9, 0, 9, 9, 9, 9, 9, 0, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 0, 9, 0, 9, 9, 9, 9, 0, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 9, 0, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 0, 0, 9, 9, 9, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 0, 9, 0, 9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 0, 0, 9, 9, 9, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 0, 0, 9, 9, 9, 9, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 9, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 9, 9, 0, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 0, 9, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 9, 9, 0, 9, 9, 0, 9, 9, 9, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 9, 9, 9, 0, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 9, 9, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 9, 9, 0, 0, 9, 9, 9, 0, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 0, 0, 9, 9, 0, 0, 9, 9, 9, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 9, 9, 0, 0, 9, 9, 9, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 9, 9, 0, 9, 9, 0, 9, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 9, 9, 0, 9, 9, 0, 9, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 9, 9, 0, 9, 9, 0, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 9, 9, 0, 0, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 9, 9, 0, 9, 9, 0, 9, 9, 0, 9, 0, 9, 9, 0, 0, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 0, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 9, 9, 9, 0, 0, 0, 0, 0, 9, 9, 0, 9, 0, 9, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 9, 0, 9, 9, 9, 0, 9, 9, 9, 9, 0, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 0, 0, 0, 9, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 9, 0, 9, 9, 9, 0, 9, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 0, 9, 9, 0, 0, 9, 9, 9, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 0, 9, 9, 9, 9, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 9, 9, 9, 9, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 9, 9, 0, 9, 9, 9, 0, 9, 9, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 9, 9, 0, 9, 9, 9, 0, 9, 9, 9, 0, 9, 9, 0, 9, 9, 9, 0, 9, 9, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 9, 9, 0, 9, 9, 9, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 9, 0, 9, 9, 9, 0, 9, 9, 9, 0, 9, 9, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 0, 0, 0, 9, 0, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 9, 9, 0, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 0, 9, 9, 9, 0, 9, 0, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 9, 9, 0, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 0, 9, 9, 9, 0, 9, 0, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 9, 0, 9, 9, 9, 0, 9, 9, 0, 9, 9, 9, 9, 9, 0, 9, 9, 9, 0, 9, 0, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 9, 0, 9, 9, 9, 0, 9, 9, 0, 9, 9, 9, 9, 9, 0, 9, 9, 9, 0, 9, 0, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 9, 0, 9, 9, 9, 0, 9, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 9, 9, 0, 9, 9, 9, 0, 9, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 9, 9, 0, 0, 0, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 0, 0, 0, 0, 9, 9, 9, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 0, 0, 0, 0, 9, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 9, 0, 0, 9, 9, 9, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 0, 9, 9, 9, 9, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 9, 0, 0, 0, 0, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 9, 9, 0, 9, 9, 9, 9, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 0, 9, 9, 0, 0, 9, 9, 0, 9, 9, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9, 0, 0, 0, 0, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
      new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        };

        public const int TotalFlags = 10;
        private const int TotalRocks = 5;

        private readonly Bitmap _miniMap;
        private readonly Graphics _miniMapSurface;
    
        private readonly int _centerTileX;
        private readonly int _centerTileY;
        private readonly TileEntry[] _flags = new TileEntry[TotalFlags];

        private GameObject _player;
        private readonly GameObject[] _enemies = new Enemy[5];
        private int _enemyCount;

        private readonly CountDownTimer _smokeBombTimer = new CountDownTimer(5000);
        private readonly Queue _bombs = new Queue();

        public Maze(Bitmap tileSheet, int viewTilesX, int viewTilesY)
        {
            PreprocessMap();

            var tileRects = Utility.TileExtractor(0, 8, 8, 8, 8, 3, 0);

            Initialize(tileSheet, viewTilesX, viewTilesY, tileRects, _map);
            _miniMap = new Bitmap(TileCountX, TileCountY);
            _miniMapSurface = Graphics.FromImage(_miniMap);
           
            Utility.ClearBitmap(_miniMapSurface, Brushes.MiniMap);

            _centerTileX = _map[0].Length / 2;
            _centerTileY = _map.Length / 2;

            _smokeBombTimer.Expired += new EventHandler(SmokeBombTimer_Expired);
        }

        void SmokeBombTimer_Expired(object sender, EventArgs e)
        {
            TileEntry smokeBomb = (TileEntry)_bombs.Dequeue();
            SetTileValue(smokeBomb.TileX, smokeBomb.TileY, smokeBomb.TileId);
            if (_bombs.Count > 0)
            {
                _smokeBombTimer.Start(2000);
            }
        }

        public TileEntry[] Flags { get { return _flags; } }

        public void AddPlayer(GameObject player)
        {
            _player = player;
            _player.Owner = this;
        }

        public void AddEnemy(GameObject enemy)
        {
            _enemies[_enemyCount++] = enemy;
            enemy.Owner = this;
        }

        public int CenterTileX { get { return _centerTileX; } }
        public int CenterTileY { get { return _centerTileY; } }

        public bool IsCellOpen(GameCharacter character, Direction direction)
        {
            int x = character.X >> TileViewer.TileSizeShift;
            int y = character.Y >> TileViewer.TileSizeShift;
            switch (direction)
            {
                case Direction.Left: return character.IsCellOpen(_map[y][x - 1]);
                case Direction.Right: return character.IsCellOpen(_map[y][x + 1]);
                case Direction.Up: return character.IsCellOpen(_map[y - 1][x]);
                case Direction.Down: return character.IsCellOpen(_map[y + 1][x]);
            }

            return false;
        }

        public bool IsColliding(int x1, int y1, int x2, int y2)
        {
            // Objects are colliding if centers are within
            // 12 pixels of each other. 
            // Eliminated sqrt for performance.
            int dx = (x1 + HalfTileSize) - (x2 + HalfTileSize);
            int dy = (y1 + HalfTileSize) - (y2 + HalfTileSize);

            return ((dx * dx) + (dy * dy)) < 144;
        }

        private readonly Direction[] _openDirections = new Direction[4];
        public Direction[] GetOpenDirections(GameCharacter character, Direction current)
        {
            int i = 0;

            _openDirections[0] = Direction.Stop;
            _openDirections[1] = Direction.Stop;
            _openDirections[2] = Direction.Stop;
            _openDirections[3] = Direction.Stop;

            if (current != Direction.Right && IsCellOpen(character, Direction.Left)) _openDirections[i++] = Direction.Left;
            if (current != Direction.Left && IsCellOpen(character, Direction.Right)) _openDirections[i++] = Direction.Right;
            if (current != Direction.Down && IsCellOpen(character, Direction.Up)) _openDirections[i++] = Direction.Up;
            if (current != Direction.Up && IsCellOpen(character, Direction.Down)) _openDirections[i++] = Direction.Down;

            if (i == 0)
            {
                if (current == Direction.Right && IsCellOpen(character, Direction.Left)) _openDirections[i++] = Direction.Left;
                else if (current == Direction.Left && IsCellOpen(character, Direction.Right)) _openDirections[i++] = Direction.Right;
                else if (current == Direction.Down && IsCellOpen(character, Direction.Up)) _openDirections[i++] = Direction.Up;
                else if (current == Direction.Up && IsCellOpen(character, Direction.Down)) _openDirections[i++] = Direction.Down;
            }

            return _openDirections;
        }

        private void DrawItem(int x, int y, Color color)
        {
            _miniMapSurface.SetPixel(x, y - 1, color);
            _miniMapSurface.SetPixel(x - 1, y, color);
            _miniMapSurface.SetPixel(x + 1, y, color);
            _miniMapSurface.SetPixel(x, y + 1, color);
        }
        public Bitmap GetMiniMap()
        {
            Utility.ClearBitmap(_miniMapSurface, Brushes.MiniMap);

            for (int i = 0; i < TotalFlags; i++)
            {
                TileEntry flag = _flags[i];

                if (GetTileValue(flag.TileX, flag.TileY) == Tile.Flag)
                {
                    DrawItem(flag.TileX, flag.TileY, Color.Yellow);
                }
            }

            for (int i = 0; i < _enemyCount; i++)
            {
                GameObject enemy = _enemies[i];
                DrawItem(enemy.X >> TileViewer.TileSizeShift, enemy.Y >> TileViewer.TileSizeShift, Color.Red);
            }

            DrawItem(_player.X >> TileViewer.TileSizeShift, _player.Y >> TileViewer.TileSizeShift, Color.White);

            return _miniMap;
        }

        public bool DropSmokeBomb(int tileX, int tileY)
        {
            if (_bombs.Count > 15) return false;

            int tileId = GetTileValue(tileX, tileY);
            if (tileId >= Tile.Empty && tileId != Tile.Smoke)
            {
                SetTileValue(tileX, tileY, Tile.Smoke);
                _bombs.Enqueue(new TileEntry() { TileX = tileX, TileY = tileY, TileId = tileId });
                if (!_smokeBombTimer.IsRunning)
                {
                    _smokeBombTimer.Start();
                }
                return true;
            }

            return false;
        }

        public void ClearSmokeBomb(int tileX, int tileY)
        {
            foreach (TileEntry entry in _bombs)
            {
                if (entry.TileX == tileX && entry.TileY == tileY)
                {
                    SetTileValue(tileX, tileY, entry.TileId);
                }
            }
        }

        public void GenerateLevel()
        {
            int mapHeight = _map.Length;
            int mapWidth = _map[0].Length;

            _smokeBombTimer.Cancel();
            _bombs.Clear();

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    switch (_map[y][x])
                    {
                        case Tile.Flag:
                        case Tile.Rock:
                        case Tile.Smoke: _map[y][x] = Tile.Empty; break;
                    }
                }
            }

            int flagCount = 0;
            while (flagCount < TotalFlags)
            {
                int x = Utility.GetRandomNumber(mapWidth);
                int y = Utility.GetRandomNumber(mapHeight);
                if (x != 20 && y != 50 && _map[y][x] == Tile.Empty)
                {
                    _map[y][x] = Tile.Flag;
                    _flags[flagCount] = new TileEntry() { TileX = x, TileY = y, TileId = Tile.Flag };
                    flagCount++;
                }
            }

            int rockCount = 0;
            while (rockCount < TotalRocks)
            {
                int x = Utility.GetRandomNumber(mapWidth);
                int y = Utility.GetRandomNumber(mapHeight);
                if (x != 20 && y != 50 && _map[y][x] == Tile.Empty)
                {
                    _map[y][x] = Tile.Rock;
                    rockCount++;
                }
            }
        }

        private void PreprocessMap()
        {
            int[] walls = new int[16];
            walls[0] = 16;
            walls[1] = 4;
            walls[2] = 1;
            walls[3] = 13;
            walls[4] = 2;
            walls[5] = 6;
            walls[6] = 11;
            walls[7] = 10;
            walls[8] = 3;
            walls[9] = 14;
            walls[10] = 5;
            walls[11] = 9;
            walls[12] = 12;
            walls[13] = 8;
            walls[14] = 7;
            walls[15] = 15;

            int[][] newMap = new int[_map.Length][];
            int mapHeight = _map.Length;
            int mapWidth = _map[0].Length;

            for (int y = 0; y < mapHeight; y++)
            {
                newMap[y] = new int[mapWidth];
                for (int x = 0; x < mapWidth; x++)
                {
                    switch (_map[y][x])
                    {
                        case 1: newMap[y][x] = 0; break;
                        case 0: newMap[y][x] = 17; break;
                        case 9:
                            {
                                int neighborMask = GetWallNeighborMask(x, y);
                                newMap[y][x] = walls[neighborMask];
                            }
                            break;
                        default:
                            newMap[y][x] = _map[y][x];
                            break;
                    }
                }
            }
            _map = newMap;
        }

        private int GetWallNeighborMask(int x, int y)
        {
            int result = 0;
            if (x > 0 && _map[y][x - 1] == 9) result |= 8;
            if (x < _map[y].Length - 1 && _map[y][x + 1] == 9) result |= 2;
            if (y > 0 && _map[y - 1][x] == 9) result |= 1;
            if (y < _map.Length - 1 && _map[y + 1][x] == 9) result |= 4;
            return result;
        }
    }
}
