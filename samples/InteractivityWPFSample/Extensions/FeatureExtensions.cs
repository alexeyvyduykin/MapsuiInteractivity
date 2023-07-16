using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Linq;

namespace InteractivityWPFSample.Extensions;

public static class FeatureExtensions
{
    public static T? GetValue<T>(this IFeature feature, string property)
    {
        if (feature.Fields.Contains(property) == true)
        {
            return (T)feature[property]!;
        }

        return default;
    }

    public static string ToFeatureInfo(this IFeature feature)
    {
        string res = string.Empty;

        foreach (string field in feature.Fields)
        {
            res += $"{field}:{feature[field]}";
            res += Environment.NewLine;
        }

        if (feature.Fields.Any())
        {
            res = res.Remove(res.Length - 1);
        }

        return res;
    }

    public static string ToWkt(this IFeature feature)
    {
        if (feature is GeometryFeature gf)
        {
            WKTWriter writer = new WKTWriter();

            return writer.Write(gf.Geometry);
        }

        return string.Empty;
    }

    public static GeometryFeature ToFeature(this Geometry geometry, string name)
    {
        var feature = geometry.ToFeature();

        feature["Name"] = name;

        return feature;
    }
}
