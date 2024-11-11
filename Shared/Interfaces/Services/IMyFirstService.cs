using MagicOnion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces.Services
{
    public interface IMyFirstService:IService<IMyFirstService>
    { 
        //ここにどのようなAPIを作るか、関数形式で定義を作成

        //『足し算』API 二つの整数を引数で受け取り合計値を返す
        UnaryResult<int> SumAsync(int x,int y);

        UnaryResult<int> SubAsync(int x,int y);
    }
}
