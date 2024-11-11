﻿using MagicOnion;
using MagicOnion.Server;
using Shared.Interfaces.Services;

namespace ServerProject.Services
{
    public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
    {
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            Console.WriteLine("Received:"+ x +","+ y);
            return x + y;
        }

        public async UnaryResult<int> SubAsync(int x, int y)
        {
            Console.WriteLine("Received:" + x + "," + y);
            return x - y;
        }
    }
}
