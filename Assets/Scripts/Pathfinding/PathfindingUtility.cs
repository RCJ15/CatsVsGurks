using UnityEngine;

public static class PathfindingUtility
{
    public static Vector3 RotatePoint(Vector3 p, Vector3 pivot, float angleDegrees)
    {
        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleRadians);
        float sin = Mathf.Sin(angleRadians);

        // Translate point to origin
        float dx = p.x - pivot.x;
        float dy = p.y - pivot.y;

        // Rotate
        float xNew = dx * cos - dy * sin;
        float yNew = dx * sin + dy * cos;

        // Translate back
        return new((xNew + pivot.x), (yNew + pivot.y));
    }

    public static bool PointInRotatedBox(
        Vector2 p,
        Vector2 center,
        Vector2 size,
        float rotationDegrees)
    {
        float theta = rotationDegrees * Mathf.Deg2Rad;

        // Translate point to rectangle center
        float translatedX = p.x - center.x;
        float translatedY = p.y - center.y;

        float cosTheta = Mathf.Cos(theta);
        float sinTheta = Mathf.Sin(theta);

        // Rotate point in opposite direction
        float rotatedX = translatedX * cosTheta - translatedY * sinTheta;
        float rotatedY = translatedX * sinTheta + translatedY * cosTheta;

        /*
        Vector3 offset = new Vector2(0, 3);

        bool result = Mathf.Abs(rotatedX) <= size.x / 2f && Mathf.Abs(rotatedY) <= size.y / 2f;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(translatedX, translatedY) + offset, 0.05f);
        Gizmos.color = result ? Color.green : Color.red;
        Gizmos.DrawSphere(new Vector3(rotatedX, rotatedY) + offset, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(offset, size);
        */

        // Check if within unrotated rectangle bounds
        return Mathf.Abs(rotatedX) <= size.x / 2f && Mathf.Abs(rotatedY) <= size.y / 2f;
    }

    public static bool PointInBox(Vector2 p, Vector2 center, Vector2 size)
    {
        Vector2 halfSize = size / 2;

        Vector2 bottomLeft = center - halfSize;
        Vector2 topRight = center + halfSize;

        return 
            p.x >= bottomLeft.x && p.y >= bottomLeft.y
            &&
            p.x <= topRight.x && p.y <= topRight.y;
    }

    public static bool PointInCircle(Vector2 p, Vector2 center, float radius)
    {
        float dx = center.x - p.x;
        float dy = center.y - p.y;

        return (dx * dx + dy * dy) <= (radius * radius);
    }

    public static bool CircleIntersectsBox(Vector2 circleCenter, float circleRadius, Vector2 boxCenter, Vector2 boxSize)
    {
        Vector2 halfBoxSize = boxSize / 2f;

        float closestX = Mathf.Clamp(circleCenter.x, boxCenter.x - halfBoxSize.x, boxCenter.x + halfBoxSize.x);
        float closestY = Mathf.Clamp(circleCenter.y, boxCenter.y - halfBoxSize.y, boxCenter.y + halfBoxSize.y);

        return PointInCircle(new(closestX, closestY), circleCenter, circleRadius);
    }

    // Define region codes
    private const int INSIDE = 0;   // 0000
    private const int LEFT = 1;     // 0001
    private const int RIGHT = 2;    // 0010
    private const int BOTTOM = 4;   // 0100
    private const int TOP = 8;      // 1000

    // Cohen–Sutherland clipping algorithm
    public static bool LineIntersectsBox(Vector2 p1, Vector2 p2, Vector2 center, Vector2 size)
    {
        Vector2 halfSize = size / 2f;
        Vector2 min = center - halfSize;
        Vector2 max = center + halfSize;

        int ComputeCode(float x, float y)
        {
            int code = INSIDE;

            if (x < min.x) code |= LEFT;
            else if (x > max.x) code |= RIGHT;
            if (y < min.y) code |= BOTTOM;
            else if (y > max.y) code |= TOP;

            return code;
        }

        int code1 = ComputeCode(p1.x, p1.y);
        int code2 = ComputeCode(p2.x, p2.y);
        bool accept = false;

        while (true)
        {
            if ((code1 | code2) == 0)
            {
                // Both points inside
                accept = true;
                break;
            }
            else if ((code1 & code2) != 0)
            {
                // Both points share an outside zone -> reject
                break;
            }
            else
            {
                // At least one endpoint is outside
                float x = 0, y = 0;
                int codeOut = (code1 != 0) ? code1 : code2;

                // Find intersection point
                if ((codeOut & TOP) != 0)
                {
                    x = p1.x + (p2.x - p1.x) * (max.y - p1.y) / (p2.y - p1.y);
                    y = max.y;
                }
                else if ((codeOut & BOTTOM) != 0)
                {
                    x = p1.x + (p2.x - p1.x) * (min.y - p1.y) / (p2.y - p1.y);
                    y = min.y;
                }
                else if ((codeOut & RIGHT) != 0)
                {
                    y = p1.y + (p2.y - p1.y) * (max.x - p1.x) / (p2.x - p1.x);
                    x = max.x;
                }
                else if ((codeOut & LEFT) != 0)
                {
                    y = p1.y + (p2.y - p1.y) * (min.x - p1.x) / (p2.x - p1.x);
                    x = min.x;
                }

                // Replace outside point with intersection point
                if (codeOut == code1)
                {
                    p1.x = x;
                    p1.y = y;
                    code1 = ComputeCode(p1.x, p1.y);
                }
                else
                {
                    p2.x = x;
                    p2.y = y;
                    code2 = ComputeCode(p2.x, p2.y);
                }
            }
        }

        return accept;
    }
}
