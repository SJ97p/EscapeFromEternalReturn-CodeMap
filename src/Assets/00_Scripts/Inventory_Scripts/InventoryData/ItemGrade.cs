using UnityEngine;

namespace HBDinosaur_ER_Project.ItemData
{
    // 아이템 등급 열거형
    public enum ItemGrade
    {
        WHITE, // 일반
        GREEN, // 고급
        BLUE, // 희귀
        PURPLE, // 영웅
        ORANGE, // 전설
        RED // 초월
    }

    // 등급별 Color 변환 유틸리티
    public static class ItemGradeExtensions
    {
        public static Color ToColor(this ItemGrade Grade)
        {
            return Grade switch
            {
                ItemGrade.WHITE => new Color(0.42f, 0.33f, 0.15f),
                ItemGrade.GREEN => new Color(0.31f, 0.48f, 0.32f),
                ItemGrade.PURPLE => new Color(0.42f, 0.32f, 0.57f),
                ItemGrade.ORANGE => new Color(0.52f, 0.41f, 0.15f),
                ItemGrade.RED => new Color(0.39f, 0.15f, 0.16f),
                _ => Color.white
            };
        }
    }
}