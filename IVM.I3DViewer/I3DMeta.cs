using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace IVM.Studio.I3D
{
    public class I3DMeta
    {
        I3DViewer view;

        public float umWidth;
        public float umHeight;
        public float pixelPerUM_X;
        public float pixelPerUM_Y;
        public float pixelPerUM_Z;

        public List<DateTime> timePerFrame;

        public I3DMeta(I3DViewer v)
        {
            view = v;
        }

        void Init()
        {
            umWidth = 0;
            umHeight = 0;
            pixelPerUM_X = 1.0f;
            pixelPerUM_Y = 1.0f;
            pixelPerUM_Z = 1.0f;

            timePerFrame = new List<DateTime>();
        }

        int StrToFrame(string s)
        {
            string[] pairs = s.Split('&');
            foreach (string p in pairs)
            {
                string[] kv = p.Split('=');
                if (kv.Length <= 1)
                    continue;

                if (kv[0] == "TL")
                {
                    int frame = I3DCommon.IntParse(kv[1]);
                    return frame - 1;
                }
            }

            return -1;
        }

        DateTime StrToTime(string s)
        {
            return DateTime.Parse(s);
        }

        bool ParseCSV(string csvPath)
        {
            using (FileStream fs = new FileStream(csvPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
                {
                    string l = sr.ReadLine();
                    if (l == null)
                        return false;

                    List<string> keys = l.Split(',').ToList();
                    var iStageZ = keys.FindIndex(k => k.Contains("StageZ"));
                    var iFovX = keys.FindIndex(k => k.Contains("FovX"));
                    var iFovY = keys.FindIndex(k => k.Contains("FovY"));
                    var iXpixel = keys.FindIndex(k => k.Contains("Xpixel"));
                    var iYpixel = keys.FindIndex(k => k.Contains("Ypixel"));
                    var iSequence = keys.FindIndex(k => k.Contains("Sequence"));
                    var iTime = keys.FindIndex(k => k.Contains("Time"));

                    int[] vStageZs = { 0, 0 };

                    int lcnt = 0;
                    while ((l = sr.ReadLine()) != null)
                    {
                        List<string> vals = l.Split(',').ToList();
                        int vStageZ = I3DCommon.IntParse(vals[iStageZ]);
                        float vFovX = I3DCommon.FloatParse(vals[iFovX]);
                        float vFovY = I3DCommon.FloatParse(vals[iFovY]);
                        int vXpixel = I3DCommon.IntParse(vals[iXpixel]);
                        int vYpixel = I3DCommon.IntParse(vals[iYpixel]);
                        string vSequence = vals[iSequence];
                        string vTime = vals[iTime];

                        int frame = StrToFrame(vSequence);
                        if (timePerFrame.Count == frame)
                        {
                            DateTime t = StrToTime(vTime);
                            timePerFrame.Add(t);
                        }

                        if (lcnt <= 1)
                        {
                            //Console.WriteLine("{0} {1} {2} {3} {4}", vStageZ, vFovX, vFovY, vXpixel, vYpixel);

                            pixelPerUM_X = (float)vFovX / (float)vXpixel;
                            pixelPerUM_Y = (float)vFovY / (float)vYpixel;

                            umWidth = vFovX;
                            umHeight = vFovY;

                            vStageZs[lcnt] = vStageZ;
                        }
                        lcnt++;
                    }

                    pixelPerUM_Z = Math.Abs((float)vStageZs[1] - (float)vStageZs[0]);
                }
            }

            return true;
        }

        public bool Load(string imgPath)
        {
            string metaPath = imgPath + ".csv";

            if (!File.Exists(metaPath))
            {
                string atag = "_ALIGN";
                if (imgPath.IndexOf(atag) == (imgPath.Length - atag.Length))
                {
                    imgPath = imgPath.Substring(0, imgPath.Length - atag.Length);
                    metaPath = imgPath + ".csv";
                }

                if (!File.Exists(metaPath))
                    return false;
            }

            Init();

            ParseCSV(metaPath);

            return true;
        }
    }
}
