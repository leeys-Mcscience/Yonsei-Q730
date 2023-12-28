using McQLib.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace McQLib.Recipes
{
    /// <summary>
    /// 모든 레시피 클래스의 패킷에 포함되는 안전 조건, 종료 조건 및 기록 조건에 대한 속성을 구현하는 최상위 클래스입니다.
    /// </summary>
    public abstract class Recipe : ICloneable
    {
        /// <summary>
        /// 레시피의 이름을 가져옵니다.
        /// </summary>
        [Browsable( false )]
        public string Name => GetType().Name;

        /// <summary>
        /// 시퀀스를 패킷으로 변환하는 과정에서 레시피 오류가 발생했을 경우 발생한 위치를 표기하기 위한 플래그입니다.
        /// </summary>
        [Browsable( false )]
        public string Error { get; set; }

        [Browsable( false )]
        public abstract Image Icon { get; }

        /// <summary>
        /// 레시피의 설명 정보를 반환합니다.
        /// </summary>
        /// <returns>레시피의 설명 정보입니다.</returns>
        public abstract string GetManualString();

        /// <summary>
        /// 레시피의 요약 정보를 반환합니다.
        /// </summary>
        /// <returns>레시피의 요약 정보입니다.</returns>
        public abstract string GetSummaryString();

        /// <summary>
        /// 툴팁에 표시하기 위한 레시피의 세부 정보를 반환합니다.
        /// </summary>
        /// <returns>레시피의 세부 정보입니다.</returns>
        public virtual string GetDetailString()
        {
            var str = string.Empty;

            if( EndCondition != null ) str += EndCondition.GetDetailString() + "\r\n";
            if( SaveCondition != null ) str += SaveCondition.GetDetailString() + "\r\n";
            if( SafetyCondition != null ) str += SafetyCondition.GetDetailString() + "\r\n";

            return str;
        }

        /// <summary>
        /// 속성의 값에 따라 다른 속성의 특성이나 값이 변경되어야 한다면 아래 메서드에 관련된 처리를 구현하십시오.
        /// <br>PropertyGrid에 지정되는 레시피 인스턴스가 변경될 때 이 메서드를 호출하도록 하십시오.</br>
        /// <br>여러 단계 파생되었다면 이 메서드를 재정의 할 때 반드시 부모 클래스의 이 메서드도 함께 호출하십시오.</br>
        /// </summary>
        public virtual void Refresh() { }

        /// <summary>
        /// 이 레시피의 복사본을 생성합니다.
        /// <br><see cref="Recipe.Clone()"/>은 레시피 복사/붙여넣기 기능과 Sequence.CopyTo() 메서드를 위해 레시피의 깊은 복사를 구현합니다.</br>
        /// <br>여기서 깊은 복사라 함은, 레시피에 속한 모든 필드와 필드 형태의 속성(단순히 get과 set으로 접근/설정만 가능한 필드처럼 사용되는 속성)의 완전한 깊은 복사를 의미합니다.</br>
        /// </summary>
        /// <returns>레시피의 복사본입니다.</returns>
        public abstract object Clone();

        // 아래 Condition 중 사용하지 않고자 하는 Condition은 override 하여 null을 반환하도록 하시오.

        /// <summary>
        /// IRecipe을 구현하는 레시피가 안전 조건을 필요로 한다면 이 속성이 SafetyCondition 개체를 반환하도록 하십시오.
        /// </summary>
        [Category( "Conditions" )]
        [DisplayName( "Save Condition" )]
        [Description( "측정 데이터를 저장하기 위한 조건입니다." )]
        [ID( "FF02" )]
        public virtual SaveCondition SaveCondition => _save;

        /// <summary>
        /// IRecipe을 구현하는 레시피가 종료 조건을 필요로 한다면 이 속성이 EndCondition 개체를 반환하도록 하십시오.
        /// </summary>
        [Category( "Conditions" )]
        [DisplayName( "End Condition" )]
        [Description( "측정을 종료하기 위한 조건입니다. 최소 하나 이상의 값이 설정되어야 합니다." )]
        [ID( "FF01" )]
        public virtual EndCondition EndCondition => _end;

        /// <summary>
        /// IRecipe을 구현하는 레시피가 기록 조건을 필요로 한다면 이 속성이 SaveCondition 개체를 반환하도록 하십시오.
        /// </summary>
        [Category( "Conditions" )]
        [DisplayName( "Safety Condition" )]
        [Description( "샘플을 보호하기 위한 안전 조건입니다. 최소 하나 이상의 값이 설정되어야 합니다." )]
        [ID( "FF00" )]
        public virtual SafetyCondition SafetyCondition => _safety;

        protected SaveCondition _save = new SaveCondition();
        protected EndCondition _end = new EndCondition();
        protected SafetyCondition _safety = new SafetyCondition();

        protected static readonly string _title = "=== Parameter ===\r\n";
        protected static readonly string _trTitle = "=== TR Option ===\r\n";
    }
}
