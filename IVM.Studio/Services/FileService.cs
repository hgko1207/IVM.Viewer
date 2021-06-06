using IVM.Studio.Models;
using IVM.Studio.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

/**
 * @Class Name : FileService.cs
 * @Description : 파일 관리 서비스
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.03.29     고형균              최초생성
 *
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
    }
}
