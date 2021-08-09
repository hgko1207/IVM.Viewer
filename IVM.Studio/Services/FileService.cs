using IVM.Studio.Models;
using IVM.Studio.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

/**
 * @Class Name : FileService.cs
 * @Description : 파일 관리 서비스
 * @author 고형균
 * @since 2021.03.29
 * @version 1.0
 */
namespace IVM.Studio.Services
{
    public class FileService
    {
        /// <summary>
        /// 주어진 이미지/비디오 파일에 해당되는 메타데이터를 찾습니다.
        /// </summary>
        /// <param name="SlidesRootPath"></param>
        /// <param name="file"></param>
        /// <returns>적절한 메타데이터가 존재하지 않는 경우 null을 반환합니다.</returns>
        public Metadata ReadMetadataOfImage(string slidesRootPath, FileInfo file)
        {
            string filePath = file.DirectoryName;
            if (slidesRootPath == filePath)
            {
                // 단일 캡쳐인 경우: 생성날짜와 하루 전/후의 csv를 검색
                DateTime metadataDate = file.CreationTime;
                for (int i = -1; i <= 1; i++)
                {
                    string csvPath = Path.Combine(slidesRootPath, $"{metadataDate + TimeSpan.FromDays(i):yyyy-MM-dd}.csv");
                    if (!File.Exists(csvPath))
                        continue;

                    foreach (Metadata j in ReadAllMetadataFromCSV(csvPath))
                        if (Path.GetFileNameWithoutExtension(j.FileName) == Path.GetFileNameWithoutExtension(file.FullName))
                            return j;
                }

                return null;
            }
            else
            {
                // 이미지 모드 캡쳐인 경우: 캡쳐 폴더 이름과 같은 이름의 csv를 검색
                string csvPath = Path.Combine(slidesRootPath, filePath.Substring(slidesRootPath.Length).Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[0] + ".csv");
                if (!File.Exists(csvPath))
                    return null;

                foreach (Metadata j in ReadAllMetadataFromCSV(csvPath))
                    if (Path.GetFileNameWithoutExtension(j.FileName) == Path.GetFileNameWithoutExtension(file.FullName))
                        return j;

                return null;
            }
        }

        /// <summary>
        /// 주어진 이미지/비디오 파일에 해당되는 메타데이터를 찾습니다.
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="file"></param>
        /// <returns>적절한 메타데이터가 존재하지 않는 경우 null을 반환합니다.</returns>
        public Metadata ReadMetadataFromImage(string rootPath, FileInfo file)
        {
            string filePath = file.DirectoryName;
            if (rootPath == filePath)
            {
                // 단일 캡쳐인 경우: 생성날짜와 하루 전/후의 csv를 검색
                DateTime metadataDate = file.CreationTime;
                for (int i = -1; i <= 1; i++)
                {
                    string csvPath = Path.Combine(rootPath, $"{metadataDate + TimeSpan.FromDays(i):yyyy-MM-dd}.csv");
                    if (!File.Exists(csvPath))
                        continue;

                    foreach (Metadata j in ReadAllMetadataFromCSV(csvPath))
                    {
                        if (Path.GetFileNameWithoutExtension(j.FileName) == Path.GetFileNameWithoutExtension(file.FullName))
                            return j;
                    }
                }

                return null;
            }
            else
            {
                // 이미지 모드 캡쳐인 경우: 캡쳐 폴더 이름과 같은 이름의 csv를 검색
                string csvPath = Path.Combine(rootPath, filePath.Substring(rootPath.Length).Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[0] + ".csv");
                if (!File.Exists(csvPath))
                    return null;

                foreach (Metadata j in ReadAllMetadataFromCSV(csvPath))
                {
                    if (Path.GetFileNameWithoutExtension(j.FileName) == Path.GetFileNameWithoutExtension(file.FullName))
                        return j;
                }

                return null;
            }
        }

        /// <summary>
        /// 주어진 슬라이드에 포함된 모든 이미지/비디오 파일의 메타데이터를 읽어옵니다.
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        public IEnumerable<Metadata> ReadAllMetadataFromCSV(string csvFile)
        {
            if (!File.Exists(csvFile)) yield break;
            foreach (string i in File.ReadLines(csvFile).Skip(1))
            {
                Metadata rec = Deserialize<Metadata>(i);
                yield return rec;
            }
        }

