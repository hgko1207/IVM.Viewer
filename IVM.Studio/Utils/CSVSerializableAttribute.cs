using System;

namespace IVM.Studio.Utils
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CSVSerializableAttribute : Attribute
    {
        public int Column { get; }

        public string GroupName { get; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="Column">프로퍼티가 CSV 파일 내에서 차지하는 열의 순서 번호입니다. 같은 클래스 내에서 빈 번호는 허용하나 중복된 번호가 존재하면 안됩니다.</param>
        /// <param name="GroupName">프로퍼티가 소속될 소집합 이름입니다. UI에 표시할 때 같은 소집합에 소속하는 프로퍼티끼리 묶는데 사용됩니다.</param>
        public CSVSerializableAttribute(int Column, string GroupName)
        {
            this.Column = Column;
            this.GroupName = GroupName;
        }
    }
}
