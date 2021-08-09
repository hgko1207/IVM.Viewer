using System;
using GlmNet;

namespace IVM.Studio.I3D
{
    public class I3DCommon
    {
        public static float Deg2Rad(float deg)
        {
            return deg / 180.0f * (float)Math.PI;
        }

        public static int IntParse(string s)
        {
            int v = 0;
            int.TryParse(s, out v);
            return v;
        }

        public static float FloatParse(string s)
        {
            float v = 0;
            float.TryParse(s, out v);
            return v;
        }


        private static bool ray_to_plane(vec3 orig, vec3 dir, I3DPlane pln, ref float OutT, ref float OutVD)
        {
            OutVD = pln.a * dir.x + pln.b * dir.y + pln.c * dir.z;
            if (OutVD == 0.0f)
                return false;

            OutT = -(pln.a * orig.x + pln.b * orig.y + pln.c * orig.z + pln.d) / OutVD;
            return true;
        }

        private static void sort_points(ref vec3[] points, int point_count, I3DPlane pln)
        {
            if (point_count == 0) 
                return;

            vec3[] points2 = new vec3[point_count];
            Array.Copy(points, points2, point_count);

            vec3 plane_normal = new vec3(pln.a, pln.b, pln.c);
            vec3 origin = points[0];

            Array.Sort(points2, (lhs, rhs) => {
                vec3 v = glm.cross(lhs - origin, rhs - origin);
                if (glm.dot(v, plane_normal) < 0)
                    return -1;
                return 1;
            });

            Array.Copy(points2, points, point_count);
        }

        // reference: https://www.asawicki.info/news_1428_finding_polygon_of_plane-aabb_intersection
        // Maximum out_point_count == 6, so out_points must point to 6-element array.
        // out_point_count == 0 mean no intersection.
        // out_points are not sorted.
        public static void calc_plane_aabb_intersection_points(I3DPlane pln, vec3 aabb_min, vec3 aabb_max,
            ref vec3[] out_points, ref int out_point_count)
        {
            out_point_count = 0;
            float vd = 0.0f, t = 0.0f;

            // Test edges along X axis, pointing right.
            vec3 dir = new vec3(aabb_max.x - aabb_min.x, 0.0f, 0.0f);
            vec3 orig = aabb_min;
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;
            orig = new vec3(aabb_min.x, aabb_max.y, aabb_min.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;
            orig = new vec3(aabb_min.x, aabb_min.y, aabb_max.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;
            orig = new vec3(aabb_min.x, aabb_max.y, aabb_max.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;

            // Test edges along Y axis, pointing up.
            dir = new vec3(0.0f, aabb_max.y - aabb_min.y, 0.0f);
            orig = new vec3(aabb_min.x, aabb_min.y, aabb_min.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;
            orig = new vec3(aabb_max.x, aabb_min.y, aabb_min.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;
            orig = new vec3(aabb_min.x, aabb_min.y, aabb_max.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;
            orig = new vec3(aabb_max.x, aabb_min.y, aabb_max.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;

            // Test edges along Z axis, pointing forward.
            dir = new vec3(0.0f, 0.0f, aabb_max.z - aabb_min.z);
            orig = new vec3(aabb_min.x, aabb_min.y, aabb_min.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;
            orig = new vec3(aabb_max.x, aabb_min.y, aabb_min.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;
            orig = new vec3(aabb_min.x, aabb_max.y, aabb_min.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;
            orig = new vec3(aabb_max.x, aabb_max.y, aabb_min.z);
            if (ray_to_plane(orig, dir, pln, ref t, ref vd) && t >= 0.0f && t <= 1.0f)
                out_points[out_point_count++] = orig + dir * t;

            sort_points(ref out_points, out_point_count, pln);
        }
    }
}
