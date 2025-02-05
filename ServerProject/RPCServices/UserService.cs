////////////////////////////////////////////////////////////////////////////
///
///  ユーザー通信スクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using MagicOnion;
using MagicOnion.Server;
using MagicOnionServer.Model.Context;
using Shared.Interfaces.Services;
using Shared.Model.Entity;
using System.Data;
using System.Linq;


namespace ServerProject.Services
{
    public class UserService:ServiceBase<IUserService>, IUserService
    {        
        //ユーザー登録
        public async UnaryResult<User>RegistUserAsync(string name) 
        {
            using var context = new GameDbContext();

            //バリデーションチェック
            if (context.Users.Where(user => user.Name == name).Count() > 0)
            {//DBに同じ名前が登録されていた場合

                //例外を返す
                throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "既に使用されている名前です。");
                
            }
            //entitiyFrameWorkCore

            //テーブルにレコードを追加
            User user = new User();
            user.Name = name;
            user.Token = Guid.NewGuid().ToString();
            user.Created_at = DateTime.Now;
            user.Updated_at = DateTime.Now;
            context.Users.Add(user);

            //DBにデータを保存
            await context.SaveChangesAsync();

            //作成したユーザーのIDを返す
            return user;
        }


        //ID指定でユーザー情報を取得するAPI
        public async UnaryResult<User> GetUserInfoAsync(int userId)
        {
            using var context = new GameDbContext();

            //テーブルからユーザー情報を取得
            User userInfo = context.Users.Where(user => user.Id == userId).First();

            //作成したユーザーの情報を返す
            return userInfo;
        }


        //全ユーザー情報を取得するAPI
        public async UnaryResult<User[]> GetAllUserInfoAsync()
        {
            using var context = new GameDbContext();

            //テーブルからユーザー情報を取得
            User[] usersInfo = context.Users.ToArray();

            //全ユーザーの情報を返す
            return usersInfo;
        }


        //全ユーザー情報を取得するAPI
        public async UnaryResult<bool> UpdateUserInfoAsync(int userId, string userName)
        {
            using var context = new GameDbContext();

            //テーブルからユーザー情報を取得・名前上書き
            User user = context.Users.Where(user => user.Id == userId).First();
            user.Name = userName;

            //DBにデータを保存
            await context.SaveChangesAsync();

            return true;
        }

    }
}
