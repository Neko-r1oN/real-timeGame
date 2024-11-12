using MagicOnion;
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
        public async UnaryResult<int> SumAllAsync(int[] numList)
        {
            Console.WriteLine("Received:" + numList);
            int Allnum = 0;
            foreach (int num in numList) {
                Allnum += num;
            }
            
            return Allnum;
        }
        
        public async UnaryResult<int[]> ColcForOperotionAsync(int x,int y)
        {
            Console.WriteLine("Received:" + x + "," + y);
            int[] list;
            
            list = new int[4];
            for (int t =0;t<=3;t++ )
            {
                switch (t)
                {
                    case 0:
                        list[t] = x + y; 
                        break;
                    case 1:
                        list[t] = x - y;
                        break;
                    case 2: 
                        list[t] = x * y;
                        break;
                    case 3: 
                        list[t] = x / y;
                        break;

                    default:
                        Console.WriteLine("Error!!");
                        break;
                }
            }
            return list;
            

        }

        public async UnaryResult<float> SumAllNumberAsync(Number numArray)
        {
            Console.WriteLine("Received:" + numArray.x + "," + numArray.y);
            return numArray.x + numArray.y;
        }
    }
}
