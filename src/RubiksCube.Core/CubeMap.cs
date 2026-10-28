using System;
using System.Collections.Generic;
using System.Text;

namespace RubiksCube.Core
{
    public static class CubeMap
    {
        public static readonly int[][] CornerPositions =
        [
            [8, 9, 20],   // URF (Up-Right-Front)
            [6, 18, 38],  // UFL (Up-Front-Left)
            [0, 36, 47],  // ULB (Up-Left-Back)
            [2, 45, 11],  // UBR (Up-Back-Right)
            [29, 26, 15], // DFR (Down-Front-Right)
            [27, 24, 44], // DLF (Down-Left-Front)
            [33, 53, 42], // DBL (Down-Back-Left)
            [35, 17, 51]  // DRB (Down-Right-Back)
        ];

        public static readonly int[][] EdgePositions = 
        [
            [7, 19],   // UF (Up-Front)
            [5, 10],   // UR (Up-Right)
            [1, 46],   // UB (Up-Back)
            [3, 37],   // UL (Up-Left)
            [25, 28],  // DF (Down-Front)
            [32, 16],  // DR (Down-Right)
            [34, 52],  // DB (Down-Back)
            [30, 43],  // DL (Down-Left)
            [23, 12],  // FR (Front-Right)
            [21, 41],  // FL (Front-Left)
            [14, 48],  // BR (Back-Right)
            [39, 50]   // BL (Back-Left)
        ];
    }
}
