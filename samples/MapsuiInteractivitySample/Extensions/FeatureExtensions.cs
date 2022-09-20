using Mapsui;
using Mapsui.Nts;
using NetTopologySuite.IO;
using System;
using System.Linq;

namespace MapsuiInteractivitySample
{
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
    }
}
