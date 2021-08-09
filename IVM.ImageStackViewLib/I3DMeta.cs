using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace ivm
{
    public class I3DMeta
    {
        ImageStackView view;

        public int pixelWidth;
        public int pixelHeight;
        public int umWidth;
        public int umHeight;
        public float pixelPerUM_X;
        public float pixelPerUM_Y;
        public float pixelPerUM_Z;

        public I3DMeta(ImageStackView v)
        {
            view = v;
        }

        void Init()
        {
            pixelWidth = 0;
            pixelHeight = 0;
            umWidth = 0;
            umHeight = 0;
            pixelPerUM_X = 1.0f;
            pixelPerUM_Y = 1.0f;
            pixelPerUM_Z = 1.0f;
        }

        bool ParseCSV(string csvPath)
        {
            using (FileStream fs = new FileStream(csvPath, FileMode.Open))
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

                    int[] vStageZs = { 0, 0 };

                    int lcnt = 0;
                    while ((l = sr.ReadLine()) != null)
                    {
                        List<string> vals = l.Split(',').ToList();
                        int vStageZ = I3DCommon.IntParse(vals[iStageZ]);
                        int vFovX = I3DCommon.IntParse(vals[iFovX]);
                        int vFovY = I3DCommon.IntParse(vals[iFovY]);
                        int vXpixel = I3DCommon.IntParse(vals[iXpixel]);
                        int vYpixel = I3DCommon.IntParse(vals[iYpixel]);

                        //Console.WriteLine("{0} {1} {2} {3} {4}", vStageZ, vFovX, vFovY, vXpixel, vYpixel);

                        pixelPerUM_X = (float)vFovX / (float)vXpixel;
                        pixelPerUM_Y = (float)vFovY / (float)vYpixel;

                        umWidth = vFovX;
                        umHeight = vFovY;

                        vStageZs[lcnt] = vStageZ;

                        lcnt++;
                        if (lcnt >= 2)
                            break;
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
                return false;

            Init();

            ParseCSV(metaPath);

            return true;
        }
    }
}
