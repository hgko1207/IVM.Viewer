using IVM.Studio.Models;
using IVM.Studio.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace IVM.Studio.Services
{
    class FileService
    {
        /// <summary>
        /// 주어진 이미지/비디오 파일에 해당되는 메타데이터를 찾습니다.
        /// </summary>
        /// <param name="SlidesRootPath"></param>
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
        /// 주어진 폴더 안의 이미지를 열거합니다.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="approvedExtensions"></param>
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
    }
}
