using IVM.Studio.Models;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GDIDrawing = System.Drawing;
using OpenCvDrawing = OpenCvSharp;
using WPFDrawing = System.Windows.Media;

namespace IVM.Studio.Services
{
    public class ImageService
    {
        /// <summary>
        /// 이미지 로드
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Bitmap LoadImage(string path)
        {
            using (Bitmap bitmap = new Bitmap(path))
            {
                return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), GDIDrawing.Imaging.PixelFormat.Format32bppArgb);
            }
        }

        /// <summary>
        /// 각 채널별 색 투영 정보를 사용하여 <seealso cref="TranslateColor(GDIDrawing.Bitmap, float[][])"/> 함수에서 사용할 컬러 매트릭스를 생성합니다.
        /// </summary>
        /// <param name="startLevelByChannel">각 채널의 이미지 레벨 조정시 사용할 시작 레벨입니다. 픽셀의 컬러 레벨이 해당 값 이하인 경우 최소값으로 투영됩니다.</param>
        /// <param name="endLevelByChannel">각 채널의 이미지 레벨 조정시 사용할 끝 레벨입니다. 픽셀의 컬러 레벨이 해당 값 이하인 경우 최대값으로 투영됩니다.</param>
        /// <param name="brightnessByChannel">각 채널의 밝기입니다. 기본값은 0, 최소값은 -1, 최대값은 1입니다.</param>
        /// <param name="contrastByChannel">각 채널의 대비입니다. 기본값은 1, 최소값은 0입니다.</param>
        /// <param name="translationByChannel">
        /// 이미지의 채널이 어떤 색상으로 투영될지를 의미합니다. 크기가 4인 <seealso cref="int[]"/>이여야 합니다.
        /// 0은 빨강, 1은 녹색, 2는 파랑, 3은 알파, -1은 비표시입니다.
        /// </param>
        /// <param name="visibilityByChannel">
        /// 이미지의 채널이 투영될지 아닐지를 정합니다. 크기가 4인 <seealso cref="bool[]"/>이여야 합니다.
        /// 값이 false일 경우 <paramref name="TranslationByChannel"/>의 값을 무시하고, 투영되지 않습니다.
        /// </param>
        public float[][] GenerateColorMatrix(int[] startLevelByChannel, int[] endLevelByChannel, float[] brightnessByChannel, float[] contrastByChannel, 
            int[] translationByChannel, bool[] visibilityByChannel)
        {
            float[] contrast_adjust = contrastByChannel.Select(s => (1 - s) / 2).ToArray();
            float[][] colorMatrix = new float[][] {
                new float[] { 0, 0, 0, 0, 0 },
                new float[] { 0, 0, 0, 0, 0 },
                new float[] { 0, 0, 0, 0, 0 },
                new float[] { 0, 0, 0, 0, 0 },
                new float[] { 0, 0, 0, 0, 0 }
            };

            HashSet<int> visibleColors = new HashSet<int>();
            for (int i = 0; i < 4; i++)
            {
                if (translationByChannel[i] == -1 || !visibilityByChannel[i] || visibleColors.Contains(translationByChannel[i]))
                {
                    continue;
                }
                colorMatrix[i][translationByChannel[i]] = contrastByChannel[i] / (endLevelByChannel[i] - startLevelByChannel[i]) * 255;
                colorMatrix[4][translationByChannel[i]] = brightnessByChannel[i] + contrast_adjust[i] - (float)startLevelByChannel[i] / (endLevelByChannel[i] - startLevelByChannel[i]);
                visibleColors.Add(translationByChannel[i]);
            }

            // 알파 레이어가 없는 경우 모든 알파값은 1이 됨
            if (!visibleColors.Contains(3))
            {
                colorMatrix[3][3] = 0;
                colorMatrix[4][3] = 1;
            }

            return colorMatrix;
        }

        /// <summary>
        /// 주어진 컬러 매트릭스에 따라 이미지의 색상을 투영합니다.
        /// </summary>
        public Bitmap TranslateColor(Bitmap Image, float[][] ColorMatrix)
        {
            GDIDrawing.Imaging.ColorMatrix cm = new GDIDrawing.Imaging.ColorMatrix(ColorMatrix);
            GDIDrawing.Imaging.ImageAttributes attr = new GDIDrawing.Imaging.ImageAttributes();
            attr.SetColorMatrix(cm);

            Point[] destPoints = new Point[] {
                new Point(0, 0),
                new Point(Image.Width, 0),
                new Point(0, Image.Height)
            };
            Rectangle srcRect = new Rectangle(0, 0, Image.Width, Image.Height);
            Bitmap bitmap = new Bitmap(Image.Width, Image.Height);

            using (Graphics graphics= Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(Image, destPoints, srcRect, GraphicsUnit.Pixel, attr);
                return bitmap;
            }
        }

        /// <summary>
        /// 주어진 컬러맵에 따라 이미지의 모든 채널의 영상을 각각 투영한 후 맥스 풀링을 하여 출력합니다.
        /// </summary>
        public Bitmap ApplyColorMaps(Bitmap Image, IList<ColorMap?> ColorMaps)
        {
            if (ColorMaps.All(s => s == null)) 
                return (Bitmap)Image.Clone();

            // 각 채널에 컬러맵 적용하여 리스트에 넣기
            using (OpenCvDrawing.Mat src = Image.ToMat())
            {
                IEnumerable<OpenCvDrawing.Mat> results = ColorMaps.Select((s, idx) => {
                    if (s == null) 
                        return src.ExtractChannel(idx != 3 ? 2 - idx : idx);
                    else 
                        return ApplyColorMapMat(src, idx, s.GetValueOrDefault());
                });

                using (OpenCvDrawing.Mat dst = OpenCvDrawing.Mat.Zeros(src.Size(), OpenCvDrawing.MatType.CV_8UC4))
                {
                    foreach ((OpenCvDrawing.Mat resultCh, int idx) in results.Select((s, idx) => (s, idx)))
                    {
                        if (resultCh.Channels() == 1)
                        {
                            // 1ch to 4ch
                            int Channel = idx != 3 ? 2 - idx : idx;
                            using (OpenCvDrawing.Mat result_ch4 = OpenCvDrawing.Mat.Zeros(src.Size(), OpenCvDrawing.MatType.CV_8UC4))
                            {
                                OpenCvDrawing.Cv2.InsertChannel(resultCh, result_ch4, Channel);
                                OpenCvDrawing.Cv2.Max(dst, result_ch4, dst);
                            }
                        }
                        else
                        {
                            // 3ch to 4ch
                            OpenCvDrawing.Mat[] chs = resultCh.Split();
                            using (OpenCvDrawing.Mat result_ch4 = new OpenCvDrawing.Mat())
                            {
                                OpenCvDrawing.Cv2.Merge(new[] { chs[0], chs[1], chs[2], (OpenCvDrawing.Mat)OpenCvDrawing.Mat.Zeros(src.Size(), OpenCvDrawing.MatType.CV_8UC1) }, result_ch4);
                                OpenCvDrawing.Cv2.Max(dst, result_ch4, dst);
                            }

                            foreach (OpenCvDrawing.Mat i in chs) 
                                i.Dispose();
                        }

                        resultCh.Dispose();
                    }

                    return dst.ToBitmap();
                }
            }
        }

        public OpenCvDrawing.Mat ApplyColorMapMat(OpenCvDrawing.Mat Image, int Channel, ColorMap ColorMap)
        {
            // RGBA(IVM Viewer standard) to BGRA(OpenCV)
            if (Channel != 3) 
                Channel = 2 - Channel;

            using (OpenCvDrawing.Mat src = Image.ExtractChannel(Channel))
            {
                OpenCvDrawing.Mat dst = new OpenCvDrawing.Mat(Image.Size(), OpenCvDrawing.MatType.CV_8UC3);
                OpenCvDrawing.Cv2.ApplyColorMap(src, dst, (OpenCvDrawing.ColormapTypes)ColorMap);
                return dst;
            }
        }

        /// <summary>
        /// 주어진 인수에 따라 이미지를 회전 또는 반전합니다.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="horizontalReflect"></param>
        /// <param name="verticalReflect"></param>
        /// <param name="rotate">0은 원상태 유지, 1은 시계방향으로 90도 회전, 2는 시계방향으로 180도 회전, 3은 시계방향으로 270도 회전입니다.</param>
        public void ReflectAndRotate(Bitmap image, bool horizontalReflect, bool verticalReflect, int rotate)
        {
            int targetForm = rotate;
            // Flip with X
            if (horizontalReflect) 
                targetForm ^= 0b100;
            // Flip with Y
            if (verticalReflect) 
                targetForm ^= 0b110;

            image.RotateFlip((RotateFlipType)targetForm);
        }

        /// <summary>주어진 비트맵 이미지 위에 다른 이미지를 그립니다.</summary>
        public void DrawImageOnImage(Bitmap originalImage, Bitmap overlayImage)
        {
            if (originalImage == null || overlayImage == null) 
                return;

            using (Graphics graphics = Graphics.FromImage(originalImage))
            {
                graphics.DrawImage(overlayImage, new Point(0, 0));
            }
        }

        /// <summary>
        /// 주어진 비트맵 이미지에 스케일 바를 그립니다.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fOVSizeX">이미지의 X축의 길이입니다. 단위는 <paramref name="fOVSizeX"/>, <paramref name="lengthOfScaleBar"/>와 같아야 합니다.</param>
        /// <param name="fOVSizeY">이미지의 Y축의 길이입니다. 단위는 <paramref name="fOVSizeY"/>, <paramref name="lengthOfScaleBar"/>와 같아야 합니다.</param>
        /// <param name="lengthOfScaleBar">이미지에 표시될 스케일 바의 길이입니다. 단위는 <paramref name="fOVSizeX"/>, <paramref name="fOVSizeY"/>와 같아야 합니다.</param>
        /// <param name="thicknessOfScaleBar">이미지에 표시될 스케일 바의 굵기입니다. 단위는 픽셀입니다.</param>
        /// <param name="clampSizeOfScaleBar">이미지에 표시될 스케일 바의 양 끝부분에 표시될 눈금의 길이입니다. 단위는 픽셀입니다.</param>
        /// <param name="marginOfScaleBar">이미지에 표시될 스케일 바와 이미지 경계 사이의 여백 크기입니다. 단위는 픽셀입니다.</param>
        public void DrawScaleBar(Bitmap image, int fOVSizeX, int fOVSizeY, int lengthOfScaleBar, int thicknessOfScaleBar, int clampSizeOfScaleBar, int marginOfScaleBar)
        {
            using (Graphics gr = Graphics.FromImage(image))
            using (Pen pen = new Pen(Brushes.White, thicknessOfScaleBar))
            {
                int canvasWidth = image.Width;
                int canvasHeight = image.Height;
                Size scaleBarX = new Size((int)((double)canvasWidth / fOVSizeX * lengthOfScaleBar), 0);
                Size scaleBarY = new Size(0, (int)((double)canvasHeight / fOVSizeY * lengthOfScaleBar));
                Point StartPoint = new Point(canvasWidth - marginOfScaleBar, canvasHeight - marginOfScaleBar);
                Size clampX = new Size(clampSizeOfScaleBar, 0);
                Size clampY = new Size(0, clampSizeOfScaleBar);

                gr.DrawLine(pen, StartPoint, StartPoint - scaleBarX);
                gr.DrawLine(pen, StartPoint, StartPoint - scaleBarY);
                gr.DrawLine(pen, StartPoint - scaleBarX + clampY, StartPoint - scaleBarX - clampY);
                gr.DrawLine(pen, StartPoint - scaleBarY + clampX, StartPoint - scaleBarY - clampX);
                gr.FillRectangle(Brushes.White, new Rectangle(StartPoint - new Size(thicknessOfScaleBar / 2, thicknessOfScaleBar / 2),
                    new Size(thicknessOfScaleBar, thicknessOfScaleBar)));
            }
        }

        /// <summary>
        /// 주어진 GDI+ 비트맵을 WPF 이미징 프레임워크에서 사용 가능한 <seealso cref="WPFDrawing.Imaging.BitmapSource"/>로 변환합니다.
        /// </summary>
        public WPFDrawing.Imaging.BitmapSource ConvertGDIBitmapToWPF(Bitmap Image)
        {
            if (Image == null) 
                return null;

            Rectangle rect = new Rectangle(0, 0, Image.Width, Image.Height);
            GDIDrawing.Imaging.BitmapData bitmapData = Image.LockBits(rect, GDIDrawing.Imaging.ImageLockMode.ReadWrite, GDIDrawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                int size = rect.Width * rect.Height * 4;
                WPFDrawing.Imaging.BitmapSource result = WPFDrawing.Imaging.BitmapSource.Create(Image.Width, Image.Height, 
                    Image.HorizontalResolution, Image.VerticalResolution, WPFDrawing.PixelFormats.Bgra32,
                    null, bitmapData.Scan0, size, bitmapData.Stride);
                result.Freeze();
                return result;
            }
            finally
            {
                Image.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        /// 주어진 이미지에서 히스토그램 이미지를 생성합니다.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="translationByChannel">
        /// 이 이미지를 생성할 때 이미지의 각 채널이 어떤 색상으로 투영되었는지를 뜻합니다. 크기가 4인 <seealso cref="int[]"/>이여야 합니다.
        /// 0은 빨강, 1은 녹색, 2는 파랑, 3은 알파, -1은 비표시입니다. 이 값이 -1인 채널은 히스토그램 계산에 사용되지 않습니다.
        /// </param>
        /// <param name="VisibilityByChannel">
        /// 이미지의 채널이 투영되었는지 아닌지에 관한 정보입니다. 크기가 4인 <seealso cref="bool[]"/>이여야 합니다.
        /// 이 값이 false인 채널은 히스토그램 계산에 사용되지 않습니다.
        /// </param>
        public Bitmap CreateHistogram(Bitmap image, int[] translationByChannel, bool[] visibilityByChannel)
        {
            int[,] colorValue = new int[4, 256];
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color c = image.GetPixel(x, y);
                    for (int i = 0; i < 4; i++)
                    {
                        if (!visibilityByChannel[i]) 
                            continue;

                        switch (translationByChannel[i])
                        {
                            case 0:
                                colorValue[0, c.R]++;
                                break;
                            case 1:
                                colorValue[1, c.G]++;
                                break;
                            case 2:
                                colorValue[2, c.B]++;
                                break;
                            case 3:
                                colorValue[3, c.A]++;
                                break;
                        }
                    }
                }
            }

            // 히스토그램 보간 - 히스토그램이 부드럽게 변화하도록 함
            int[,] adjustedColorValue = new int[colorValue.GetLength(0), colorValue.GetLength(1) - 1];
            for (int i = 0; i < colorValue.GetLength(0); i++)
            {
                adjustedColorValue[i, 0] = (int)Math.Round((colorValue[i, 0] + colorValue[i, 1]) / 2d);
                for (int j = 1; j < colorValue.GetLength(1) - 1; j++)
                {
                    adjustedColorValue[i, j] = (int)Math.Round((colorValue[i, j + 1] + adjustedColorValue[i, j - 1]) / 2d);
                }
            }

            // 평균값 및 표준편차 계산
            // 평균 + 표준편차 * 2가 히스토그램 이미지의 세로축 최대 표시 범위가 됩니다.
            IEnumerable<int> flattenColor = adjustedColorValue.Cast<int>();
            double avg = flattenColor.Average();
            double variance = flattenColor.Select(s => Math.Pow(s - avg, 2)).Sum();
            double stddev = Math.Sqrt(variance / colorValue.Length);
            Bitmap hist = new Bitmap(adjustedColorValue.GetLength(1), 100);

            int max = (int)(avg + stddev * 2);
            if (max == 0)
            {
                using (Graphics g = Graphics.FromImage(hist))
                {
                    g.FillRectangle(Brushes.White, new Rectangle(0, 0, hist.Width, hist.Height));
                }
            }
            else
            {
                // 표시 최대값에 맞춰서 보정
                for (int i = 0; i < adjustedColorValue.GetLength(0); i++)
                {
                    for (int j = 0; j < adjustedColorValue.GetLength(1); j++)
                    {
                        adjustedColorValue[i, j] = Math.Min(100, (int)Math.Round(adjustedColorValue[i, j] * 100d / max));
                    }
                }

                using (Graphics g = Graphics.FromImage(hist))
                {
                    for (int i = 0; i < adjustedColorValue.GetLength(0); i++)
                    {
                        for (int j = 0; j < adjustedColorValue.GetLength(1); j++)
                        {
                            Color color;
                            switch (i)
                            {
                                case 0:
                                    color = Color.FromArgb(127, 255, 0, 0);
                                    break;
                                case 1:
                                    color = Color.FromArgb(127, 0, 255, 0);
                                    break;
                                case 2:
                                    color = Color.FromArgb(127, 0, 0, 255);
                                    break;
                                default:
                                    color = Color.FromArgb(127, 0, 0, 0);
                                    break;
                            }

                            using (Brush brush = new SolidBrush(color))
                            {
                                g.FillRectangle(brush, j, 100 - adjustedColorValue[i, j], 1, adjustedColorValue[i, j]);
                            }
                        }
                    }
                }
            }

            return hist;
        }
    }
}
