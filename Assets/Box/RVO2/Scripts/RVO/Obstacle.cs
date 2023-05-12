/*
 * Obstacle.cs
 * RVO2 Library C#
 *
 * Copyright 2008 University of North Carolina at Chapel Hill
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Please send all bug reports to <geom@cs.unc.edu>.
 *
 * The authors may be contacted via:
 *
 * Jur van den Berg, Stephen J. Guy, Jamie Snape, Ming C. Lin, Dinesh Manocha
 * Dept. of Computer Science
 * 201 S. Columbia St.
 * Frederick P. Brooks, Jr. Computer Science Bldg.
 * Chapel Hill, N.C. 27599-3175
 * United States of America
 *
 * <http://gamma.cs.unc.edu/RVO2/>
 */

namespace RVO
{
    /**
     * <summary>定义模拟中的静态碰撞体。Defines static obstacles in the simulation.</summary>
     */
    internal class Obstacle
    {

        /// <summary>
        /// 下一个障碍物
        /// </summary>
        internal Obstacle next_;
        /// <summary>
        /// 上一个障碍物
        /// </summary>
        internal Obstacle previous_;
        /// <summary>
        /// 朝向
        /// </summary>
        internal Vector2 direction_;
        /// <summary>
        /// 位置
        /// </summary>
        internal Vector2 point_;
        /// <summary>
        /// id
        /// </summary>
        internal int id_;
        /// <summary>
        /// 是否是凸包
        /// </summary>
        internal bool convex_;
    }
}