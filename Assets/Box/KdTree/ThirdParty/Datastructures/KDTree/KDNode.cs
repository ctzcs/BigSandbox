﻿/*MIT License

Copyright(c) 2018 Vili Volčini / viliwonka

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures.ViliWonka.KDTree {

    /// <summary>
    /// 节点
    /// </summary>
    public class KDNode {

        /// <summary>
        /// 对齐的轴
        /// </summary>
        public float partitionCoordinate;
        public int partitionAxis = -1;

        public KDNode negativeChild;
        public KDNode positiveChild;

        public int start;
        public int end;

        public int Count { get { return end - start; } }

        public bool Leaf { get { return partitionAxis == -1; } }

        public KDBounds bounds;

    };

}
