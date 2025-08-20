namespace ThermalDemo.Draw.Interfaces.Service
{
    public interface IThermalDrawService
    {
        void DrawLine(double x1, double y1, double x2, double y2);
        void DrawBox(double left, double top, double width, double height);
        void DrawEllipse(double left, double top, double width, double height);
        void DeleteShape(string key);
        void DeleteAllShapes();
        void ApplyRatiosToAllShapes(double widthRatio, double heightRatio);
        void RotateShapeAdditionalDegrees(string key, double addedAngle);
        void OffsetShape(string key, double x, double y);
        void MoveShapeTo(string key, double x, double y);
        void ResizeShape(string key, double width, double height);
        void MoveAndResizeShape(string key, double x, double y, double width, double height);
        void LineMoveStartTo(string key, double x, double y);
        void LineMoveEndTo(string key, double x, double y);
        void MoveShapeComplete(string key);

        void ApplyRatiosToAllShapesInImage(string imageKey, double widthRatio, double heightRatio);
        void DrawLine(string imageKey, double x1, double y1, double x2, double y2, string stroke);
        void DrawBox(string imageKey, double left, double top, double width, double height, string stroke);
        void DrawEllipse(string imageKey, double left, double top, double width, double height, string stroke);
    }
}
