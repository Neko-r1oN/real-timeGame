////////////////////////////////////////////////////////////////////////////
///
///  API定義(テスト)スクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using MagicOnion;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces.Services
{
    public interface IMyFirstService:IService<IMyFirstService>
    {
        //ここにどのようなAPIを作るか、関数形式で定義を作成

        /// <summary>
        /// 足し算処理
        /// </summary>
        /// <param name="x">足す数x</param>
        /// <param name="y">足す数y</param>
        /// <returns>xとyの合計値</returns>
        UnaryResult<int> SumAsync(int x,int y);
        /// <summary>
        /// 引き算処理
        /// </summary>
        /// <param name="x">元の数x</param>
        /// <param name="y">引く数y</param>
        /// <returns>xとyの合計値</returns>
        UnaryResult<int> SubAsync(int x,int y);

        UnaryResult<int> SumAllAsync(int[] numList);

        UnaryResult<int[]> ColcForOperotionAsync(int x,int y);

        UnaryResult<float> SumAllNumberAsync(Number numArray);


    }


    [MessagePackObject]
    public class Number
    {
        [Key(0)]
        public float x;
        [Key(1)]
        public float y;
    }
}