        /// <summary>
        /// 주어진 슬라이드 루트 폴더 안에서 주어진 모드 순서에 해당하는 이미지를 찾아 반환합니다. 값이 0인 경우 해당 모드를 사용하지 않습니다.
        /// </summary>
        /// <param name="slideRootDir"></param>
        /// <param name="approvedExtensions"></param>
        /// <param name="timeLapse"></param>
        /// <param name="multiPosition"></param>
        /// <param name="mosaic"></param>
        /// <param name="zStack"></param>
        /// <returns>null인 경우 검색에 실패한 경우입니다.</returns>
        public FileInfo FindImageInSlide(DirectoryInfo slideRootDir, IEnumerable<string> approvedExtensions, int timeLapse, int multiPosition, int mosaic, int zStack)
        {
            List<string> paths = new List<string>();
            List<int> indices = new List<int>();
            if (timeLapse > 0)
            {
                paths.Add($"TL");
                indices.Add(timeLapse);
            }
            if (multiPosition > 0)
            {
                paths.Add($"MP");
                indices.Add(multiPosition);
            }
            if (mosaic > 0)
            {
                paths.Add($"MS");
                indices.Add(mosaic);
            }
            if (zStack > 0)
            {
                paths.Add($"ZS");
                indices.Add(zStack);
            }

            if (paths.Count == 0)
                return null;

            // 폴더 탐색 (첫 두글자가 주어진 모드와 맞는 폴더 중에 첫 두글자를 제외한 숫자가 모드 번호와 맞는 경우)
            // ZS0001, ZS001, ZS01, MS03 등등....
            DirectoryInfo cursor = slideRootDir;
            for (int i = 0; i < paths.Count - 1; i++)
            {
                DirectoryInfo dir = cursor.EnumerateDirectories(paths[i] + "*").FirstOrDefault(s => {
                    try
                    {
                        string numberString = s.Name.Substring(2);
                        int numberInt = Convert.ToInt32(numberString);
                        return numberInt == indices[i];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return false;
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
                });

                if (dir != null)
                    cursor = dir;
                else
                    return null;
            }

            // 파일 탐색
            try
            {
                return GetImagesInFolder(cursor, approvedExtensions, false).ElementAt(indices[indices.Count - 1] - 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>
        /// 주어진 폴더 안의 이미지를 열거합니다.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="extensions"></param>
        /// <param name="recursively">true 이면 재귀적으로 모든 하위 폴더를 포함해서 검사합니다.</param>
        /// <returns></returns>
        public IEnumerable<FileInfo> GetImagesInFolder(DirectoryInfo directory, IEnumerable<string> extensions, bool recursively)
        {
            IEnumerable<FileInfo> files = directory.EnumerateFiles("*", recursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            // 확장자가 유효하고 Registration 파일이 아닌 모든 경우
            return files.Where(s => {
                if (extensions.Contains(s.Extension, StringComparer.OrdinalIgnoreCase))
                    return !Regex.IsMatch(s.Name, "_(TL|ZS)?Reg", RegexOptions.IgnoreCase);
                else
                    return false;
            });
        }

        /// <summary>
        /// 주어진 객체를 CSV로 직렬화합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rec"></param>
        /// <returns></returns>
        private string Serialize<T>(T rec)
        {
            List<string> serialized = new List<string>();
            foreach (PropertyInfo prop in GetColumns<T>())
            {
                object value = prop.GetValue(rec);
                if (value == null) continue;
                serialized.Add(value.ToString());
            }
            return string.Join(",", serialized);
        }

        /// <summary>
        /// 타입 <typeparamref name="T"/>의 프로퍼티 이름을 CSV로 직렬화합니다. CSV 제목 행에 사용될 수 있습니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string SerializeColumns<T>()
        {
            return string.Join(",", GetColumns<T>().Select(s => s.Name));
        }

        /// <summary>
        /// 주어진 문자열을 타입 <typeparamref name="T"/>의 객체로 역직렬화합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        private T Deserialize<T>(string str) where T : new()
        {
            string[] split = str.Split(',');
            PropertyInfo[] columns = GetColumns<T>();
            T rec = new T();

            for (int i = 0; i < Math.Min(columns.Length, split.Length); i++)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(columns[i].PropertyType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        columns[i].SetValue(rec, converter.ConvertFrom(split[i]));
                    }
                    catch (Exception)
                    {
                        // 변환 실패시 기본값 사용
                        if (columns[i].PropertyType.IsValueType) columns[i].SetValue(rec, Activator.CreateInstance(columns[i].PropertyType));
                        else columns[i].SetValue(rec, null);
                    }
                }
            }

            return rec;
        }

        /// <summary>
        /// 타입 <typeparamref name="T"/>에서 CSV 직렬화 가능한 모든 프로퍼티의 배열을 가져옵니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private PropertyInfo[] GetColumns<T>()
        {
            SortedDictionary<int, PropertyInfo> columns = new SortedDictionary<int, PropertyInfo>();

            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                CSVSerializableAttribute attr = prop.GetCustomAttribute<CSVSerializableAttribute>();
                if (attr is null)
                    continue;

                columns.Add(attr.Column, prop);
            }

            return columns.Values.ToArray();
        }

        public ChannelNameConverter GenerateChannelNameConverter(Metadata metadata)
        {
            ChannelNameConverter result = new ChannelNameConverter();
            for (int i = 0; i < 4; i++)
            {
                string ChannelColor, ChannelName;
                switch (i)
                {
                    case 0: ChannelColor = metadata.ChA; ChannelName = "A"; break;
                    case 1: ChannelColor = metadata.ChB; ChannelName = "B"; break;
                    case 2: ChannelColor = metadata.ChC; ChannelName = "C"; break;
                    case 3: ChannelColor = metadata.ChD; ChannelName = "D"; break;
                    default: continue;
                }

                switch (ChannelColor)
                {
                    case "R": result.AddMatch(0, ChannelName); break;
                    case "G": result.AddMatch(1, ChannelName); break;
                    case "B": result.AddMatch(2, ChannelName); break;
                    case "A": result.AddMatch(3, ChannelName); break;
                    default: continue;
                }
            }

            result.Freeze();
            return result;
        }

        /// <summary>
        /// 주어진 슬라이드 폴더 안의 이미지 모드 갯수를 계산하여 반환합니다. 각 모드의 첫번째 폴더만을 검사합니다.
        /// </summary>
        /// <param name="slideRootDir"></param>
        /// <param name="approvedExtensions"></param>
        /// <returns>각 모드별 슬라이드 폴더 갯수를 반환합니다. 0이면 해당 계층이 존재하지 않음을 뜻합니다.</returns>
        public (int TimeLapse, int MultiPosition, int Mosaic, int ZStack) GetImagesModeStatus(DirectoryInfo slideRootDir, IEnumerable<string> approvedExtensions)
        {
            if (!slideRootDir.Exists) 
                return (0, 0, 0, 0);

            string[] imageModes = Regex.Replace(slideRootDir.Name, @"([A-Za-z\-]+)([0-9]+)(_.*)?", "$1").Split('-');
            int[] counts = new int[imageModes.Length];

            DirectoryInfo cursor = slideRootDir;
            for (int i = 0; i < imageModes.Length; i++)
            {
                if (i == imageModes.Length - 1)
                {
                    // 마지막 모드는 파일 갯수를 세야함
                    int count = GetImagesInFolder(cursor, approvedExtensions, false).Count();
                    counts[i] = Math.Max(counts[i], count);
                }
                else
                {
                    List<DirectoryInfo> subdirs = cursor.EnumerateDirectories($"{imageModes[i]}*").ToList();
                    if (subdirs.Count == 0)
                        break;

                    counts[i] = Math.Max(counts[i], subdirs.Count);
                    cursor = subdirs[0];
                }
            }

            int zsIndex = Array.FindIndex(imageModes, s => s == "ZS");
            int mpIndex = Array.FindIndex(imageModes, s => s == "MP");
            int msIndex = Array.FindIndex(imageModes, s => s == "MS");
            int tlIndex = Array.FindIndex(imageModes, s => s == "TL");

            return (tlIndex == -1 ? 0 : counts[tlIndex], mpIndex == -1 ? 0 : counts[mpIndex], msIndex == -1 ? 0 : counts[msIndex], zsIndex == -1 ? 0 : counts[zsIndex]);
        }

        /// <summary>
        /// 주어진 이미지 모드에 따라 해당 슬라이드 루트 아래에 파일이 위치할 최종 폴더를 생성합니다.
        /// </summary>
        /// <param name="slideRootDir"></param>
        /// <param name="timeLapse"></param>
        /// <param name="multiPosition"></param>
        /// <param name="mosaic"></param>
        /// <param name="zStack"></param>
        /// <returns></returns>
        public DirectoryInfo GetDirectoryWithImageMode(DirectoryInfo slideRootDir, int timeLapse, int multiPosition, int mosaic, int zStack)
        {
            List<string> Paths = new List<string> { slideRootDir.FullName };

            if (timeLapse > 0) Paths.Add($"TL{timeLapse:D4}");
            if (multiPosition > 0) Paths.Add($"MP{multiPosition:D4}");
            if (mosaic > 0) Paths.Add($"MS{mosaic:D4}");
            if (zStack > 0) Paths.Add($"ZS{zStack:D4}");

            if (Paths.Count <= 2) 
                return slideRootDir;
            else 
                Paths.RemoveAt(Paths.Count - 1);

            return new DirectoryInfo(Path.Combine(Paths.ToArray()));
        }

        /// <summary>
        /// 메타데이터 저장
        /// </summary>
        /// <param name="slidesRootPath"></param>
        /// <param name="filePath"></param>
        /// <param name="metadata"></param>
        public void SaveMetadata(string slidesRootPath, string filePath, Metadata metadata) => SaveMetadata(slidesRootPath, new FileInfo(filePath), metadata);
        public void SaveMetadata(string slidesRootPath, FileInfo file, Metadata metadata)
        {
            string filePath = file.DirectoryName;
            string csvPath;
            if (slidesRootPath == filePath)
            {
                // 단일 캡쳐인 경우: 메타데이터의 생성 날짜와 동일한 날짜로 저장.
                csvPath = Path.Combine(slidesRootPath, $"{metadata.Time:yyyy-MM-dd}.csv");
            }
            else
            {
                // 이미지 모드 캡쳐인 경우: 캡쳐 폴더 이름과 같은 이름의 csv를 사용
                csvPath = Path.Combine(slidesRootPath, filePath.Substring(slidesRootPath.Length).Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)[0] + ".csv");
            }
            if (!File.Exists(csvPath))
            {
                using (StreamWriter csv = File.CreateText(csvPath))
                {
                    csv.WriteLine(SerializeColumns<Metadata>());
                    csv.WriteLine(Serialize(metadata));
                }
            }
            else
            {
                using (StreamWriter csv = File.AppendText(csvPath))
                {
                    csv.WriteLine(Serialize(metadata));
                }
            }
        }

        /// <summary>
        /// 주어진 객체에 포함된 CSV 직렬화 가능 프로퍼티를 이용해 바인딩용 메타데이터 모델 컬렉션을 만듭니다.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public ObservableCollection<MetadataModel> ToModel(Metadata metadata)
        {
            ObservableCollection<MetadataModel> metadataCollection = new ObservableCollection<MetadataModel>();

            ImageSequence sequence = metadata.Sequence;
            metadataCollection.Add(new MetadataModel()
            {
                Group = "Sequence",
                Name1 = "TL",
                Value1 = sequence.TimeLapseNumbering.ToString(),
                Name2 = "MP",
                Value2 = sequence.MultiPositionNumbering.ToString(),
                Name3 = "MS",
                Value3 = sequence.MosaicNumbering.ToString(),
                Name4 = "ZS",
                Value4 = sequence.ZStackNumbering.ToString()
            });
            metadataCollection.Add(new MetadataModel()
            {
                Group = "Pseudocolor",
                Name1 = "DAPI",
                Value1 = metadata.ChA,
                Name2 = "GFP",
                Value2 = metadata.ChB,
                Name3 = "RFP",
                Value3 = metadata.ChC,
                Name4 = "NIR",
                Value4 = metadata.ChD,
            });
            metadataCollection.Add(new MetadataModel()
            {
                Group = "Laser (%)",
                Name1 = "DAPI",
                Value1 = metadata.LaserA.ToString(),
                Name2 = "GFP",
                Value2 = metadata.LaserB.ToString(),
                Name3 = "RFP",
                Value3 = metadata.LaserC.ToString(),
                Name4 = "NIR",
                Value4 = metadata.LaserD.ToString(),
            });
            metadataCollection.Add(new MetadataModel()
            {
                Group = "Detector",
                Name1 = "DAPI",
                Value1 = metadata.GainA.ToString(),
                Name2 = "GFP",
                Value2 = metadata.GainB.ToString(),
                Name3 = "RFP",
                Value3 = metadata.GainC.ToString(),
                Name4 = "NIR",
                Value4 = metadata.GainD.ToString(),
            });
            metadataCollection.Add(new MetadataModel()
            {
                Group = "Pinhole (μm)",
                Name1 = "DAPI",
                Value1 = metadata.PhA.ToString(),
                Name2 = "GFP",
                Value2 = metadata.PhB.ToString(),
                Name3 = "RFP",
                Value3 = metadata.PhC.ToString(),
                Name4 = "NIR",
                Value4 = metadata.PhD.ToString(),
            });
            metadataCollection.Add(new MetadataModel()
            {
                Group = "Stage",
                Name1 = "X-axis",
                Value1 = metadata.StageX.ToString(),
                Name2 = "Y-axis",
                Value2 = metadata.StageY.ToString(),
                Name3 = "Z-axis",
                Value3 = metadata.StageZ.ToString(),
            });
            metadataCollection.Add(new MetadataModel()
            {
                Group = "FOV (μm)",
                Name1 = "X-axis",
                Value1 = metadata.FovX.ToString(),
                Name2 = "Y-axis",
                Value2 = metadata.FovY.ToString(),
            });
            metadataCollection.Add(new MetadataModel()
            {
                Group = "Format",
                Name1 = "Format",
                Value1 = metadata.FileName.Contains("png") ? "PNG" : "AVI",
                Name2 = "FPS",
                Value2 = metadata.FPS.ToString(),
                Name3 = "AVG",
                Value3 = metadata.Averaging.ToString(),
            });

            return metadataCollection;
        }
    }
}
