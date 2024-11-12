<?php

    namespace Database\Seeders;

    use App\Models\User;
    use Illuminate\Database\Console\Seeds\WithoutModelEvents;
    use Illuminate\Database\Seeder;

    class UsersTableSeeder extends Seeder
    {
        /**
         * Run the database seeds.
         */
        public function run(): void
        {

            User::create([

                'id' => 1,
                'name' => 'r1oN',
                'password' => 'r1oN.22',
                'level' => 29,
                'exp' => 290,
                'life' => 1000,
            ]);
            User::create([
                'id' => 2,
                'name' => 'SyuEn',
                'password' => 'SyuEn7',
                'level' => 44,
                'exp' => 777,
                'life' => 4649,
            ]);
            User::create([
                'id' => 3,
                'name' => 'GOD',
                'password' => 'XxGODxX',
                'level' => 999,
                'exp' => 9999,
                'life' => 99999,
            ]);

            User::factory(100)->create();
        }

    }
