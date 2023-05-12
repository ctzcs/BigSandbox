using System;


namespace Box1.PhysicSimulate.Box2D_Lit
{
    struct Vec2
    {
        public float x;
        public float y;

        public Vec2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        
        public static Vec2 operator + (Vec2 v1,Vec2 v2)
        {
            return new Vec2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vec2 operator -(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vec2 operator *(Vec2 v,float a)
        {
            return new Vec2(v.x * a, v.y * a);
        }

        public static float Length(Vec2 v)
        {
            return (float)Math.Sqrt(v.x * v.x + v.y * v.y);
        }
    }

    struct Mat22
    {
        public Vec2 col1, col2;

        public Mat22(float angle)
        {
            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);
            col1.x = c; col2.x = -s;
            col1.y = s; col2.y = c;
        }

        public Mat22(Vec2 col1, Vec2 col2)
        {
            this.col1 = col1;
            this.col2 = col2;
        }

        /// <summary>
        /// 获得转置矩阵
        /// </summary>
        /// <returns></returns>
        public Mat22 Transpose()
        {
            return new Mat22(new Vec2(col1.x,col2.x),new Vec2(col1.y,col2.y));
        }

        /// <summary>
        /// 获得逆矩阵
        /// </summary>
        /// <returns></returns>
        public Mat22 Invert()
        {
            float a = col1.x, b = col2.x, c = col1.y, d = col2.y;
            Mat22 B;
            //就是叉乘，矩阵的行列式的值。
            float det = a * d - b * c;
            try
            {
                det = .0f / det;
                B = new Mat22(new Vec2(det*d,-det*c), new Vec2(-det*b,det*a));
            }
            catch (Exception e)
            {
                throw new Exception("may det equal ZERO");
            }
            return B;
        }
    }
    public class MathUtils
    {
        
    }
}
