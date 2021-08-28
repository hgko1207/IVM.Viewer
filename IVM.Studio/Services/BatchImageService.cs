using IVM.Studio.Models;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenCvDrawing = OpenCvSharp;

/**
 * @Class Name : BatchImageService.cs
 * @Description : 이미지 생성 서비스
 * @author 고형균
 * @since 2021.08.09
 * @version 1.0
 */
namespace IVM.Studio.Services
{
    public class BatchImageService
    {
        protected IContainerExtension ContainerExtension;

        public BatchImageService(IContainerExtension containerExtension)
        {
            ContainerExtension = containerExtension;
        }

        /// <summary>
        /// 주어진 정보를 기반으로 Z 스택 프로젝션을 수행합니다.
        /// </summary>
        /// <param name="slidesRootDir"></param>
        /// <param name="srcSlideRootDir"></param>
        /// <param name="dstSlideRootDir"></param>
        /// <param name="approvedExtensions">프로젝션 대상이 될 이미지 확장자를 담는 컬렉션입니다. ".ivm"과 같은 형식입니다.</param>
        /// <param name="startZIndex">Z스택 프로젝션을 시작할 기준 위치입니다. 1부터 시작합니다.</param>
        /// <param name="endZIndex">Z스택 프로젝션을 끝낼 기준 위치입니다. 1부터 시작합니다.</param>
        /// <returns></returns>
        public Task ZStackProjection(string slidesRootDir, DirectoryInfo srcSlideRootDir, DirectoryInfo dstSlideRootDir, IEnumerable<string> approvedExtensions, int startZIndex, int endZIndex)
        {
            return Task.Run(() =>
            {
                // 원본 슬라이드 구조 해석
                var (tlCount, mpCount, msCount, zsCount) = ContainerExtension.Resolve<FileService>().GetImagesModeStatus(srcSlideRootDir, approvedExtensions);

                if (zsCount <= 1 || startZIndex <= 0 || endZIndex <= 0 || startZIndex > zsCount || endZIndex > zsCount)
                {
                    throw new ArgumentException("A given slide path doesn't have sufficient number of slides.");
                }

                foreach (int tlIndex in EnumerateModeCount(tlCount))
                {
                    foreach (int mpIndex in EnumerateModeCount(mpCount))
                    {
                        foreach (int msIndex in EnumerateModeCount(msCount))
                        {
                            OpenCvDrawing.Mat resultImg = null;
                            OpenCvDrawing.Mat resultImgSize = null;

                            try
                            {
                                // 프로젝션
                                FileInfo first = null;
                                Metadata metadata = null;

                                for (int zsIndex = startZIndex; zsIndex <= endZIndex; zsIndex++)
                                {
                                    FileInfo file = ContainerExtension.Resolve<FileService>().FindImageInSlide(srcSlideRootDir, approvedExtensions, tlIndex, mpIndex, msIndex, zsIndex);
                                    // 첫번째 파일의 메타데이터 저장
                                    if (metadata == null)
                                    {
                                        metadata = ContainerExtension.Resolve<FileService>().ReadMetadataOfImage(slidesRootDir, file);
                                    }

                                    if (file == null) throw new FileNotFoundException("Image not found during projection.");
                                    if (first == null)
                                    {
                                        resultImgSize = new OpenCvDrawing.Mat(file.FullName, OpenCvDrawing.ImreadModes.Unchanged);
                                        resultImg = OpenCvDrawing.Mat.Zeros(resultImgSize.Size(), OpenCvDrawing.MatType.CV_8UC4);
                                        resultImgSize = null;
                                        first = file;
                                    }
                                    using (OpenCvDrawing.Mat img = new OpenCvDrawing.Mat(file.FullName, OpenCvDrawing.ImreadModes.Unchanged))
                                    {
                                        OpenCvDrawing.Cv2.Max(img, resultImg, resultImg);
                                    }
                                }

                                // 타겟 폴더에 결과 이미지 쓰기
                                DirectoryInfo di = ContainerExtension.Resolve<FileService>().GetDirectoryWithImageMode(dstSlideRootDir, tlIndex, mpIndex, msIndex, 1);
                                di.Create();

                                string fileName32 = Path.Combine(di.FullName, Path.GetFileNameWithoutExtension(first.Name) + ".ivm");
                                string fileName24 = Path.Combine(di.FullName, Path.GetFileNameWithoutExtension(first.Name) + ".png");

                                ContainerExtension.Resolve<ImageService>().SaveOpenCvImage(resultImg, fileName32, fileName24);

                                // 메타데이터 저장
                                metadata.FileName = Path.GetFileNameWithoutExtension(first.Name) + ".ivm";
                                ContainerExtension.Resolve<FileService>().SaveMetadata(slidesRootDir, fileName32, metadata);
                            }
                            finally
                            {
                                resultImg?.Dispose();
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 주어진 정보를 기반으로 모자이크를 수행합니다.
        /// </summary>
        /// <param name="slidesRootDir"></param>
        /// <param name="srcSlideRootDir"></param>
        /// <param name="dstSlideRootDir"></param>
        /// <param name="approvedExtensions">모자이크 대상이 될 이미지 확장자를 담는 컬렉션입니다. ".ivm"과 같은 형식입니다.</param>
        /// <param name="cropRate">이미지의 패딩 비율입니다. 개별 이미지에서 해당 비율만큼 가장자리를 잘라낸 후 모자이크를 수행합니다. 단위는 퍼센트입니다.</param>
        /// <returns></returns>
        public Task Mosaic(string slidesRootDir, DirectoryInfo srcSlideRootDir, DirectoryInfo dstSlideRootDir, IEnumerable<string> approvedExtensions, double cropRate)
        {
            return Task.Run(() => {
                // 원본 슬라이드 구조 해석
                var (tlCount, mpCount, msCount, zsCount) = ContainerExtension.Resolve<FileService>().GetImagesModeStatus(srcSlideRootDir, approvedExtensions);
                if (msCount <= 1)
                {
                    throw new ArgumentException("A given slide path doesn't have sufficient number of slides.");
                }

                foreach (int tlIndex in EnumerateModeCount(tlCount))
                {
                    foreach (int mpIndex in EnumerateModeCount(mpCount))
                    {
                        foreach (int zsIndex in EnumerateModeCount(zsCount))
                        {
                            // 이미지 순서 가져오기
                            Comparer<int> NearIntComparer = Comparer<int>.Create((a, b) => {
                                if (Math.Abs(a - b) <= 2) return 0;
                                else return a - b;
                            });

                            SortedDictionary<int, SortedDictionary<int, FileInfo>> files = new SortedDictionary<int, SortedDictionary<int, FileInfo>>(NearIntComparer);
                            foreach (int msIndex in EnumerateModeCount(msCount))
                            {
                                FileInfo file = ContainerExtension.Resolve<FileService>().FindImageInSlide(srcSlideRootDir, approvedExtensions, tlIndex, mpIndex, msIndex, zsIndex);
                                if (file == null) 
                                    throw new FileNotFoundException("Image not found during mosaic.");

                                Metadata meta = ContainerExtension.Resolve<FileService>().ReadMetadataOfImage(slidesRootDir, file);
                                int stageY = meta.StageY;
                                int stageX = meta.StageX;

                                if (!files.ContainsKey(stageY))
                                {
                                    files.Add(stageY, new SortedDictionary<int, FileInfo>());
                                }

                                files[stageY].Add(stageX, file);
                            }

                            OpenCvDrawing.Mat resultImg = null;

                            try
                            {
                                // 모자이크 이미지 붙이기
                                foreach (KeyValuePair<int, SortedDictionary<int, FileInfo>> horizontalMosaicImages in files)
                                {
                                    OpenCvDrawing.Mat horizontalResultImg = null;
                                    try
                                    {
                                        foreach (KeyValuePair<int, FileInfo> horizontalMosaicImage in horizontalMosaicImages.Value)
                                        {
                                            // 가로로 붙이기
                                            if (horizontalResultImg == null)
                                            {
                                                horizontalResultImg = LoadImageWithCropping(horizontalMosaicImage.Value, cropRate);
                                            }
                                            else
                                            {
                                                using (OpenCvDrawing.Mat img1 = horizontalResultImg)
                                                using (OpenCvDrawing.Mat img2 = LoadImageWithCropping(horizontalMosaicImage.Value, cropRate))
                                                {
                                                    horizontalResultImg = ContainerExtension.Resolve<ImageService>().MosaicHConcat(img1, img2);
                                                }
                                            }
                                        }

                                        // 세로로 붙이기
                                        if (resultImg == null)
                                        {
                                            resultImg = horizontalResultImg;
                                            horizontalResultImg = null;
                                        }
                                        else
                                        {
                                            using (OpenCvDrawing.Mat img1 = resultImg)
                                            {
                                                resultImg = ContainerExtension.Resolve<ImageService>().MosaicVConcat(img1, horizontalResultImg);
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        horizontalResultImg?.Dispose();
                                    }
                                }

                                // 모자이크 결과 이미지 쓰기
                                DirectoryInfo directoryInfo = ContainerExtension.Resolve<FileService>().GetDirectoryWithImageMode(dstSlideRootDir, tlIndex, mpIndex, 1, zsIndex);
                                directoryInfo.Create();

                                string fileName32, fileName24;
                                if (zsIndex == 0)
                                {
                                    fileName32 = Path.Combine(directoryInfo.FullName, "Mosaic.ivm");
                                    fileName24 = Path.Combine(directoryInfo.FullName, "Mosaic.png");
                                }
                                else
                                {
                                    fileName32 = Path.Combine(directoryInfo.FullName, $"ZS{zsIndex:D4}.ivm");
                                    fileName24 = Path.Combine(directoryInfo.FullName, $"ZS{zsIndex:D4}.png");
                                }
                                ContainerExtension.Resolve<ImageService>().SaveOpenCvImage(resultImg, fileName32, fileName24);
                            }
                            finally
                            {
                                resultImg?.Dispose();
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// LoadImageWithCropping
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        private OpenCvDrawing.Mat LoadImageWithCropping(FileInfo fileInfo, double padding)
        {
            OpenCvDrawing.Mat result = new OpenCvDrawing.Mat(fileInfo.FullName);
            if (padding > 0)
            {
                int HorizontalPadding = (int)Math.Round(result.Width * padding / 100);
                int VerticalPadding = (int)Math.Round(result.Height * padding / 100);
                OpenCvDrawing.Mat result2 = result[VerticalPadding, result.Height - VerticalPadding, HorizontalPadding, result.Width - VerticalPadding];
                result.Dispose();
                result = result2;
            }
            return result;
        }

        /// <summary>
        /// Enumerate ModeCount
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private IEnumerable<int> EnumerateModeCount(int count)
        {
            if (count >= 1)
            {
                for (int i = 1; i <= count; i++) yield return i;
            }
            else
            {
                yield return 0;
            }
        }
    }
}
