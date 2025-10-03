using System.Reflection;
using System.Linq;

public static class MP4Paths
{
    public const string CommonStart = "StartCommon.mp4";
    public const string CommonRay = "Rare_ray.mp4";
    public const string EpicStart = "StartEpic.mp4";
    public const string EpicRay = "Epic_ray.mp4";
    public const string LegendaryStart = "StartLegend.mp4";
    public const string LegendaryRay = "Legend_ray.mp4";

    public static string[] GetAllFileNames()
    {
        var fields = typeof(MP4Paths).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(string)).ToArray();

        var fileNames = new string[fields.Length];
        for (int i = 0; i < fields.Length; i++)
        {
            fileNames[i] = (string)fields[i].GetRawConstantValue();
        }

        return fileNames;
    }
}
