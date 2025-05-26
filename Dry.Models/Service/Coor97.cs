using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.Service
{
    public class Coor97
    {
        double a0 = 6378137.0;
        double b0 = 6356752.314245;
        double lon0 = 121 * Math.PI / 180;
        double k0 = 0.9999;
        double dx = 250000;

        /// <summary>
        /// 給WGS84經緯度度分秒轉成TWD97坐標
        /// </summary>
        /// <param name="lonD"></param>
        /// <param name="lonM"></param>
        /// <param name="lonS"></param>
        /// <param name="latD"></param>
        /// <param name="latM"></param>
        /// <param name="latS"></param>
        /// <returns></returns>
        public string LongLat2TWD97(int lonD, int lonM, int lonS, int latD, int latM, int latS)
        {

            double RadianLon = (double)lonD + (double)lonM / 60 + (double)lonS / 3600;
            double RadianLat = (double)latD + (double)latM / 60 + (double)latS / 3600;
            return Cal_lonlat_To_twd97(RadianLon, RadianLat);

        }

        private string Cal_lonlat_To_twd97(double lon, double lat)
        {
            string TWD97 = String.Empty;
            lon = (lon / 180) * Math.PI;
            lat = (lat / 180) * Math.PI;
            double e = Math.Pow((1 - Math.Pow(b0, 2) / Math.Pow(a0, 2)), 0.5);
            double n = (a0 - b0) / (a0 + b0);
            double e2 = Math.Pow(e, 2) / (1 - Math.Pow(e, 2));
            double nu = a0 / Math.Pow((1 - (Math.Pow(e, 2)) * (Math.Pow(Math.Sin(lat), 2))), 0.5);
            double p = lon - lon0;
            double A = a0 * (1 - n + (5 / 4) * (Math.Pow(n, 2) - Math.Pow(n, 3)) + (81 / 64) * (Math.Pow(n, 4) - Math.Pow(n, 5)));
            double B = (3 * a0 * n / 2.0) * (1 - n + (7 / 8.0) * (Math.Pow(n, 2) - Math.Pow(n, 3)) + (55 / 64.0) * (Math.Pow(n, 4) - Math.Pow(n, 5)));
            double C = (15 * a0 * (Math.Pow(n, 2)) / 16.0) * (1 - n + (3 / 4.0) * (Math.Pow(n, 2) - Math.Pow(n, 3)));
            double D = (35 * a0 * (Math.Pow(n, 3)) / 48.0) * (1 - n + (11 / 16.0) * (Math.Pow(n, 2) - Math.Pow(n, 3)));
            double E1 = (315 * a0 * (Math.Pow(n, 4)) / 51.0) * (1 - n);
            double S = A * lat - B * Math.Sin(2 * lat) + C * Math.Sin(4 * lat) - D * Math.Sin(6 * lat) + E1 * Math.Sin(8 * lat);

            double K2 = k0 * nu * Math.Sin(2 * lat) / 4.0;
            double K1 = S * k0;
            double K3 = (k0 * nu * Math.Sin(lat) * (Math.Pow(Math.Cos(lat), 3)) / 24.0) * (5 - Math.Pow(Math.Tan(lat), 2) + 9 * e2 * Math.Pow((Math.Cos(lat)), 2) + 4 * (Math.Pow(e2, 2)) * (Math.Pow(Math.Cos(lat), 4)));
            double y = K1 + K2 * (Math.Pow(p, 2)) + K3 * (Math.Pow(p, 4));
            double K4 = k0 * nu * Math.Cos(lat);
            double K5 = (k0 * nu * (Math.Pow(Math.Cos(lat), 3)) / 6.0) * (1 - Math.Pow(Math.Tan(lat), 2) + e2 * (Math.Pow(Math.Cos(lat), 2)));
            double x = K4 * p + K5 * (Math.Pow(p, 3)) + dx;
            TWD97 = x.ToString() + "," + y.ToString();
            return TWD97;
        }

        /// <summary>
        /// 給WGS84經緯度弧度轉成TWD97坐標
        /// </summary>
        /// <param name="RadianLon"></param>
        /// <param name="RadianLat"></param>
        /// <returns></returns>
        public string LonLat2TWD97(double RadianLon, double RadianLat)
        {
            return Cal_lonlat_To_twd97(RadianLon, RadianLat);
        }
        /// <summary>
        /// 給TWD97坐標 轉成 WGS84 度分秒字串  (type1傳度分秒   2傳弧度)
        /// </summary>
        /// <param name="XValue"></param>
        /// <param name="YValue"></param>
        /// <param name="changetype"></param>
        /// <returns></returns>
        public string TWD972LonLat(double XValue, double YValue, int changetype)
        {
            string LonLat = string.Empty;
            if (changetype == 1)
            {
                string[] Answer = (Cal_TWD97_To_lonlat(XValue, YValue)).Split(',');
                int LonDValue = (int)Math.Truncate(double.Parse(Answer[0]));
                int LonMValue = (int)((double.Parse(Answer[0]) - LonDValue) * 60);
                int LonSValue = (int)((((double.Parse(Answer[0]) - LonDValue) * 60) - LonMValue) * 60);
                int LatDValue = (int)Math.Truncate(double.Parse(Answer[1]));
                int LatMValue = (int)((double.Parse(Answer[1]) - LatDValue) * 60);
                int LatSValue = (int)((((double.Parse(Answer[1]) - LatDValue) * 60) - LatMValue) * 60);
                LonLat = LonDValue.ToString() + "度" + LonMValue.ToString() + "分" + LonSValue.ToString() + "秒,"
                + LatDValue.ToString() + "度" + LatMValue.ToString() + "分" + LatSValue.ToString() + "秒,";
            }
            else
            {
                LonLat = Cal_TWD97_To_lonlat(XValue, YValue);
            }

            return LonLat;
        }



        private string Cal_TWD97_To_lonlat(double x, double y)
        {

            double dy = 0;
            double e = Math.Pow((1 - Math.Pow(b0, 2) / Math.Pow(a0, 2)), 0.5);
            x -= dx;
            y -= dy;
            //Calculate the Meridional Arc
            double M = y / k0;
            //Calculate Footprint Latitude
            double mu = M / (a0 * (1.0 - Math.Pow(e, 2) / 4.0 - 3 * Math.Pow(e, 4) / 64.0 - 5 * Math.Pow(e, 6) / 256.0));
            double e1 = (1.0 - Math.Pow((1.0 - Math.Pow(e, 2)), 0.5)) / (1.0 + Math.Pow((1.0 - Math.Pow(e, 2)), 0.5));

            double J1 = (3 * e1 / 2 - 27 * Math.Pow(e1, 3) / 32.0);
            double J2 = (21 * Math.Pow(e1, 2) / 16 - 55 * Math.Pow(e1, 4) / 32.0);
            double J3 = (151 * Math.Pow(e1, 3) / 96.0);
            double J4 = (1097 * Math.Pow(e1, 4) / 512.0);

            double fp = mu + J1 * Math.Sin(2 * mu) + J2 * Math.Sin(4 * mu) + J3 * Math.Sin(6 * mu) + J4 * Math.Sin(8 * mu);
            //Calculate Latitude and Longitude
            double e2 = Math.Pow((e * a0 / b0), 2);
            double C1 = Math.Pow(e2 * Math.Cos(fp), 2);
            double T1 = Math.Pow(Math.Tan(fp), 2);
            double R1 = a0 * (1 - Math.Pow(e, 2)) / Math.Pow((1 - Math.Pow(e, 2) * Math.Pow(Math.Sin(fp), 2)), (3.0 / 2.0));
            double N1 = a0 / Math.Pow((1 - Math.Pow(e, 2) * Math.Pow(Math.Sin(fp), 2)), 0.5);

            double D = x / (N1 * k0);

            //計算緯度
            double Q1 = N1 * Math.Tan(fp) / R1;
            double Q2 = (Math.Pow(D, 2) / 2.0);
            double Q3 = (5 + 3 * T1 + 10 * C1 - 4 * Math.Pow(C1, 2) - 9 * e2) * Math.Pow(D, 4) / 24.0;
            double Q4 = (61 + 90 * T1 + 298 * C1 + 45 * Math.Pow(T1, 2) - 3 * Math.Pow(C1, 2) - 252 * e2) * Math.Pow(D, 6) / 720.0;
            double lat = fp - Q1 * (Q2 - Q3 + Q4);
            //計算經度
            double Q5 = D;
            double Q6 = (1 + 2 * T1 + C1) * Math.Pow(D, 3) / 6;
            double Q7 = (5 - 2 * C1 + 28 * T1 - 3 * Math.Pow(C1, 2) + 8 * e2 + 24 * Math.Pow(T1, 2)) * Math.Pow(D, 5) / 120.0;
            double lon = lon0 + (Q5 - Q6 + Q7) / Math.Cos(fp);
            lat = (lat * 180) / Math.PI;
            lon = (lon * 180) / Math.PI;
            string lonlat = lon.ToString() + "," + lat.ToString();
            return lonlat;
        }
    }
}
